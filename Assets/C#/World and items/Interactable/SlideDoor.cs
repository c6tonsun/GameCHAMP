using UnityEngine;

public class SlideDoor : VisualizedOverlaps, IInteractable
{
    [HideInInspector]
    public bool isInteractable;
    [Header("Door stuff")]
    public float speed;
    public Transform close, open;
    private float _lerpTime;

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

        if (speed > 0)
            speed = -speed;
    }

    private new void Update()
    {
        #region color

        if (_isUnderCursor)
            _isUnderCursor = false;
        else
            _material.color = _defaultColor;

        #endregion

        base.Update();
        
        if (_colliders.Length > 0 && speed < 0)
            return;

        _lerpTime = MathHelp.Clamp(_lerpTime + Time.deltaTime * speed, 0f, 1f);
        transform.position = Vector3.Lerp(close.position, open.position, _lerpTime);

        offset = close.position - transform.position;
    }
    
    public void Close()
    {
        if (speed > 0)
            speed = -speed;
    }

    public void Open()
    {
        if (speed < 0)
            speed = -speed;
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

    public void OnCursorHover()
    {
        _isUnderCursor = true;
        _material.color = _otherColor;
    }
}
