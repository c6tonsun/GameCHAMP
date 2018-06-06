using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public ItemMovement currentCubeHandler;
    public ItemMovement lastCubeHandler;

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
        manipulationArea.ManipulateArea(ItemMovement.GravityMode.Slow, transform.position - oldPos);

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
            if (currentCubeHandler.currentMode == ItemMovement.GravityMode.World ||
                currentCubeHandler.currentMode == ItemMovement.GravityMode.Self)
            {
                currentCubeHandler.SetGravityMode(ItemMovement.GravityMode.Player);
                offset = currentCubeHandler.transform.position - hit.point;
                distance = hit.distance;
            }
        }
        else
        {
            currentCubeHandler.SetGravityMode(ItemMovement.GravityMode.Self);
        }

        if (Input.GetKeyDown(KeyCode.Space) && currentCubeHandler.currentMode == ItemMovement.GravityMode.Player)
        {
            currentCubeHandler.SetGravityMode(ItemMovement.GravityMode.World);
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
            currentCubeHandler = hit.collider.GetComponent<ItemMovement>();

            if(currentCubeHandler != null && !alreadyActived)
            {
                currentCubeHandler.SetGravityMode(ItemMovement.GravityMode.Self);
            }

            if(lastCubeHandler != null && currentCubeHandler != lastCubeHandler && !alreadyActived)
            {
                lastCubeHandler.SetGravityMode(ItemMovement.GravityMode.World);
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
            currentCubeHandler.currentMode == ItemMovement.GravityMode.World ||
            currentCubeHandler.currentMode == ItemMovement.GravityMode.Self)
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
            currentCubeHandler.SetGravityMode(ItemMovement.GravityMode.ERROR);
        }
        else
        {
            currentCubeHandler.SetGravityMode(ItemMovement.GravityMode.Player);
        }
    }
}
