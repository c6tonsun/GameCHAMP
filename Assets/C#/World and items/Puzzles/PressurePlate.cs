using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : VisualizedOverlaps {

    private Vector3 _pressedPos;
    private Vector3 _unpressedPos;

    protected float _maxClamp;
    protected float _minClamp;

    protected float _lerpTime;
    public float speed;

    protected bool _hasWeight;

    private int _itemCount = 1;

    protected void Start()
    {
        _maxClamp = 1;
        _minClamp = 0;
        _unpressedPos = transform.position;
        _pressedPos = transform.parent.position;
        _lerpTime = 1f;
    }

    protected new void Update()
    {
        base.Update();
        
        _lerpTime = MathHelp.Clamp(_lerpTime + Time.deltaTime * speed, _minClamp, _maxClamp);

        int totalCount = 0;

        for (int i = 0; i < _colliders.Length; i++)
        {
            if(_colliders[i].GetComponent<Item>())
            {
                totalCount += 1;
            }            
        }

        if (totalCount >= _itemCount)
        {
            if (!_hasWeight) _hasWeight = true;

            if (speed > 0)
            {
                speed = -speed;
            }
        }
        else
        {
            if (speed < 0)
            {
                speed = -speed;
            }
        }
        
        transform.position = Vector3.Lerp(_pressedPos, _unpressedPos, _lerpTime);

    }

}
