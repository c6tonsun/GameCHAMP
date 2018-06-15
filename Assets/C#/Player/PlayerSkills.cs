using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    private PlayerMagnesis playerMagnesis;
    private Rigidbody _rb;

    private Transform camTransform;
    private Item currentItem;
    private Item lastItem;

    private float distance;
    //private Vector3 offset;
    private RaycastHit hit;

    private float lerp = 0.1f;

    private bool alreadyActivated = false;

    private bool useAim = false;

    private float minDistance = 8f;
    private float maxDistance = 12f;

    private float activationInput;
    private float lastActivationInput;

    private float aimInput;
    private float lastAimInput;

    // Use this for initialization
    void Start()
    {
        camTransform = FindObjectOfType<Camera>().transform;
        playerMagnesis = GetComponent<PlayerMagnesis>();
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        #region aim
        aimInput = Input.GetAxisRaw("Aim");
        
        if(lastAimInput <= 0 && aimInput > 0)
        {
            useAim = !useAim;

            if (!_rb.useGravity)
                useAim = false;

            if (useAim)
            {
                playerMagnesis.MagnesisOn();
            }
            else
            {
                alreadyActivated = false;
                playerMagnesis.MagnesisOff();
            }
        }

        lastAimInput = aimInput;
        #endregion

        if (!alreadyActivated)
        {
            RaycastHandling();
        }
        
        if (currentItem == null)
        {
            return;
        }

        #region actiation
        activationInput = Input.GetAxisRaw("Activation");

        if (lastActivationInput <= 0 && activationInput > 0 && useAim)
        {
            alreadyActivated = !alreadyActivated;
        }

        if (alreadyActivated)
        {
            if(currentItem.currentMode == Item.GravityMode.World || currentItem.currentMode == Item.GravityMode.Self)
            {
                currentItem.SetGravityMode(Item.GravityMode.Player);
                //offset = currentItem.transform.position - hit.point;
                distance = hit.distance;
            }
        }
        else
        {
            currentItem.SetGravityMode(Item.GravityMode.Self);
        }

        lastActivationInput = activationInput;
        #endregion

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

        Vector3 lastPos = currentItem.transform.position;

        distance += Input.GetAxisRaw("Distance input") * 2;

        if (distance < minDistance)
            distance = minDistance;
        else if (distance > maxDistance)
            distance = maxDistance;

        Vector3 pointerPos = camTransform.position + (camTransform.forward * distance);
        currentItem.transform.position = Vector3.Lerp(currentItem.transform.position, pointerPos, lerp);

        Vector3 movement = currentItem.transform.position - lastPos;

        if (currentItem.CanMoveCheck(movement))
        {
            currentItem.SetGravityMode(Item.GravityMode.Player);
        }
        else
        {
            currentItem.transform.position = lastPos;
            currentItem.SetGravityMode(Item.GravityMode.ERROR);
        }

    }
}