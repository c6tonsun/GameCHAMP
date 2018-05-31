using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeHandler : MonoBehaviour {

    [HideInInspector]
    public Rigidbody rb;
    public Vector3 ownG;
    public float maxSpeed;

    public Material defMaterial;
    public Material highMaterial;
    public Material actMaterial;
    public Material errorMaterial;

    private MeshRenderer mr;

    public enum GravityMode
    {
        ERROR = 0,
        Room = 1,
        Self = 2,
        Player = 3
    }

    public GravityMode currentMode = GravityMode.Room;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
    }

    private void Update()
    { 
       
    }
    
    void FixedUpdate ()
    {
        if (rb.velocity.magnitude < maxSpeed)
        {
            if(currentMode == GravityMode.Room)
            {
                rb.AddForce(GameManager.roomG * rb.mass * Time.fixedDeltaTime, ForceMode.Acceleration);
            }
            else if(currentMode == GravityMode.Self)
            {
                rb.AddForce(ownG * rb.mass * Time.fixedDeltaTime, ForceMode.Acceleration);
            }

        }
	}

    public GravityMode SetGravityMode(GravityMode mode)
    {
        currentMode = mode;

        if(currentMode == GravityMode.Room)
        {
            mr.material = defMaterial;
        }
        else if(currentMode == GravityMode.Self)
        {
            mr.material = highMaterial;
        }
        else if(currentMode == GravityMode.Player)
        {
            mr.material = actMaterial;
        }
        else
        {
            mr.material = errorMaterial;
        }

        return currentMode;
    }

}
