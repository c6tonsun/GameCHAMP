using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMovement : MonoBehaviour {

    [HideInInspector]
    public Rigidbody rb;
    private Collider _collider;
    private MeshRenderer mr;

    public float selfGravityY = 5;
    public float slowGravityY = 3;
    public float hoverHeightTreshhold = 1f;
    private Vector3 _activeGravity;
    private float _maxGravitySqrMagnitude;

    public Material defMaterial;
    public Material highMaterial;
    public Material actMaterial;
    public Material stopMaterial;

    private Transform manipulationSphere;

    public bool isColliding = false;

    public enum GravityMode
    {
        ERROR = 0,
        World = 1,
        Self = 2,
        Player = 3,
        Stop = 4,
        Slow = 5
    }

    public GravityMode currentMode;
    
	private void Start ()
    {
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();

        manipulationSphere = FindObjectOfType<ManipulationArea>().transform;

        Collider[] cols = GetComponents<Collider>();
        foreach (Collider col in cols)
        {
            if (col.isTrigger == false)
                _collider = col;
        }

        _activeGravity = Vector3.zero;
        SetGravityMode(GravityMode.World);
    }
    
    private void FixedUpdate ()
    {
        // actively set activeGravity
        if(currentMode == GravityMode.Slow)
        {

            bool abovePlayer = transform.position.y > manipulationSphere.position.y;
            // calculate difference
            float difference = transform.position.y - manipulationSphere.position.y;
            // difference to absolute value
            if (difference < 0)
                difference *= -1;
            bool inTreshhold = difference < hoverHeightTreshhold;

            if (abovePlayer)
                _activeGravity.y = -slowGravityY;
            else
                _activeGravity.y = slowGravityY;

            if (inTreshhold)
                _activeGravity.y *= 0.5f;

            _maxGravitySqrMagnitude = _activeGravity.y * _activeGravity.y;
        }

        if (isColliding)
            return;

        rb.AddForce(_activeGravity * rb.mass * Time.fixedDeltaTime, ForceMode.Acceleration);

        if (rb.velocity.sqrMagnitude < _maxGravitySqrMagnitude)
            rb.velocity = rb.velocity.normalized * _activeGravity.y;
	}

    private void OnCollisionEnter(Collision collision)
    {
        isColliding = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }

    public void SetGravityMode(GravityMode mode)
    {
        if (currentMode == mode)
            return;

        currentMode = mode;

        if(currentMode == GravityMode.World)
        {
            mr.material = defMaterial;
            _activeGravity.y = GameManager.WORLD_GRAVITY_Y;
        }
        else if(currentMode == GravityMode.Self)
        {
            mr.material = highMaterial;
            _activeGravity.y = selfGravityY;
        }
        else if(currentMode == GravityMode.Player)
        {
            mr.material = actMaterial;
            _activeGravity.y = 0f;
        }
        else if(currentMode == GravityMode.Stop)
        {
            mr.material = stopMaterial;
            _activeGravity.y = 0f;
        }
        else if(currentMode == GravityMode.Slow)
        {
            mr.material = defMaterial;
            _activeGravity.y = slowGravityY;
        }
        else
        {
            mr.material = null;
        }

        _maxGravitySqrMagnitude = _activeGravity.y * _activeGravity.y;
    }

    public void Move(Vector3 movement)
    {
        RaycastHit hit;

        if (_collider is BoxCollider)
        {
            if (Physics.BoxCast(transform.position, transform.localScale * 0.5f, movement.normalized, out hit, transform.rotation, movement.magnitude))
                return;
        }
        else if (_collider is SphereCollider)
        {
            if (Physics.SphereCast(transform.position, transform.localScale.x * 0.5f, movement.normalized, out hit, movement.magnitude))
                return;
        }
        else if (_collider is CapsuleCollider)
        {
            Vector3 point1 = transform.position - transform.up * 0.5f * transform.localScale.y;
            Vector3 point2 = transform.position + transform.up * 0.5f * transform.localScale.y;

            if (Physics.CapsuleCast(point1, point2, transform.localScale.x * 0.5f, movement.normalized, movement.magnitude))
                return;
        }

        transform.position += movement;
    }

    public GravityMode GetGravityMode()
    {
        return currentMode;
    }
}
