using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    public Transform on, off;
    public float speed;
    private float _lerpTime;

    private void Update()
    {
        _lerpTime = MathHelp.Clamp(_lerpTime + Time.deltaTime * speed, 0f, 1f);

        transform.position = Vector3.Lerp(off.position, on.position, _lerpTime);
        transform.rotation = Quaternion.Lerp(off.rotation, on.rotation, _lerpTime);

        if (_lerpTime == 0f || _lerpTime == 1f)
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
