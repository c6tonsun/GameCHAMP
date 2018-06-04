using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public CubeHandler currentCubeHandler;
    public CubeHandler lastCubeHandler;

    public ManipulationArea manipulationArea;

    public Transform camTransform;

    public float distance;
    public Vector3 offset;

    private RaycastHit hit;

    public bool alreadyActived = false;

    private void Update ()
    {
        Vector3 oldPos = transform.position;
        transform.position += new Vector3(Input.GetAxis("Vertical") * Time.deltaTime, 0, 0);
        manipulationArea.ManipulateArea(CubeHandler.GravityMode.Stop, transform.position - oldPos);

        if(!alreadyActived)
        {
            RaycastHandling();
        }

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
            if (currentCubeHandler.currentMode == CubeHandler.GravityMode.Room ||
                currentCubeHandler.currentMode == CubeHandler.GravityMode.Self)
            {
                currentCubeHandler.SetGravityMode(CubeHandler.GravityMode.Player);
                offset = currentCubeHandler.transform.position - hit.point;
                distance = hit.distance;
            }
        }
        else
        {
            currentCubeHandler.SetGravityMode(CubeHandler.GravityMode.Self);
        }

        if (Input.GetKeyDown(KeyCode.Space) && currentCubeHandler.currentMode == CubeHandler.GravityMode.Player)
        {
            currentCubeHandler.SetGravityMode(CubeHandler.GravityMode.Room);
            currentCubeHandler.rb.AddForce(camTransform.forward * 1000f * currentCubeHandler.rb.mass * Time.fixedDeltaTime, ForceMode.Impulse);
            alreadyActived = false;
        }

        MoveCube();
	}

    private void RaycastHandling()
    {
        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, float.MaxValue))
        {
            lastCubeHandler = currentCubeHandler;
            currentCubeHandler = hit.collider.GetComponent<CubeHandler>();

            if(currentCubeHandler != null && !alreadyActived)
            {
                currentCubeHandler.SetGravityMode(CubeHandler.GravityMode.Self);
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
        if (currentCubeHandler == null ||
            currentCubeHandler.currentMode == CubeHandler.GravityMode.Room ||
            currentCubeHandler.currentMode == CubeHandler.GravityMode.Self)
        {
            return;
        }
        
        Vector3 lastPos = currentCubeHandler.transform.position;
        distance += Input.GetAxisRaw("Mouse ScrollWheel") * 2;
        currentCubeHandler.transform.position = camTransform.position + (camTransform.forward * distance) + offset;
        currentCubeHandler.rb.velocity = Vector3.zero;

        Vector3 direction = currentCubeHandler.transform.position - lastPos;

        if (Physics.BoxCast(lastPos, transform.localScale * 0.5f, direction, out hit, transform.rotation, direction.magnitude))
        {
            currentCubeHandler.transform.position = lastPos;
            currentCubeHandler.SetGravityMode(CubeHandler.GravityMode.ERROR);
        }
        else
        {
            currentCubeHandler.SetGravityMode(CubeHandler.GravityMode.Player);
        }
    }
}
