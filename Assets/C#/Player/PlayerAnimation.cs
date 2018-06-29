using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // component / script referenses
    private Animator _anim;
    private Rigidbody _rb;
    private PlayerGravity _playerGravity;
    private InputHandler _inputHandler;

    // local variables
    private bool _isGrounded;
    private float _yMovement;
    private float _xzMovement;
    private float _x, _z;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponentInParent<Rigidbody>();
        _playerGravity = GetComponentInParent<PlayerGravity>();
        _inputHandler = FindObjectOfType<InputHandler>();
    }

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
    }
}
