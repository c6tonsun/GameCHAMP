using UnityEngine;

public class PlayerGravity : MonoBehaviour {

    private InputHandler _inputHandler;

    private PlayerAim _playerAim;
    private Rigidbody _rb;
    private RaycastHit _hit;
    
    public float maxUpMomentum = 7;
    private float maxJumpTime = 1f;
    private float maxJumpTimer;
    [Range(0.5f, 1f)]
    public float factor = 0.7f;
    [Range(0.3f, 0.5f)]
    public float radius = 0.35f;
    [HideInInspector]
    public bool isGrounded;

    private void Start()
    {
        _playerAim = GetComponent<PlayerAim>();
        _rb = GetComponent<Rigidbody>();
        _inputHandler = FindObjectOfType<InputHandler>();
    }

    private void Update()
    {
        isGrounded = Physics.SphereCast(transform.position - transform.up * transform.localScale.y * 0.5f, radius, Vector3.down, out _hit, factor - 0.5f);

        StaticObject staticObject = null;
        if (isGrounded)
             staticObject = _hit.collider.GetComponent<StaticObject>();

        if (_inputHandler.KeyDown(InputHandler.Key.Jump))
        {
            if (!_rb.useGravity)
            {
                _rb.useGravity = true;
            }
            else if (isGrounded && (staticObject == null || staticObject != null && staticObject.isWalkable))
            {
                _rb.useGravity = false;
                _playerAim.MagnesisOff();
                maxJumpTimer = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        maxJumpTimer += Time.fixedDeltaTime;

        if (!_rb.useGravity && (_rb.velocity.y > maxUpMomentum || maxJumpTimer > maxJumpTime))
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
