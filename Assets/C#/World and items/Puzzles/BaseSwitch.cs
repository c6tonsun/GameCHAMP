using UnityEngine;

public class BaseSwitch : VisualizedOverlaps
{
    [Header("Switch stuff")]
    public SuccessLight successLight;

    private Vector3 _pressedPos;
    private Vector3 _unpressedPos;
    
    public float speed;

    protected float _lerpTime;
    protected bool _inPosition;

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

        if (successLight == null)
            Debug.LogError("Switch without success light!");
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

        transform.position = Vector3.Lerp(_unpressedPos, _pressedPos, _lerpTime);
    }

    public void GoUp()
    {
        if (speed > 0) speed = -speed;
    }

    public void GoDown()
    {
        if (speed < 0) speed = -speed;
    }
}
