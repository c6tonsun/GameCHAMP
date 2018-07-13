using UnityEngine;

public class PlayerManipulationArea : MonoBehaviour
{
    [HideInInspector]
    public bool isVisible = false;
    [HideInInspector]
    public bool itemsActivated = false;

    private Collider[] _colliders;
    private float _radius;
    private Item[] _oldItems;
    private Item[] _newItems;
    private int _itemCount;

    private Item _item;

    private void Awake()
    {
        transform.parent = null;
    }

    private void Start()
    {
        _radius = MathHelp.AbsBiggest(transform.localScale, ignoreY: false) * 0.5f;
        _newItems = new Item[0];
        _oldItems = new Item[0];
    }

    public void DoUpdate(float rotationInput)
    {
        _oldItems = _newItems;
        _colliders = Physics.OverlapSphere(transform.position, _radius);
        _newItems = new Item[_colliders.Length];

        _itemCount = 0;

        // set items to array, item count
        for(int i = 0; i < _colliders.Length; i++)
        {
            _newItems[i] = _colliders[i].GetComponent<Item>();
            if (_newItems[i] != null)
                _itemCount++;
        }

        if (_itemCount == 0)
            itemsActivated = false;

        // find items that exited area
        bool found;
        for (int i = 0; i < _oldItems.Length; i++)
        {
            if (_oldItems[i] == null)
                continue;

            found = false;

            for (int j = 0; j < _newItems.Length && !found; j++)
            {
                if (_newItems[j] == null)
                    continue;

                if (_oldItems[i] == _newItems[j])
                    found = true;
            }

            if (!found)
            {
                _oldItems[i].SetGravityMode(Item.GravityMode.World);
            }

        }

        // item mode handling
        for(int i = 0; i < _newItems.Length; i++)
        {
            if (_newItems[i] == null || itemsActivated)
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

        MoveItems(rotationInput);
    }

    public void ActivateItems()
    {
        for(int i = 0; i < _newItems.Length; i++)
        {
            _item = _newItems[i];

            if (_item == null)
                continue;

            if (_item.currentMode == Item.GravityMode.Self || _item.currentMode == Item.GravityMode.Freeze)
            {
                _item.SetGravityMode(Item.GravityMode.Player);
                _item.offset = _item.transform.position - transform.position;
            }
        }

        itemsActivated = true;
    }

    public void DeactivateItems()
    {
        if (_newItems == null)
            return;

        for (int i = 0; i < _newItems.Length; i++)
        {
            if(_newItems[i] != null)
            {
                _newItems[i].SetGravityMode(Item.GravityMode.World);
            }
        }

        itemsActivated = false;
    }

    public void FreezeItems()
    {
        foreach (Item item in _newItems)
            item.SetGravityMode(Item.GravityMode.Freeze);

        itemsActivated = false;
    }

    public void ShootItems(Vector3 camPos)
    {
        Vector3[] directions = new Vector3[_newItems.Length];
        for (int i = 0; i < _newItems.Length; i++)
        {
            if (_newItems[i] == null)
                continue;

            _newItems[i].SetGravityMode(Item.GravityMode.World);
            directions[i] = (_newItems[i].transform.position - camPos).normalized;
        }

        StartCoroutine(GameManager.ShootAll(_newItems, directions));

        itemsActivated = false;
    }

    public void SetVisible(bool value)
    {
        isVisible = value;

        transform.GetComponent<Renderer>().enabled = isVisible;
        transform.GetComponent<Collider>().enabled = isVisible;

        if(!isVisible)
        {
            DeactivateItems();
            _newItems = new Item[0];
            _oldItems = new Item[0];
        }
    }

    public void MoveItems(float rotationInput)
    {
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
                return true;
        }

        return false;
    }
}
