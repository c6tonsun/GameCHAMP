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

    private float distance = 0;
    private Vector3 offset = Vector3.zero;

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

        //ownG = new Vector3(-9.81f, 0, 0);
    }

    private void Update()
    {
#region GravityMode Logic

        if(GravityMode.ERROR == currentMode)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {

                if (hit.collider.gameObject == gameObject)
                {

                    if (Input.GetMouseButtonDown(0))
                    {
                        if (currentMode != GravityMode.Player)
                        {
                            distance = hit.distance;
                            offset = transform.position - hit.point;
                        }

                        mr.material = actMaterial;
                        currentMode = GravityMode.Player;
                    }
                    else
                    {
                        mr.material = highMaterial;
                        currentMode = GravityMode.Self;
                    }
                }
                else
                {
                    mr.material = defMaterial;
                    currentMode = GravityMode.Room;
                }
            }
        }
        
        #endregion

        if (currentMode == GravityMode.Player)
        {
            Vector3 lastPos = transform.position;
            distance += Input.GetAxisRaw("Mouse ScrollWheel") * 2;
            transform.position = ray.GetPoint(distance) + offset;
            rb.velocity = Vector3.zero;

            Vector3 direction = transform.position - lastPos;

            if (Physics.BoxCast(lastPos, transform.localScale * 0.5f, direction, out hit, transform.rotation, direction.magnitude))
            {
                mr.material = errorMaterial;
                transform.position = lastPos;
            } else
            {
                mr.material = actMaterial;
            }
        }
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

    public void SetOwnGravity(Vector3 gravity)
    {
        ownG = gravity;
    }

}
