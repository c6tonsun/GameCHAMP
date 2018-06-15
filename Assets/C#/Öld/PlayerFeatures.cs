using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeatures : MonoBehaviour {

    public ItemMovement currentItem;
    public ItemMovement lastItem;

    public ManipulationArea manipulationArea;

    public Transform camTransform;

    public float distance;
    public Vector3 offset;

    //private Vector3 oldPos;

    private RaycastHit hit;

    public bool alreadyActived = false;

    private ItemMovement.GravityMode gravityMode;

    private void Start()
    {
        camTransform = transform.GetChild(0).transform;
        manipulationArea = GetComponentInChildren<ManipulationArea>();
        //oldPos = transform.position;
    }

    private void Update ()
    {

        //manipulationArea.ManipulateArea(ItemMovement.GravityMode.Slow, transform.position - oldPos);
        //oldPos = transform.position;
        

        // Selecting the item
        if(!alreadyActived)
        {
            RaycastHandling();
        }

        if(currentItem == null)
        {
            alreadyActived = false;
            return;
        }

#region Activating the item

        if (Input.GetKeyDown(KeyCode.E))
        {
            alreadyActived = !alreadyActived;
        }

        if (alreadyActived)
        {
            if (gravityMode == ItemMovement.GravityMode.World ||
                gravityMode == ItemMovement.GravityMode.Self)
            {
                currentItem.SetGravityMode(ItemMovement.GravityMode.Player);
                offset = currentItem.transform.position - hit.point;
                distance = hit.distance;
            }
        }
        else
        {
            currentItem.SetGravityMode(ItemMovement.GravityMode.Self);
        }

        #endregion

#region Throwing the item

        if (Input.GetKeyDown(KeyCode.Space) && gravityMode == ItemMovement.GravityMode.Player)
        {
            currentItem.SetGravityMode(ItemMovement.GravityMode.World);
            currentItem.rb.AddForce(camTransform.forward * 1000f * currentItem.rb.mass * Time.fixedDeltaTime, ForceMode.Impulse);
            alreadyActived = false;
        }

#endregion

        // Moving the item
        MoveItem();
	}

    private void RaycastHandling()
    {
        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, float.MaxValue))
        {
            lastItem = currentItem;
            currentItem = hit.collider.GetComponent<ItemMovement>();
            
            if(currentItem != null)
                gravityMode = currentItem.GetGravityMode();

            if (currentItem != null && !alreadyActived)
            {
                currentItem.SetGravityMode(ItemMovement.GravityMode.Self);
            }

            if(lastItem != null && currentItem != lastItem && !alreadyActived)
            {
                lastItem.SetGravityMode(ItemMovement.GravityMode.World);
            }
        }

        if (currentItem == null)
        {
            if(alreadyActived)
            {
                currentItem = lastItem;
            }

            lastItem = null;
        }

        if (currentItem != null)
            gravityMode = currentItem.GetGravityMode();

    }

    private void MoveItem()
    {
        if (currentItem == null ||
            gravityMode == ItemMovement.GravityMode.World ||
            gravityMode == ItemMovement.GravityMode.Self)
        {
            return;
        }
        
        Vector3 lastPos = currentItem.transform.position;
        distance += Input.GetAxisRaw("Mouse ScrollWheel") * 2;
        currentItem.transform.position = camTransform.position + (camTransform.forward * distance) + offset;
        currentItem.rb.velocity = Vector3.zero;

        Vector3 direction = currentItem.transform.position - lastPos;

        if (Physics.BoxCast(lastPos, transform.localScale * 0.5f, direction, out hit, transform.rotation, direction.magnitude))
        {
            currentItem.transform.position = lastPos;
            currentItem.SetGravityMode(ItemMovement.GravityMode.ERROR);
        }
        else
        {
            currentItem.SetGravityMode(ItemMovement.GravityMode.Player);
        }
    }
}
