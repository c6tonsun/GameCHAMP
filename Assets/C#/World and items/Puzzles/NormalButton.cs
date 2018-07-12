using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalButton : PressurePlate, IButton {

    public bool _isSendingSignal;
    private bool _isButtonLocked;

    public bool _isInverted;

    private bool _isLocked;
    public float _delay;
    private float _delayTimer;

    private new void Start()
    {
        base.Start();
    }

    private new void Update()
    {
        base.Update();

        if (base._isUp)
        {
            if (_isSendingSignal) _isSendingSignal = false;
        }
        else if(base._isDown)
        {
            if (!_isSendingSignal) _isSendingSignal = true;

            if(base._hasWeight)
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

        if (!_isLocked && base._isDown) base.GoUp(); 

        

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
