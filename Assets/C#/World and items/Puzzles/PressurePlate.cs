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

    private float _massThreshold = 10f;

    protected bool _isUp;
    protected bool _isDown;

    protected void Start()
    {
        _maxClamp = 1f;
        _minClamp = 0;
        _unpressedPos = transform.position;
        _pressedPos = transform.parent.position;
        _lerpTime = 1f;
    }

    protected new void Update()
    {
        base.Update();
        
        _lerpTime = MathHelp.Clamp(_lerpTime + Time.deltaTime * speed, _minClamp, _maxClamp);

        _isUp = _lerpTime == _maxClamp;
        _isDown = _lerpTime == _minClamp;

        float totalMass = 0;

        for (int i = 0; i < _colliders.Length; i++)
        {
            if(_colliders[i].GetComponent<Item>())
            {
                totalMass += _colliders[i].GetComponent<Rigidbody>().mass;
            }            
        }

        if(totalMass >= _massThreshold)
        {
            _hasWeight = true;
            GoDown();
        } 
        else
        {
            _hasWeight = false;
        }

        transform.position = Vector3.Lerp(_pressedPos, _unpressedPos, _lerpTime);

    }

    protected void GoUp()
    {
        if (speed < 0) speed = -speed;
    }

    protected void GoDown()
    {
        if (speed > 0) speed = -speed;
    }

}
