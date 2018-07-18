﻿using System.Collections;
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

    [Header("Switch stuff")]
    public float speed;

    private float _lerpTime;

    protected float _maxClamp;
    protected float _minClamp;

    [Range(0.01f, 100f)]
    public float massThreshold;
    private float _totalMass;
    protected bool _isEnoughMass;
    private Rigidbody _rbForMass;

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

        #region mass calculation

        _totalMass = 0f;
        foreach (Collider col in _colliders)
        {
            _rbForMass = col.GetComponent<Rigidbody>();
            if (_rbForMass != null)
                _totalMass += _rbForMass.mass;
        }
        _isEnoughMass = _totalMass >= massThreshold;

        #endregion

        _lerpTime = MathHelp.Clamp(_lerpTime + Time.deltaTime * speed, _minClamp, _maxClamp);

        if (_isMovable)
        {
            transform.position = Vector3.Lerp(_unpressedPos, _pressedPos, _lerpTime);
        }
        if (_isRotable)
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

    public void SetMovable(bool value)
    {
        _isMovable = value;
    }

    public void SetRotable(bool value)
    {
        _isRotable = value;
    }

}
