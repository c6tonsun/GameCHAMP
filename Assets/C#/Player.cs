using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public CubeHandler currentCubeHandler;
    public CubeHandler lastCubeHandler;
    private CubeHandler.GravityMode curCubeGravMode;

    public float distance;
    public Vector3 offset;

    private Ray ray;
    private RaycastHit hit;

    public bool alreadyActived = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        RaycastHandler();

        if(currentCubeHandler == null)
        {
            alreadyActived = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            alreadyActived = !alreadyActived;
        }

        if (alreadyActived)
        {
            if (curCubeGravMode == CubeHandler.GravityMode.Room || curCubeGravMode == CubeHandler.GravityMode.Self)
            {
                curCubeGravMode = currentCubeHandler.SetGravityMode(CubeHandler.GravityMode.Player);
                offset = currentCubeHandler.transform.position - hit.point;
                distance = hit.distance;
            }
        }
        else
        {
            curCubeGravMode = currentCubeHandler.SetGravityMode(CubeHandler.GravityMode.Self);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(curCubeGravMode == CubeHandler.GravityMode.Player)
            {
                curCubeGravMode = currentCubeHandler.SetGravityMode(CubeHandler.GravityMode.Room);
                currentCubeHandler.rb.AddForce(new Vector3(0, 0, 1000f) * currentCubeHandler.rb.mass * Time.fixedDeltaTime, ForceMode.Impulse);
                alreadyActived = false;
            }
        }

        MoveCube();
	}

    void RaycastHandler()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, float.MaxValue))
        {
            lastCubeHandler = currentCubeHandler;
            currentCubeHandler = hit.collider.GetComponent<CubeHandler>();

            if(currentCubeHandler != null && !alreadyActived)
            {
                curCubeGravMode = currentCubeHandler.SetGravityMode(CubeHandler.GravityMode.Self);
            }

            if(lastCubeHandler != null && currentCubeHandler != lastCubeHandler && !alreadyActived)
            {
                lastCubeHandler.SetGravityMode(CubeHandler.GravityMode.Room);
            }
            
        }

        if (currentCubeHandler == null)
        {
            if(alreadyActived)
            {
                currentCubeHandler = lastCubeHandler;
            }

            lastCubeHandler = null;
            return;
        }


    }

    private void MoveCube()
    {
        
        if(currentCubeHandler == null || curCubeGravMode == CubeHandler.GravityMode.Room || curCubeGravMode == CubeHandler.GravityMode.Self)
        {
            return;
        }

        Vector3 lastPos = currentCubeHandler.transform.position;
        distance += Input.GetAxisRaw("Mouse ScrollWheel") * 2;
        currentCubeHandler.transform.position = ray.GetPoint(distance) + offset;
        currentCubeHandler.rb.velocity = Vector3.zero;

        Vector3 direction = currentCubeHandler.transform.position - lastPos;

        if (Physics.BoxCast(lastPos, transform.localScale * 0.5f, direction, out hit, transform.rotation, direction.magnitude))
        {
            currentCubeHandler.transform.position = lastPos;
            curCubeGravMode = currentCubeHandler.SetGravityMode(CubeHandler.GravityMode.ERROR);
        }
        else
        {
            curCubeGravMode = currentCubeHandler.SetGravityMode(CubeHandler.GravityMode.Player);
        }
    }

}
