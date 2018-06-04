using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeHandler : MonoBehaviour {

    [HideInInspector]
    public Rigidbody rb;
    public Vector3 selfGravity;
    private float maxSpeed;

    public Material defMaterial;
    public Material highMaterial;
    public Material actMaterial;
    public Material stopMaterial;

    private MeshRenderer mr;

    public enum GravityMode
    {
        ERROR = 0,
        Room = 1,
        Self = 2,
        Player = 3,
        Stop = 4
    }

    public GravityMode currentMode = GravityMode.Room;
    
	private void Start ()
    {
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();

        currentMode = GravityMode.Room;
        maxSpeed = GameManager.worldGravity.magnitude;
    }
    
    private void FixedUpdate ()
    {
        if (currentMode == GravityMode.Room)
        {
            rb.AddForce(GameManager.worldGravity * rb.mass * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
        else if (currentMode == GravityMode.Self)
        {
            rb.AddForce(selfGravity * rb.mass * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;
	}

    public void SetGravityMode(GravityMode mode)
    {
        if (currentMode == mode)
            return;

        currentMode = mode;

        if(currentMode == GravityMode.Room)
        {
            mr.material = defMaterial;
            maxSpeed = GameManager.worldGravity.magnitude;
        }
        else if(currentMode == GravityMode.Self)
        {
            mr.material = highMaterial;
            maxSpeed = selfGravity.magnitude;
        }
        else if(currentMode == GravityMode.Player)
        {
            mr.material = actMaterial;
        }
        else if(currentMode == GravityMode.Stop)
        {
            mr.material = stopMaterial;
            maxSpeed = 0;
        }
        else
        {
            mr.material = null;
        }
    }

    public void Move(Vector3 movement)
    {
        RaycastHit hit;
        if (Physics.BoxCast(transform.position, transform.localScale * 0.5f, movement, out hit, transform.rotation, movement.magnitude))
            return;
        else
            transform.position += movement;
    }
}
