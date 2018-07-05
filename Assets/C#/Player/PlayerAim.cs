using UnityEngine;

public class PlayerAim : MonoBehaviour {

    private CameraControl _cameraRotation;
    private Material _pointer;

    private bool _isInTransition;
    [HideInInspector]
    public float aimLerp;
    private float _lerpSpeed;
    public float onSpeed;
    public float offSpeed;
    public Vector3 onV3;
    public Vector3 offV3;

    private void Start()
    {
        _cameraRotation = FindObjectOfType<CameraControl>();
        _pointer = _cameraRotation.GetComponentInChildren<MeshRenderer>().material;

        AimOff();
    }

    public void AimOn()
    {
        _lerpSpeed = onSpeed;
        _isInTransition = true;
    }

    public void AimOff()
    {
        _lerpSpeed = offSpeed;
        _isInTransition = true;
    }

    private void Update()
    {
        if (_isInTransition)
        {
            aimLerp += _lerpSpeed * Time.deltaTime;
            _pointer.color = new Color(_pointer.color.r, _pointer.color.g, _pointer.color.b, aimLerp);

            if (aimLerp > 1)
            {
                _cameraRotation.pivotOffset = onV3;
                aimLerp = 1f;
                _isInTransition = false;
            }
            else if (aimLerp < 0)
            {
                _cameraRotation.pivotOffset = offV3;
                aimLerp = 0f;
                _isInTransition = false;
            }
            else
            {
                _cameraRotation.pivotOffset = Vector3.Lerp(offV3, onV3, aimLerp);
            }
        }
    }
}
