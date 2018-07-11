using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButton : PressurePlate, IButton {

    private bool _isSendingSignal;
    private bool _isButtonLocked;

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

        if (base._lerpTime == base._maxClamp)
        {
            _hasBeenUp = true;
        }

        if (base._lerpTime == base._minClamp)
        {
            if (_hasBeenUp)
                _alreadyPressed = !_alreadyPressed;

            _hasBeenUp = false;

            if (_alreadyPressed)
            {
                _isLocked = false;
            }
            else
            {
                _isLocked = true;
            }
        }

        if (_isLocked)
        {
            base._maxClamp = 0.5f;
            _isSendingSignal = true;
        }
        else
        {
            base._maxClamp = 1f;
            _isSendingSignal = false;
        }

        base.Update();

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
