using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : MonoBehaviour {

    private Rigidbody _rb;
    private RaycastHit _hit;
    private float gravityInput;
    private float oldGravityInput;
    
    public float maxUpMomentum = 5;
    [Range(0.5f, 1f)]
    public float factor = 0.7f;
    [Range(0.3f, 0.5f)]
    public float radius = 0.35f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
		//aslhhfsjvhasdvakh
        gravityInput = Input.GetAxisRaw("Gravity input");

        if (oldGravityInput <= 0 && gravityInput > 0)
        {
            if (!_rb.useGravity)
                _rb.useGravity = true;
            else if (Physics.SphereCast(transform.position - transform.up * transform.localScale.y * 0.5f, radius, Vector3.down, out _hit, factor - 0.5f))
            {
                if (_hit.collider.GetComponent<UnWalkable>() == null)
                    _rb.useGravity = false;
            }
        }

        oldGravityInput = gravityInput;
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
