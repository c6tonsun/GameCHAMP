using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiStepInteractable : MonoBehaviour, IInteractable, IPuzzlePiece {

    private PuzzleMaster _puzzleMaster;
    public PuzzleMaster PuzzleMaster
    {
        get { return _puzzleMaster; }
        set { _puzzleMaster = value; }
    }

    public bool _isSendingSignal;
    private bool _oldIsSendingSignal;

    private bool _isUsedInPuzzle;

    public Transform On, Off;
    public float Speed;

    [Range(3, 6)]
    public int Steps;

    [Range(1,6)]
    public int ActiveStep;

    private float _stepDifference;

    private int _currentStep;

    private float _minLerp = 0f, _maxLerp = 0f;
    private float _lerpTime;

    private bool _isGoingDown;
    
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

        _stepDifference = 1f / (Steps-1);
        _isGoingDown = true;

        if(ActiveStep > Steps)
        {
            Debug.LogError("Active step is too great. Max should be " + Steps);
        }
    }

    private void Update()
    {
        #region color

        if (_isUnderCursor)
            _isUnderCursor = false;
        else
            _material.color = _defaultColor;

        #endregion

        _oldIsSendingSignal = _isSendingSignal;
        _isSendingSignal = _currentStep == ActiveStep-1;

        _isUsedInPuzzle = _puzzleMaster != null;

        _lerpTime = MathHelp.Clamp(_lerpTime + Time.deltaTime * Speed, _minLerp, _maxLerp);

        transform.position = Vector3.Lerp(Off.position, On.position, _lerpTime);
        transform.rotation = Quaternion.Lerp(Off.rotation, On.rotation, _lerpTime);

        if (_isUsedInPuzzle) CheckSingalChanged();
    }

    private void GoStepDown()
    {
        if (Speed < 0) Speed = -Speed;

        _currentStep++;

        if(_currentStep < Steps-1)
        {
            _maxLerp = _currentStep * _stepDifference;
        }
        else
        {
            _isGoingDown = false;
            _maxLerp = 1f;
        }
    }

    private void GoStepUp()
    {
        if (Speed > 0) Speed = -Speed;

        _currentStep--;

        if (_currentStep > 0)
        {
            _minLerp = _currentStep * _stepDifference;
        }
        else
        {
            _isGoingDown = true;
            _minLerp = 0f;
        }
    }

    public void Interact()
    {
        if(_isGoingDown)
        {
            GoStepDown();
        }
        else
        {
            GoStepUp();
        }
    }

    public void OnCursorHover()
    {
        _isUnderCursor = true;
        _material.color = _otherColor;
    }

    public bool IsSendingSignal()
    {
        return _isSendingSignal;
    }

    public void CheckSingalChanged()
    {
        if (_oldIsSendingSignal != _isSendingSignal)
            _puzzleMaster.CheckPuzzlePieces();
    }

}
