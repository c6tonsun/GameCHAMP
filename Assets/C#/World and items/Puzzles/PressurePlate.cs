using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : VisualizedOverlaps {

    private Vector3 _pressedPos;
    private Vector3 _unpressedPos;

    private Quaternion _flickedRot;
    private Quaternion _unFlickedRot;

    protected float _moveMaxClamp;
    protected float _moveMinClamp;

    protected float _rotationMaxClamp;
    protected float _rotationMinClamp;

    protected float _moveLerpTime;
    public float movementSpeed;

    protected float _rotationLerpTime;
    public float rotationSpeed;

    protected bool _hasWeight;

    private float _massThreshold = 10f;

    protected bool _isUp;
    protected bool _isDown;

    protected void Start()
    {
        _moveMaxClamp = 1f;
        _moveMinClamp = 0;
        _rotationMaxClamp = 1f;
        _rotationMinClamp = 0f;
        _unpressedPos = transform.position;
        _pressedPos = transform.parent.position;
        _moveLerpTime = 1f;
    }

    protected new void Update()
    {
        base.Update();
        
        _moveLerpTime = MathHelp.Clamp(_moveLerpTime + Time.deltaTime * movementSpeed, _moveMinClamp, _moveMaxClamp);
        _rotationLerpTime = MathHelp.Clamp(_rotationLerpTime + Time.deltaTime * rotationSpeed, _rotationMaxClamp, _rotationMinClamp);

        _isUp = _moveLerpTime == _moveMaxClamp;
        _isDown = _moveLerpTime == _moveMinClamp;

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

        transform.position = Vector3.Lerp(_pressedPos, _unpressedPos, _moveLerpTime);
        

    }

    protected void GoUp()
    {
        if (movementSpeed < 0) movementSpeed = -movementSpeed;
    }

    protected void GoDown()
    {
        if (movementSpeed > 0) movementSpeed = -movementSpeed;
    }

}
