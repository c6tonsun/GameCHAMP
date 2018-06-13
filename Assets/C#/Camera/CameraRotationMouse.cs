using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationMouse : MonoBehaviour {
    
    [Header("Rotating")]
    public float maxRotationInput = 1;
    public float rotationSpeed = 1;
    public float minRotX = -45;
    public float maxRotX = 45;
    private Vector2 _rawRotationInput;

    [Header("Positioning")]
    public float scrollSpeed = 1;
    public float distanceFromPlayer = 4;
    [HideInInspector]
    public Vector3 pivotOffset;

    private Transform _player;
    private Vector3 _pivotOffset;
    private Vector3 _euler;
    private RaycastHit _hit;

    private void Start()
    {
        _player = FindObjectOfType<PlayerMove>().transform;
    }

    private void Update()
    {
        #region camera rotation
        // input and clamp
        _rawRotationInput.x = MathHelp.Clamp(Input.GetAxisRaw("Look input X"), -maxRotationInput, maxRotationInput);
        _rawRotationInput.y = MathHelp.Clamp(Input.GetAxisRaw("Look input Y"), -maxRotationInput, maxRotationInput);

        transform.Rotate(Vector3.up, _rawRotationInput.x * rotationSpeed);
        transform.Rotate(Vector3.right, -_rawRotationInput.y * rotationSpeed);

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
            transform.position = (_player.position + _pivotOffset) + (-transform.forward * _hit.distance);
        else
            transform.position = (_player.position + _pivotOffset) + (-transform.forward * distanceFromPlayer);
        #endregion
    }
}
