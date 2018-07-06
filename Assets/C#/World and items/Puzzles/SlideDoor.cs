using UnityEngine;

public class SlideDoor : MonoBehaviour
{
    public bool isInteractable;
    public Transform close, open;
    public float speed;
    private float _lerpTime;

    private void Update()
    {
        _lerpTime += Time.deltaTime * speed;
        transform.position = Vector3.Lerp(close.position, open.position, _lerpTime);

        if (_lerpTime < 0 || _lerpTime > 1)
            enabled = false;
    }

    private void Close()
    {
        if (speed > 0)
            speed = -speed;

        enabled = true;
    }

    private void Open()
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
