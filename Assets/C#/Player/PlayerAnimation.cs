using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // component / script referenses
    private Animator _anim;
    private Rigidbody _rb;
    private PlayerGravity _playerGravity;
    private PlayerAim _playerAim;
    private InputHandler _inputHandler;

    // local variables
    private bool _isGrounded;
    private float _yMovement;
    private float _xzMovement;
    private float _x, _z;

    // layer weight
    private int _aimLayer;

    // head look at object
    public Transform head;
    public Transform target;
    public Vector3 correction;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponentInParent<Rigidbody>();
        _playerGravity = GetComponentInParent<PlayerGravity>();
        _playerAim = GetComponentInParent<PlayerAim>();
        _inputHandler = FindObjectOfType<InputHandler>();

        // Layer index does not change so getting this once is enough.
        _aimLayer = _anim.GetLayerIndex("Aim");
    }

    // Update is done before Unity's animation update
    private void Update()
    {
        // Our input can be from -1 to 1.
        _x = _inputHandler.GetAxisInput(InputHandler.Axis.MoveX);
        _z = _inputHandler.GetAxisInput(InputHandler.Axis.MoveZ);
        // So we need to make them to positive numbers.
        if (_x < 0) _x *= -1;
        if (_z < 0) _z *= -1;
        // And then we add them.
        _xzMovement = _x + _z;
        
        // we get iformation from other components
        _isGrounded = _playerGravity.isGrounded;
        _yMovement = _rb.velocity.y;

        // set parameters to animator
        _anim.SetBool("isGrounded", _isGrounded);
        _anim.SetFloat("yMovement", _yMovement);
        _anim.SetFloat("xzMovement", _xzMovement);

        // layer weight                            aimLerp: 0 = no aim, 1 = full aim
        _anim.SetLayerWeight(_aimLayer, _playerAim.aimLerp);
    }

    // LateUpdate is done after Unity's animation update
    private void LateUpdate()
    {
        // if no target do nothing
        if (target == null)
            return;

        // calculate vector that starts from head and points towards target
        head.forward = target.position - head.position;
        // manual correction because:
        //  - different coordinate systems between 3D software and Unity
        //  - head's forward pointing in wierd direction in editor
        head.Rotate(correction);
    }
}
