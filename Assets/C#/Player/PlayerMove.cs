using UnityEngine;

public class PlayerMove : MonoBehaviour {

    private InputHandler _inputHandler;

    private Rigidbody _rb;
    private Transform camTransform;
    private CapsuleCollider _col;
    
    private float movementSpeed = 5;
    private Vector3 _inputVector;
    private Vector3 _movement;
    private float rotationLerp = 0.2f;

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
        _inputVector.x = _inputHandler.GetAxisInput(InputHandler.Axis.MoveX);
        _inputVector.y = 0f;
        _inputVector.z = _inputHandler.GetAxisInput(InputHandler.Axis.MoveZ);
        _inputVector.Normalize();

        _movement += camTransform.right * _inputVector.x * Time.deltaTime;
        _movement += camTransform.forward * _inputVector.z * Time.deltaTime;
        _movement.y = 0f;

        transform.forward = Vector3.Slerp(transform.forward, _movement.normalized, rotationLerp);

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

        RaycastHit[] hits = Physics.CapsuleCastAll(centers[0], centers[1], radius - float.Epsilon * 4, _movement.normalized, _movement.magnitude);

        foreach (RaycastHit hit in hits)
        {
            StaticObject staticObject = hit.collider.GetComponent<StaticObject>();
            if (staticObject != null && !staticObject.isWalkable)
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
