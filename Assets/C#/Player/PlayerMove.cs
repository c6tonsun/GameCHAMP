using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

    private Rigidbody _rb;
    private Transform camTransform;
    
    public float movementSpeed = 5;
    private Vector3 _inputVector;
    private Vector3 _movement;
    [Range(0f, 1f)]
    public float lerp;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        camTransform = FindObjectOfType<Camera>().transform;
    }

    private void Update()
    {
        // read and normalize input
        _inputVector.x = Input.GetAxis("Horizontal");
        _inputVector.y = 0f;
        _inputVector.z = Input.GetAxis("Vertical");
        _inputVector.Normalize();

        _movement += camTransform.right * _inputVector.x * Time.deltaTime;
        _movement += camTransform.forward * _inputVector.z * Time.deltaTime;
        _movement.y = 0f;

        transform.forward = Vector3.Lerp(transform.forward, _movement.normalized, lerp);
    }

    private void FixedUpdate()
    {
        _movement *= movementSpeed;

        if (_movement != Vector3.zero && CheckMovementCollisions())
        {
            _rb.MovePosition(transform.position + _movement);
            _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
        }

        _movement = Vector3.zero;
    }
    
    private bool CheckMovementCollisions()
    {
        Vector3 point1 = transform.position - transform.up * 0.5f * transform.localScale.y;
        Vector3 point2 = transform.position + transform.up * 0.5f * transform.localScale.y;

        RaycastHit[] hits = Physics.CapsuleCastAll(point1, point2, transform.localScale.y * 0.49f, _movement.normalized, _movement.magnitude);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponent<UnWalkable>() != null)
                return false;
        }

        return true;
    }
}
