using UnityEngine;

public class PlayerGravity : MonoBehaviour {

    private PlayerMagnesis _playerMagnesis;
    private Rigidbody _rb;
    private RaycastHit _hit;
    private float newGravityInput;
    private float oldGravityInput;
    
    public float maxUpMomentum = 5;
    [Range(0.5f, 1f)]
    public float factor = 0.7f;
    [Range(0.3f, 0.5f)]
    public float radius = 0.35f;

    private Animator _anim;
    [HideInInspector]
    public bool isGrounded;
    private float _yMovement;

    private void Start()
    {
        _playerMagnesis = GetComponent<PlayerMagnesis>();
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        isGrounded = Physics.SphereCast(transform.position - transform.up * transform.localScale.y * 0.5f, radius, Vector3.down, out _hit, factor - 0.5f);
        _yMovement = _rb.velocity.y;

        newGravityInput = Input.GetAxisRaw("Gravity input");

        if (oldGravityInput <= 0 && newGravityInput > 0)
        {
            if (!_rb.useGravity)
            {
                _rb.useGravity = true;
            }
            else if (isGrounded && _hit.collider.GetComponent<UnWalkable>() == null)
            {
                _rb.useGravity = false;
                _playerMagnesis.MagnesisOff();
            }
        }

        oldGravityInput = newGravityInput;

        // animation
        _anim.SetBool("isGrounded", isGrounded);
        _anim.SetFloat("yMovement", _yMovement);
    }

    private void FixedUpdate()
    {
        if (!_rb.useGravity && _rb.velocity.y > maxUpMomentum)
            _rb.useGravity = true;

        if (!_rb.useGravity)
            _rb.AddForce(-Physics.gravity * _rb.mass * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireSphere(transform.position - transform.up * transform.localScale.y * 0.5f, radius);
        //Gizmos.DrawWireSphere(transform.position - transform.up * transform.localScale.y * factor, radius);
    }
}
