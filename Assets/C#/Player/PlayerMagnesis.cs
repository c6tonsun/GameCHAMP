using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagnesis : MonoBehaviour {

    private CameraRotationMouse _cameraRotation;
    private Material _pointer;
    private Rigidbody _rb;

    private bool _isInTransition;
    private float _lerpTime;
    private float _lerpSpeed;
    public float onSpeed;
    public float offSpeed;
    public Vector3 onV3;
    public Vector3 offV3;

    private void Start()
    {
        _cameraRotation = FindObjectOfType<CameraRotationMouse>();
        _pointer = _cameraRotation.GetComponentInChildren<MeshRenderer>().material;
        _rb = GetComponent<Rigidbody>();

        MagnesisOff();
    }

    public void MagnesisOn()
    {
        _lerpSpeed = onSpeed;
        _isInTransition = true;
    }

    public void MagnesisOff()
    {
        _lerpSpeed = offSpeed;
        _isInTransition = true;
    }

    private void Update()
    {
        if (!_rb.useGravity && _lerpTime > 0)
            MagnesisOff();

        if (_isInTransition)
        {
            _lerpTime += _lerpSpeed * Time.deltaTime;
            _pointer.color = new Color(_pointer.color.r, _pointer.color.g, _pointer.color.b, _lerpTime);

            if (_lerpTime > 1)
            {
                _cameraRotation.pivotOffset = onV3;
                _lerpTime = 1f;
                _isInTransition = false;
            }
            else if (_lerpTime < 0)
            {
                _cameraRotation.pivotOffset = offV3;
                _lerpTime = 0f;
                _isInTransition = false;
            }
            else
            {
                _cameraRotation.pivotOffset = Vector3.Lerp(offV3, onV3, _lerpTime);
            }
        }
    }
}
