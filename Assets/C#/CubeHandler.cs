using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeHandler : MonoBehaviour {

    [HideInInspector]
    public Rigidbody rb;
    public Vector3 selfGravity;
    private float maxSpeed;
    public Vector3 slowGravity;

    public Material defMaterial;
    public Material highMaterial;
    public Material actMaterial;
    public Material stopMaterial;

    public Transform player;

    private MeshRenderer mr;

    public enum GravityMode
    {
        ERROR = 0,
        World = 1,
        Self = 2,
        Player = 3,
        Stop = 4,
        Slow = 5
    }

    public GravityMode currentMode = GravityMode.World;
    
	private void Start ()
    {
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();

        currentMode = GravityMode.World;
        maxSpeed = GameManager.worldGravity.magnitude;
        slowGravity = new Vector3(0, -3, 0);
    }
    
    private void FixedUpdate ()
    {
        if (currentMode == GravityMode.World)
        {
            rb.AddForce(GameManager.worldGravity * rb.mass * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
        else if (currentMode == GravityMode.Self)
        {
            rb.AddForce(selfGravity * rb.mass * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
        else if(currentMode == GravityMode.Slow)
        {

            rb.AddForce(slowGravity * rb.mass * Time.fixedDeltaTime, ForceMode.Acceleration);

            float y = transform.position.y;
            float playerY = player.position.y;
            float threshhold = 1f;

            float slowSpeed = 1f;

            if(y < playerY + threshhold && y > playerY - threshhold)
            {

                if(y > playerY)
                {

                    slowGravity = new Vector3(0, -slowSpeed, 0);
                    maxSpeed = slowGravity.magnitude;
                    
                }
                else if(y < playerY)
                {

                    slowGravity = new Vector3(0, slowSpeed, 0);
                    maxSpeed = slowGravity.magnitude;

                }

                if(y < playerY + threshhold / 2 && y > playerY - threshhold / 2)
                {

                    if(y > playerY)
                    {

                        slowGravity = new Vector3(0, -slowSpeed / 2, 0);
                        maxSpeed = slowGravity.magnitude;

                    }
                    else if(y < playerY)
                    {

                        slowGravity = new Vector3(0, slowSpeed / 2, 0);
                        maxSpeed = slowGravity.magnitude;

                    }

                }

            }
            else if(y > playerY)
            {
                slowGravity = new Vector3(0, -slowSpeed, 0);
                maxSpeed = slowGravity.magnitude;
            } 
            else if(y < playerY)
            {
                slowGravity = new Vector3(0, slowSpeed, 0);
                maxSpeed = slowGravity.magnitude;
            }
            

        }

        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;
	}

    public void SetGravityMode(GravityMode mode)
    {
        if (currentMode == mode)
            return;

        currentMode = mode;

        if(currentMode == GravityMode.World)
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
        else if(currentMode == GravityMode.Slow)
        {
            mr.material = defMaterial;
            
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
