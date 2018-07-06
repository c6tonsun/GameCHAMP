using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : VisualizedOverlaps {

    private Vector3 _pressedPos;
    private Vector3 _unpressedPos;

    private float _lerpTime;
    public float speed;

    private int _itemCount = 1;

    private void Start()
    {
        _unpressedPos = transform.position;
        _pressedPos = transform.parent.position;
        _lerpTime = 1f;
    }

    private new void Update()
    {
        base.Update();
        
        _lerpTime = MathHelp.Clamp(_lerpTime + Time.deltaTime * speed, 0, 1);

        int totalCount = 0;

        for (int i = 0; i < _colliders.Length; i++)
        {
            if(_colliders[i].GetComponent<Item>())
            {
                totalCount += 1;
            }            
        }

        if(totalCount >= _itemCount)
        {
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

    public bool IsActivated()
    {
        return true;
    }

}
