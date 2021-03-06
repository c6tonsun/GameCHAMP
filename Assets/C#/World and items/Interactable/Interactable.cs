﻿using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    public Transform on, off;
    public float speed;
    private float _lerpTime;

    private float _minLerp = 0f, _maxLerp = 1f;

    [Header("Color stuff")]
    public int materialIndex;
    private Material _material;
    private Color _defaultColor;
    private Color _otherColor;
    private bool _isUnderCursor;

    private void Start()
    {
        #region color

        _material = GetComponent<MeshRenderer>().materials[materialIndex];
        _defaultColor = _material.color;
        _otherColor = FindObjectOfType<GameManager>().itemModeColors[(int)Item.GravityMode.Self - 1];

        #endregion
    }

    private void Update()
    {
        #region color

        if (_isUnderCursor)
            _isUnderCursor = false;
        else
            _material.color = _defaultColor;

        #endregion

        _lerpTime = MathHelp.Clamp(_lerpTime + Time.deltaTime * speed, _minLerp, _maxLerp);

        transform.position = Vector3.Lerp(off.position, on.position, _lerpTime);
        transform.rotation = Quaternion.Lerp(off.rotation, on.rotation, _lerpTime);

        if (_lerpTime == _minLerp || _lerpTime == _maxLerp)
            enabled = false;
    }

    public void Off()
    {
        if (speed > 0)
            speed = -speed;

        enabled = true;
    }

    public void On()
    {
        if (speed < 0)
            speed = -speed;

        enabled = true;
    }

    public void Interact()
    {
        if (_lerpTime < 0.5f)
            On();
        else
            Off();
    }

    public void OnCursorHover()
    {
        _isUnderCursor = true;
        _material.color = _otherColor;
    }
}
