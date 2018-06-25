using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManipulationArea : MonoBehaviour {

    public bool _isVisible = false;
    public bool _activateItems = false;

    private CameraControl _camControl;

    private List<Item> _itemsInside;

    private void Start()
    {
        _itemsInside = new List<Item>();
        _camControl = FindObjectOfType<CameraControl>();
    }

    private void Update()
    {
        if(!_isVisible)
        {

            if(transform.GetComponent<Renderer>().enabled || transform.GetComponent<Renderer>().enabled)
            {
                _activateItems = false;
                transform.GetComponent<Renderer>().enabled = false;
                transform.GetComponent<Collider>().enabled = false;
            }
        }
        else
        {
            if(!transform.GetComponent<Renderer>().enabled || !transform.GetComponent<Renderer>().enabled)
            {
                _activateItems = false;
                transform.GetComponent<Renderer>().enabled = true;
                transform.GetComponent<Collider>().enabled = true;
            }
        }


        foreach(Item item in _itemsInside)
        {
            if(item.currentMode == Item.GravityMode.World && !_activateItems)
            {
                item.SetGravityMode(Item.GravityMode.Self);
            }

            if(_activateItems)
            {
                item.SetGravityMode(Item.GravityMode.Player);
            }
            else
            {
                item.SetGravityMode(Item.GravityMode.Self);
            }
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
        _activateItems = true;
    }

    public void DeactivateItems()
    {
        if(_activateItems)
        {
            _activateItems = false;
        }
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
        _isVisible = value;
    }

    
}
