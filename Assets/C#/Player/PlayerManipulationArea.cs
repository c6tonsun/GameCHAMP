using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManipulationArea : MonoBehaviour {

    [HideInInspector]
    public bool isVisible = false;
    public bool itemsActivated = false;

    private Collider[] hits;

    private CameraControl _camControl;

    private List<Item> _itemsInside;



    private void Start()
    {
        _itemsInside = new List<Item>();
        _camControl = FindObjectOfType<CameraControl>();
    }

    private void Update()
    {

        hits = Physics.OverlapSphere(transform.position, MathHelp.AbsBiggest(transform.localScale, ignoreY: false));

        for(int i = 0; i < hits.Length; i++)
        {
            if(hits[i].GetComponent<Item>())
            {



            }
        }

        if (!isVisible)
        {

            if(transform.GetComponent<Renderer>().enabled || transform.GetComponent<Renderer>().enabled)
            {
                transform.GetComponent<Renderer>().enabled = false;
                transform.GetComponent<Collider>().enabled = false;
            }
        }
        else
        {

            if(!transform.GetComponent<Renderer>().enabled || !transform.GetComponent<Renderer>().enabled)
            {
                transform.GetComponent<Renderer>().enabled = true;
                transform.GetComponent<Collider>().enabled = true;
            }

            foreach (Item item in _itemsInside)
            {
                if (item.currentMode == Item.GravityMode.World)
                {
                    item.SetGravityMode(Item.GravityMode.Self);
                }
            }

            MoveItems();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Item>())
        {
            Item enteredItem = other.GetComponent<Item>();
            _itemsInside.Add(enteredItem);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Item>())
        {
            Item exitedItem = other.GetComponent<Item>();

            bool found = false;
            int index = -1;

            for (int i = 0; i < _itemsInside.Count && !found; i++)
            {
                if (exitedItem.Equals(_itemsInside[i]))
                {
                    index = i;
                    found = true;
                }
            }

            if (index > -1)
            {
                _itemsInside.RemoveAt(index);
                exitedItem.SetGravityMode(Item.GravityMode.World);
            }

        }
    }

    public void ActivateItems()
    {
        
        foreach(Item item in _itemsInside)
        {
            if(item.currentMode == Item.GravityMode.World || item.currentMode == Item.GravityMode.Self)
            {
                item.SetGravityMode(Item.GravityMode.Player);
                item.transform.position = new Vector3(item.transform.position.x, transform.position.y, item.transform.position.z);
            }
        }

        itemsActivated = true;

    }

    public void DeactivateItems()
    {
        foreach (Item item in _itemsInside)
        {
            if (item.currentMode == Item.GravityMode.Player)
            {
                item.SetGravityMode(Item.GravityMode.World);
            }
        }

        itemsActivated = false;
    }

    public void PushItems()
    {
        foreach(Item item in _itemsInside)
        {
            item.rb.AddForce(_camControl.transform.forward * 100, ForceMode.Impulse);
            item.SetGravityMode(Item.GravityMode.World);
            _itemsInside.Remove(item);
        }
    }

    public void PullItems()
    {
        foreach (Item item in _itemsInside)
        {
            item.rb.AddForce(_camControl.transform.forward * -100, ForceMode.Impulse);
            item.SetGravityMode(Item.GravityMode.World);
            _itemsInside.Remove(item);
        }
    }

    public void SetVisible(bool value)
    {
        isVisible = value;
    }

    public void MoveItems()
    {
        foreach(Item item in _itemsInside)
        {
            if(item.currentMode == Item.GravityMode.Player)
            {
                Vector3 offset = item.transform.position - transform.position;
                item.transform.position = Vector3.Lerp(item.transform.position, transform.position + offset, 0.7f);
            }
        }
    }

    
}
