using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalButton : BaseSwitch, IPuzzlePiece, IButton
{

    private bool _isSendingSignal;
    private bool _oldIsSendingSignal;
    private bool _isButtonLocked;

    public bool _isInverted;

    private bool _isLocked;
    public float _delay;
    private float _delayTimer;

    private PuzzleMaster _puzzleMaster;
    public PuzzleMaster PuzzleMaster
    {
        get { return _puzzleMaster; }
        set { _puzzleMaster = value; }
    }

    private new void Start()
    {
        base.Start();
        base.SetMovable(true);
    }

    private new void Update()
    {
        if (_isButtonLocked)
        {
            return;
        }

        _oldIsSendingSignal = _isSendingSignal;

        base.Update();

        if (!_isEnoughMass && !_isLocked)
        {
            base.GoUp();
            if (_isSendingSignal) _isSendingSignal = false;
        }
        else
        {
            base.GoDown();
            if (!_isSendingSignal) _isSendingSignal = true;

            if (_isEnoughMass)
            {
                _delayTimer = 0;
                _isLocked = true;
            }
            else
            {
                _delayTimer += Time.deltaTime;

                if (_delayTimer > _delay) _isLocked = false;
                else _isLocked = true;
            }
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
            _puzzleMaster.CheckPuzzlePieces();
    }

    #endregion

    #region IButton

    public bool IsButtonLocked()
    {
        return _isButtonLocked;
    }

    public void SetButtonLocked(bool value)
    {
        _isButtonLocked = value;
    }

    #endregion
}
