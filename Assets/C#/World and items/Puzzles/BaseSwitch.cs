using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSwitch : VisualizedOverlaps
{

    private bool _isMovable;
    private bool _isRotable;

    private Vector3 _pressedPos;
    private Vector3 _unpressedPos;

    private Quaternion _flickedRot;
    private Quaternion _unflickedRot;

    public float speed;

    private float _lerpTime;

    protected float _maxClamp;
    protected float _minClamp;

    protected void Start()
    {
        _maxClamp = 1f;
        _minClamp = 0;
        _unpressedPos = transform.position;
        _pressedPos = transform.parent.position;
        _unflickedRot = transform.rotation;
        _flickedRot = transform.parent.rotation;
    }

    protected new void Update()
    {
        base.Update();

        _lerpTime = MathHelp.Clamp(_lerpTime + Time.deltaTime * speed, _minClamp, _maxClamp);

        if (_isMovable)
        {
            transform.position = Vector3.Lerp(_unpressedPos, _pressedPos, _lerpTime);
        }
        else if (_isRotable)
        {
            transform.rotation = Quaternion.Lerp(_unflickedRot, _flickedRot, _lerpTime);
        }
    }

    public void GoUp()
    {
        if (speed > 0) speed = -speed;
    }

    public void GoDown()
    {
        if (speed < 0) speed = -speed;
    }

    public float GetMassOfItems()
    {
        float totalMass = 0;

        for (int i = 0; i < _colliders.Length; i++)
        {
            if (_colliders[i].GetComponent<Item>())
            {
                totalMass += _colliders[i].GetComponent<Rigidbody>().mass;
            }
        }

        return totalMass;
    }

    public void SetMovable(bool value)
    {
        _isMovable = value;
    }

    public void SetRotable(bool value)
    {
        _isRotable = value;
    }

}
