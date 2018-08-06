using UnityEngine;

public class SwitchButton : BaseSwitch, IPuzzlePiece
{
    private bool _isSendingSignal;
    private bool _oldIsSendingSignal;

    public bool _isInverted;

    private bool _isLocked;
    private bool _alreadyPressed;
    private bool _hasBeenUp;

    private PuzzleMaster _puzzleMaster;
    public PuzzleMaster PuzzleMaster
    {
        get { return _puzzleMaster; }
        set { _puzzleMaster = value; }
    }

    private new void Start()
    {
        base.Start();

        if (successLight != null)
            successLight.SetSuccess(_isSendingSignal);
    }

    private new void Update()
    {
        _oldIsSendingSignal = _isSendingSignal;

        base.Update();

        if (!_isEnoughMass)
        {
            base.GoUp();
            _hasBeenUp = true;
        }
        else
        {
            base.GoDown();

            if (_hasBeenUp)
                _alreadyPressed = !_alreadyPressed;

            _hasBeenUp = false;

            if (_alreadyPressed)
            {
                _isLocked = true;
            }
            else
            {
                _isLocked = false;
            }
        }

        if (_isLocked)
        {
            base._minClamp = 0.5f;
            _isSendingSignal = true;

            if (!_isEnoughMass) base.GoUp();

        }
        else
        {
            base._minClamp = 0f;
            _isSendingSignal = false;

            if (!_isEnoughMass) base.GoUp();
        }

        if (_isInverted) _isSendingSignal = !_isSendingSignal;

        CheckSingalChanged();
    }

    #region IPuzzlePiece

    public bool IsSendingSignal()
    {
        return _isSendingSignal;
    }

    public void CheckSingalChanged()
    {
        if (_oldIsSendingSignal != _isSendingSignal)
        {
            _puzzleMaster.CheckPuzzlePieces();
            
            if (successLight != null)
                successLight.SetSuccess(_isSendingSignal);
        }
    }

    #endregion
}
