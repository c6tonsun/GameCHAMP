using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
    
    // gravity
    public float ownGravityY = 5;
    public bool _isOwnGravity;
    private Vector3 _activeGravity;
    private float _maxGravitySqrMagnitude;
    private RaycastHit _hit;

    // movement
    public float movementSpeed = 5;
    private Vector3 _inputVector;
    private Vector3 _movement;
    private Rigidbody _rb;

    [Range(0.5f, 1f)]
    public float factor;
    [Range(0.3f, 0.5f)]
    public float radius;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _activeGravity = Vector3.zero;
    }

    private void Update()
    {
        // read and normalize input
        _inputVector.x = Input.GetAxisRaw("Horizontal");
        _inputVector.y = 0f;
        _inputVector.z = Input.GetAxisRaw("Vertical");
        _inputVector.Normalize();

        _movement += transform.right * _inputVector.x * Time.deltaTime;
        _movement += transform.forward * _inputVector.z * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_isOwnGravity)
                _isOwnGravity = false;
            else if (Physics.SphereCast(transform.position - transform.up * transform.localScale.y * 0.5f, radius, Vector3.down, out _hit, factor - 0.5f))
            {
                if (_hit.collider.GetComponent<UnWalkable>() == null)
                    _isOwnGravity = true;
            }
        }
    }

    private void FixedUpdate()
    {
        #region movement
        _movement *= movementSpeed;

        if (_movement != Vector3.zero && CheckMovementCollisions())
        {
            _rb.MovePosition(transform.position + _movement);
            _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
        }

        _movement = Vector3.zero;
        #endregion

        #region gravity
        if (_isOwnGravity && _rb.velocity.sqrMagnitude > _maxGravitySqrMagnitude)
            _isOwnGravity = false;

        if (_isOwnGravity)
        {
            _activeGravity.y = ownGravityY;
            _maxGravitySqrMagnitude = _activeGravity.y * _activeGravity.y;
        }
        else
        {
            _activeGravity.y = GameManager.WORLD_GRAVITY_Y;
            _maxGravitySqrMagnitude = _activeGravity.y * _activeGravity.y;
        }
        
        _rb.AddForce(_activeGravity * _rb.mass * Time.fixedDeltaTime, ForceMode.Acceleration);
        
        if (_rb.velocity.sqrMagnitude > _maxGravitySqrMagnitude)
            _rb.velocity = _rb.velocity.normalized * _activeGravity.magnitude;
        #endregion
    }
    
    private bool CheckMovementCollisions()
    {
        Vector3 point1 = transform.position - transform.up * 0.5f * transform.localScale.y;
        Vector3 point2 = transform.position + transform.up * 0.5f * transform.localScale.y;

        RaycastHit[] hits = Physics.CapsuleCastAll(point1, point2, transform.localScale.y * 0.5f, _movement.normalized, _movement.magnitude);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponent<UnWalkable>() != null)
                return false;
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position - transform.up * transform.localScale.y * factor, radius);
    }
}
