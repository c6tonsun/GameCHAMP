using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    private PlayerAim _playerAim;
    private Rigidbody _rb;

    [HideInInspector]
    public bool ignoreJumpInput;
    
    private float maxJumpTime = 1f;
    private float maxJumpTimer;
    [HideInInspector]
    public float factor = 0.75f;
    [HideInInspector]
    public float radius = 0.35f;
    [HideInInspector]
    public bool isGrounded;

    private void Start()
    {
        _playerAim = GetComponent<PlayerAim>();
        _rb = GetComponent<Rigidbody>();
    }

    public void DoUpdate(bool isAiming, bool jumpInput)
    {
        isGrounded = Physics.SphereCast(new Ray(transform.position - transform.up * transform.localScale.y * 0.5f, Vector3.down), radius, factor - 0.5f);

        if (ignoreJumpInput)
        {
            ignoreJumpInput = false;
        }
        else if (!isAiming && jumpInput)
        {
            if (!_rb.useGravity)
            {
                _rb.useGravity = true;
            }
            else if (isGrounded)
            {
                _rb.useGravity = false;
                _playerAim.AimOff();
                maxJumpTimer = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        maxJumpTimer += Time.fixedDeltaTime;

        if (!_rb.useGravity && maxJumpTimer > maxJumpTime)
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
