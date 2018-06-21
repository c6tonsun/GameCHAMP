using UnityEngine;

public class CameraControl : MonoBehaviour
{

    private InputHandler _inputHandler;

    [Header("Rotating")]
    public float rotationSpeed = 1;
    public float minRotX = -45;
    public float maxRotX = 45;
    private float _maxRotationInput = 2;
    private Vector2 _rawRotationInput;

    [Header("Positioning"), Range(3f, 7f)]
    public float distanceFromPlayer = 5f;
    [HideInInspector]
    public Vector3 pivotOffset;
    public float currentDistance;

    private Transform _player;
    private Vector3 _pivotOffset;
    private Vector3 _euler;
    private RaycastHit _hit;

    private void Start()
    {
        _player = FindObjectOfType<PlayerMove>().transform;
        _inputHandler = FindObjectOfType<InputHandler>();
    }

    private void Update()
    {
        #region camera rotation
        // input and clamp
        _rawRotationInput.x = MathHelp.Clamp(_inputHandler.lookXInput, -_maxRotationInput, _maxRotationInput);
        _rawRotationInput.y = MathHelp.Clamp(_inputHandler.lookYInput, -_maxRotationInput, _maxRotationInput);

        transform.Rotate(Vector3.up, _rawRotationInput.x * rotationSpeed);
        transform.Rotate(Vector3.right, _rawRotationInput.y * rotationSpeed);

        _euler = transform.eulerAngles;
        // limit up and down
        if (_euler.x > 180)
            _euler.x -= 360;
        if (_euler.x < minRotX)
            _euler.x = minRotX;
        if (_euler.x > maxRotX)
            _euler.x = maxRotX;
        // force camera look straight
        _euler.z = 0f;
        transform.eulerAngles = _euler;
        #endregion

        #region camera position
        _pivotOffset = transform.right * pivotOffset.x + Vector3.up * pivotOffset.y;

        if (Physics.SphereCast(_player.position + _pivotOffset, 0.35f, -transform.forward, out _hit, distanceFromPlayer))
            currentDistance = _hit.distance;
        else
            currentDistance = distanceFromPlayer;

        transform.position = (_player.position + _pivotOffset) + (-transform.forward * currentDistance);
        #endregion
    }
}
