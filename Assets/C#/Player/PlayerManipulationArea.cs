using UnityEngine;

public class PlayerManipulationArea : MonoBehaviour
{
    public bool isVisible = false;
    public bool itemsActivated = false;
    
    private Collider[] _colliders;
    private Item[] _oldItems;
    private Item[] _newItems;

    private InputHandler _inputHandler;

    private void Start()
    {
        _inputHandler = FindObjectOfType<InputHandler>();

        _newItems = new Item[0];
        _oldItems = new Item[0];

        transform.parent = null;
    }

    private void Update()
    {

        if (!isVisible)
        {
            return;
        }

        _oldItems = _newItems;
        _colliders = Physics.OverlapSphere(transform.position, MathHelp.AbsBiggest(transform.localScale, ignoreY: false) * 0.5f);
        _newItems = new Item[_colliders.Length];

        for(int i = 0; i < _colliders.Length; i++)
        {
            _newItems[i] = _colliders[i].GetComponent<Item>();
        }

        for (int i = 0; i < _oldItems.Length; i++)
        {
            if(_oldItems[i] != null)
            {
                bool found = false;

                for(int j = 0; j < _newItems.Length && !found; j++)
                {

                    if (_newItems[j] == null)
                        continue;

                    if (_oldItems[i].Equals(_newItems[j]))
                    {
                        found = true;
                    }
                }

                if(!found)
                {
                    _oldItems[i].SetGravityMode(Item.GravityMode.World);
                }

            }
        }

        for(int i = 0; i < _newItems.Length; i++)
        {
            if (_newItems[i] == null)
                continue;

            if (_newItems[i].currentMode == Item.GravityMode.World)
            {
                _newItems[i].SetGravityMode(Item.GravityMode.Self);
            }
            else if (_newItems[i].currentMode == Item.GravityMode.ERROR)
            {
                _newItems[i].offset = _newItems[i].transform.position - transform.position;
            }
        }

        MoveItems();
        
    }

    public void ActivateItems()
    {
        
        for(int i = 0; i < _newItems.Length; i++)
        {
            Item item = _newItems[i];

            if (item != null)
            {
                if (item.currentMode == Item.GravityMode.Self)
                {
                    item.SetGravityMode(Item.GravityMode.Player);
                    //item.transform.position = new Vector3(item.transform.position.x, transform.position.y, item.transform.position.z);
                    item.offset = item.transform.position - transform.position;
                }
            }
        }

        itemsActivated = true;

    }

    public void DeactivateItems()
    {
        for (int i = 0; i < _newItems.Length; i++)
        {

            if(_newItems[i] != null)
            {
                _newItems[i].SetGravityMode(Item.GravityMode.World);
            }
                
        }

        itemsActivated = false;
    }

    public void PushItems()
    {

    }

    public void PullItems()
    {

    }

    public void FreezeItems()
    {

    }

    public void SetVisible(bool value)
    {
        isVisible = value;

        transform.GetComponent<Renderer>().enabled = isVisible;
        transform.GetComponent<Collider>().enabled = isVisible;

        if(!isVisible)
        {
            DeactivateItems();
        }
    }

    public void MoveItems()
    {
        float rotationInput = _inputHandler.GetAxisInput(InputHandler.Axis.Rotation);

        for(int i = 0; i < _newItems.Length; i++)
        {
            Item item = _newItems[i];

            if (item == null)
                continue;

            if (item.currentMode == Item.GravityMode.Player || item.currentMode == Item.GravityMode.ERROR)
            {
                item.DoRotate(rotationInput);

                Vector3 oldPos = item.transform.position;
                Vector3 newPos = Vector3.Lerp(item.transform.position, transform.position + item.offset, 0.3f);

                if (item.CanMoveCheck(newPos - oldPos))
                {
                    item.SetGravityMode(Item.GravityMode.Player);
                    item.transform.position = newPos;
                }
                else
                {
                    item.SetGravityMode(Item.GravityMode.ERROR);
                    item.transform.position = oldPos;
                }
            }
        }

    }

    public bool HasItems()
    {
        
        foreach(Item item in _newItems)
        {
            if(item != null)
            {
                return true;
            }
        }

        return false;
    }

}
