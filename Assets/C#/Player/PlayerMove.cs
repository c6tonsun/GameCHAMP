﻿using UnityEngine;

public class PlayerMove : MonoBehaviour {

    private InputHandler _inputHandler;

    private Rigidbody _rb;
    private Transform camTransform;
    private CapsuleCollider _col;
    
    public float movementSpeed = 5;
    private Vector3 _inputVector;
    private Vector3 _movement;
    [Range(0f, 1f)]
    public float lerp;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<CapsuleCollider>();
        camTransform = FindObjectOfType<Camera>().transform;
        _inputHandler = FindObjectOfType<InputHandler>();
    }

    private void Update()
    {
        // read and normalize input
        _inputVector.x = _inputHandler.moveXInput;
        _inputVector.y = 0f;
        _inputVector.z = _inputHandler.moveZInput;
        _inputVector.Normalize();

        _movement += camTransform.right * _inputVector.x * Time.deltaTime;
        _movement += camTransform.forward * _inputVector.z * Time.deltaTime;
        _movement.y = 0f;

        transform.forward = Vector3.Lerp(transform.forward, _movement.normalized, lerp);

        if (transform.position.y < -10)
            ResetPosition();
    }

    private void FixedUpdate()
    {
        _movement *= movementSpeed;
        Vector3 velocity = _rb.velocity;

        if (_movement != Vector3.zero && CheckMovementCollisions())
        {
            _rb.MovePosition(transform.position + _movement);
            _rb.velocity = velocity;
        }

        _movement = Vector3.zero;
    }
    
    private bool CheckMovementCollisions()
    {
        float radius;
        Vector3[] centers = MathHelp.CapsuleEndPoints(_col, out radius);

        RaycastHit[] hits = Physics.CapsuleCastAll(centers[0], centers[1], radius, _movement.normalized, _movement.magnitude);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponent<UnWalkable>() != null)
                return false;
        }

        return true;
    }

    private void ResetPosition()
    {
        transform.position = new Vector3(0f, 2f, 0f);
        _rb.velocity = Vector3.zero;
    }
}
