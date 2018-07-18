using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // component / script referenses
    private Animator _anim;
    private Rigidbody _rb;
    private PlayerGravity _playerGravity;
    private PlayerSkills _playerSkills;
    private PlayerAim _playerAim;
    private InputHandler _inputHandler;

    // local variables
    private float _xzMovement;
    private float _xMovement, _zMovement;

    // layer weight
    private int _aimLayer;
    private int _powerLayer;

    // look at
    [HideInInspector]
    public bool doLookAt;
    [HideInInspector]
    public Transform target;
    private float lookAtProsent;
    private Quaternion _oldRotation;
    private Vector3 _lookAtDirection;
    // parent
    private Vector3 _correctionParent = new Vector3(0f, 0f, 0f);
    // head
    private Transform _head;
    private Vector3 _correctionHead = new Vector3(0f, 180f, -90f);
    
    private void Start()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponentInParent<Rigidbody>();
        _playerGravity = GetComponentInParent<PlayerGravity>();
        _playerAim = GetComponentInParent<PlayerAim>();
        _playerSkills = GetComponentInParent<PlayerSkills>();
        _inputHandler = FindObjectOfType<InputHandler>();

        // Layer index does not change so getting this once is enough.
        _aimLayer = _anim.GetLayerIndex("Aim");
        _powerLayer = _anim.GetLayerIndex("Power");

        // get head
        Transform[] bones = GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (Transform child in bones)
        {
            if (child.name.Contains("PAA"))
            {
                _head = child;
                continue;
            }
        }
    }

    // Update is done before Unity's animation update
    private void Update()
    {
        // Our input can be from -1 to 1.
        _xMovement = _inputHandler.GetAxisInput(InputHandler.Axis.MoveX);
        _zMovement = _inputHandler.GetAxisInput(InputHandler.Axis.MoveZ);

        // Get absolute values and add them.
        _xzMovement = Mathf.Abs(_xMovement) + Mathf.Abs(_zMovement);

        // set parameters to animator
        _anim.SetBool("isGrounded", _playerGravity.isGrounded);
        _anim.SetFloat("yMovement", _rb.velocity.y);
        _anim.SetFloat("xMovement", _xMovement);
        _anim.SetFloat("zMovement", _zMovement);
        _anim.SetFloat("xzMovement", _xzMovement);
        _anim.SetBool("Power", _playerSkills.alreadyActivated);

        // layer weight                            aimLerp = aim prosent
        _anim.SetLayerWeight(_aimLayer, _playerAim.aimLerp);
        _anim.SetLayerWeight(_powerLayer, lookAtProsent);
    }

    // LateUpdate is done after Unity's animation update
    private void LateUpdate()
    {
        // no target do nothing
        if (target == null)
            return;

        // we don't set target to null
        // instead we just set new target when needed
        // this boolean is used for smoothing look at
        if (doLookAt)
        {
            if (lookAtProsent < 1)
                lookAtProsent += Time.deltaTime;
        }
        else
        {
            if (lookAtProsent > 0)
                lookAtProsent -= Time.deltaTime;
        }

        // first rotate parent
        transform.parent.rotation = RotateTransform(transform.parent, _correctionParent, ignoreYAxis: true);
        // then child
        _head.rotation = RotateTransform(_head, _correctionHead, ignoreYAxis: false);
    }

    private Quaternion RotateTransform(Transform toRotate, Vector3 correction, bool ignoreYAxis)
    {
        // save old rotation
        _oldRotation = toRotate.rotation;
        
        // calculate direction
        _lookAtDirection = target.position - toRotate.position;
        
        // if we don't want to look up or down
        if (ignoreYAxis)
            _lookAtDirection.y = 0f;
        
        // set new forward vector (blue arrow in scene)
        toRotate.forward = _lookAtDirection;
        
        // manual correction because:
        //  - different coordinate systems between 3D software and Unity
        //  - head's forward pointing in wierd direction in editor
        toRotate.Rotate(correction);

        // Quaternion.Lerp = 0% rotation , 100% rotation , float %
        return Quaternion.Lerp(_oldRotation, toRotate.rotation, lookAtProsent);
    }
}
