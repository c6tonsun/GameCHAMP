using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchButton : PressurePlate, IButton {

    public bool _isSendingSignal;
    private bool _isButtonLocked;

    public bool _isInverted;

    private bool _isLocked;
    private bool _alreadyPressed;
    private bool _hasBeenUp;

    private new void Start()
    {
        base.Start();
    }

    private new void Update()
    {

        if(_isButtonLocked)
        {
            return;
        }

        base.Update();

        if (base._isUp)
        {
            _hasBeenUp = true;
        }

        if (base._isDown)
        {
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
            base._maxClamp = 0.5f;
            _isSendingSignal = true;

            if (!base._hasWeight) base.GoUp();

        }
        else
        {
            base._maxClamp = 1f;
            _isSendingSignal = false;

            if (!base._hasWeight) base.GoUp();
        }

        if (_isInverted) _isSendingSignal = !_isSendingSignal;

    }

    public bool IsSendingSignal()
    {
        return _isSendingSignal;
    }

    public bool IsButtonLocked()
    {
        return _isButtonLocked;
    }

    public void SetButtonLocked(bool value)
    {
        _isButtonLocked = value;
    }

}
