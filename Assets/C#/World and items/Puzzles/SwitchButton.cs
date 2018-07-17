using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchButton : BaseSwitch, IPuzzlePiece
{

    public bool _isSendingSignal;
    private bool _isButtonLocked;

    public bool _isInverted;

    private bool _isLocked;
    private bool _alreadyPressed;
    private bool _hasBeenUp;
    
    public PuzzleMaster PuzzleMaster
    {
        get { return PuzzleMaster; }
        set { PuzzleMaster = value; }
    }

    public float massThreshold;

    private new void Start()
    {
        base.Start();
        base.SetMovable(true);

        if (massThreshold < 1f)
            massThreshold = 1f;
    }

    private new void Update()
    {

        if(_isButtonLocked)
        {
            return;
        }

        base.Update();

        float totalMass = base.GetMassOfItems();

        if(totalMass < massThreshold)
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

            if(_alreadyPressed)
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

            if (totalMass < massThreshold) base.GoUp();

        }
        else
        {
            base._minClamp = 0f;
            _isSendingSignal = false;

            if (totalMass < massThreshold) base.GoUp();
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
