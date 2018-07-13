using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHole : VisualizedOverlaps {

    private Transform _target;
    private float _timer;
    private Vector3 _startPos;
    private Quaternion _startRot;

    private Transform _itemKey;

    private void Start()
    {
        _target = transform.GetChild(0).transform;
        _target.GetComponent<MeshRenderer>().enabled = false;
    }

    private new void Update()
    {
        if(_itemKey == null)
        {
            base.Update();

            for (int i = 0; i < _colliders.Length; i++)
            {
                if (_colliders[i].GetComponent<Item>() && _colliders[i].GetComponent<Key>())
                {
                    _colliders[i].GetComponent<Item>().SetGravityMode(Item.GravityMode.World);
                    _itemKey = _colliders[i].GetComponent<Item>().transform;
                    _startPos = _itemKey.position;
                    _startRot = _itemKey.rotation;
                    _timer = 0;
                    break;
                }
            }
        }
        else
        {
            _timer += Time.deltaTime;

            _itemKey.position = Vector3.Lerp(_startPos, _target.position, _timer);
            _itemKey.rotation = Quaternion.Lerp(_startRot, _target.rotation, _timer);


        }

    }

}
