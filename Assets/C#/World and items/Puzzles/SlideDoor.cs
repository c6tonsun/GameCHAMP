﻿using UnityEngine;

public class SlideDoor : VisualizedOverlaps
{
    [Header("Door stuff")]
    public bool isInteractable;
    public Transform close, open;
    public float speed;
    private float _lerpTime;

    private new void Update()
    {
        base.Update();
        
        if (_colliders.Length > 0)
            return;

        _lerpTime += Time.deltaTime * speed;
        transform.position = Vector3.Lerp(close.position, open.position, _lerpTime);

        offset = close.position - transform.position;

        if (_lerpTime < 0 || _lerpTime > 1)
            enabled = false;
    }
    
    public void Close()
    {
        if (speed > 0)
            speed = -speed;

        enabled = true;
    }

    public void Open()
    {
        if (speed < 0)
            speed = -speed;

        enabled = true;
    }

    public void Interact()
    {
        if (_lerpTime < 0.5f)
            Open();
        else
            Close();
    }
}
