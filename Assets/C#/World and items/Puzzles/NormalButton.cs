using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalButton : BaseSwitch, IPuzzlePiece
{

    public bool _isSendingSignal;
    private bool _isButtonLocked;

    public bool _isInverted;

    private bool _isLocked;
    public float _delay;
    private float _delayTimer;
    
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

        if(totalMass < massThreshold && !_isLocked)
        {
            base.GoUp();
            if (_isSendingSignal) _isSendingSignal = false;
        }
        else
        {
            base.GoDown();
            if (!_isSendingSignal) _isSendingSignal = true;

            if(totalMass >= massThreshold)
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
