using UnityEngine;

public class SlideDoor : VisualizedOverlaps, IInteractable
{
    [Header("Door stuff")]
    public bool isInteractable;
    public Transform close, open;
    public float speed;
    private float _lerpTime;

    private void Start()
    {
        if (speed > 0)
            speed = -speed;
    }

    private new void Update()
    {
        base.Update();
        
        if (_colliders.Length > 0 && speed < 0)
            return;

        _lerpTime = MathHelp.Clamp(_lerpTime + Time.deltaTime * speed, 0f, 1f);
        transform.position = Vector3.Lerp(close.position, open.position, _lerpTime);

        offset = close.position - transform.position;

        if (_lerpTime == 0f || _lerpTime == 1f)
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
        if (!isInteractable)
            return;

        if (_lerpTime < 0.5f)
            Open();
        else
            Close();
    }
}
