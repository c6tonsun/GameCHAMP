using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // component / script referenses
    private Animator _anim;
    private Rigidbody _rb;
    private PlayerGravity _playerGravity;

    // local variables  ( Vector3 is three float variables )
    private Vector3 _lastPos;
    private Vector3 _newPos;
    private bool _isGrounded;
    private float _yMovement;
    private float _xzMovement;  // movement on x and z axis

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponentInParent<Rigidbody>();
        _playerGravity = GetComponentInParent<PlayerGravity>();

        // we need to know starting position
        // if we don't do this our '_lastPos'
        // would be default Vector3 = 0, 0, 0
        _newPos = transform.position;
    }

    private void Update()
    {
        #region xzMovement
        
        // save last frame position and read this frame position
        _lastPos = _newPos;
        _newPos = transform.position;

        // we ignore y axis movement
        _lastPos.y = 0f;
        _newPos.y = 0f;

        // we calculate movement between this and last frame
        Vector3 movement = _newPos - _lastPos;

        // Vector3 magnitude returns lenght of our movement vector
        // lenght of vector on vector
        // A = 0, 1, 0 is 1
        // B = 1, 0, 1 is 1.4...
        _xzMovement = movement.magnitude;

        #endregion
        
        // we get iformation from other components
        _isGrounded = _playerGravity.isGrounded;
        _yMovement = _rb.velocity.y;

        // set parameters to animator
        _anim.SetBool("isGrounded", _isGrounded);
        _anim.SetFloat("yMovement", _yMovement);
        _anim.SetFloat("xzMovement", _xzMovement);
    }
}
