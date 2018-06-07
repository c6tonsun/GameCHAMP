using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationMouse : MonoBehaviour {
    
    [Header("Rotating")]
    public float maxInputSpeed = 1;
    public float rotationSpeed = 1;
    public float minRotX = -45;
    public float maxRotX = 45;
    
    [Header("Positioning")]
    public float scrollSpeed = 1;
    private float distanceFromPlayer = 4;
    public float minDistanceFromPlayer = 2;
    public float maxDistanceFromPlayer = 6;
    public Vector3 pivotOffset;

    private Transform _player;
    private Vector3 _pivotOffset;
    private Vector3 _euler;
    private RaycastHit _hit;

    private void Start()
    {
        _player = transform.parent;
    }

    private void Update()
    {
        #region player and camera rotations
        float mouseX = Mathf.Clamp(Input.GetAxis("Mouse X"), -maxInputSpeed, maxInputSpeed);
        float mouseY = Mathf.Clamp(Input.GetAxis("Mouse Y"), -maxInputSpeed, maxInputSpeed);

        _player.Rotate(Vector3.up, mouseX * rotationSpeed);
        transform.Rotate(Vector3.right, -mouseY * rotationSpeed);

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
        distanceFromPlayer += Input.GetAxisRaw("Mouse ScrollWheel") * -scrollSpeed;
        if (distanceFromPlayer < minDistanceFromPlayer)
            distanceFromPlayer = minDistanceFromPlayer;
        if (distanceFromPlayer > maxDistanceFromPlayer)
            distanceFromPlayer = maxDistanceFromPlayer;

        _pivotOffset = _player.right * pivotOffset.x + _player.up * pivotOffset.y;

        if (Physics.SphereCast(_player.position + _pivotOffset, 0.35f, -transform.forward, out _hit, distanceFromPlayer))
            transform.position = (_player.position + _pivotOffset) + (-transform.forward * _hit.distance);
        else
            transform.position = (_player.position + _pivotOffset) + (-transform.forward * distanceFromPlayer);
        #endregion
    }
}
