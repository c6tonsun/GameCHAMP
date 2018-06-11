using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Transform camTransform;
    private Item currentItem;
    private Item lastItem;

    private float distance;
    private Vector3 offset;
    private RaycastHit hit;

    private float lerp = 0.1f;

    private bool alreadyActivated = false;

    // Use this for initialization
    void Start()
    {
        camTransform = transform.GetChild(0).transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(!alreadyActivated)
        {
            RaycastHandling();
        }

        if(currentItem == null)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            alreadyActivated = !alreadyActivated;
        }

        if(alreadyActivated)
        {
            if(currentItem.currentMode == Item.GravityMode.World || currentItem.currentMode == Item.GravityMode.Self)
            {
                currentItem.SetGravityMode(Item.GravityMode.Player);
                offset = currentItem.transform.position - hit.point;
                distance = hit.distance;
            }
        }
        else
        {
            currentItem.SetGravityMode(Item.GravityMode.Self);
        }

        MoveItem();

    }

    private void RaycastHandling()
    {

        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, float.MaxValue))
        {
            lastItem = currentItem;
            currentItem = hit.collider.GetComponent<Item>();

            if(currentItem != null && !alreadyActivated)
            {
                currentItem.SetGravityMode(Item.GravityMode.Self);
            }

            if (lastItem != null && currentItem != lastItem && !alreadyActivated)
            {
                lastItem.SetGravityMode(Item.GravityMode.World);
            }

        }

    }

    private void MoveItem()
    {
        if(currentItem == null || !alreadyActivated)
        {
            return;
        }

        /*
        Vector3 lastPos = currentItem.transform.position;
        distance += Input.GetAxisRaw("Mouse ScrollWheel") * 2;
        currentItem.transform.position = camTransform.position + (camTransform.forward * distance) + offset;
        currentItem.rb.velocity = Vector3.zero;
        */

        Vector3 lastPos = currentItem.transform.position;
        distance += Input.GetAxisRaw("Mouse ScrollWheel") * 2;
        Vector3 pointerPos = camTransform.position + (camTransform.forward * distance);
        currentItem.transform.position = Vector3.Lerp(currentItem.transform.position, pointerPos, lerp);

        Vector3 direction = currentItem.transform.position - lastPos;

        if (Physics.BoxCast(lastPos, transform.localScale * 0.5f, direction, out hit, transform.rotation, direction.magnitude))
        {
            currentItem.transform.position = lastPos;
            currentItem.SetGravityMode(Item.GravityMode.ERROR);
        }
        else
        {
            currentItem.SetGravityMode(Item.GravityMode.Player);
        }

    }
}