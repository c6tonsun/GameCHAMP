using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    public Transform on, off;
    public float speed;
    private float _lerpTime;

    private float _minLerp = 0f, _maxLerp = 1f;

    private void Update()
    {
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
}
