using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalLever : BaseSwitch {

    [Range(1, 6)]
    public int modeCount;
    [Range(1, 6)]
    public int _activeMode;

    private float[] _modeValues;
    private int _currentMode;

    private int _goToMode;
    private bool _isDone;

    private new void Start()
    {
        base.Start();
        base.SetRotable(true);
        _modeValues = new float[modeCount];
        _currentMode = 0;
        _goToMode = 5;

        float lerpValue = 0;
        float modeDifference = 1f / modeCount;

        for(int i = 0; i < _modeValues.Length; i++)
        {
            if(i == _modeValues.Length-1)
            {
                _modeValues[i] = 1f;
            } 
            else
            {
                _modeValues[i] = lerpValue;
                lerpValue += modeDifference;
            }
        }
    }

    private new void Update()
    {
        if(!_isDone)
        {
            GoLowerMode();
            base.Update();
        }

        if (_currentMode == _goToMode)
        {
            _isDone = true;
        }

    }

    private void GoLowerMode()
    {
        if(_currentMode + 1 < modeCount)
        {
            _currentMode += 1;
            _maxClamp = _modeValues[_currentMode];
            base.GoDown();
        }

    }

    private void GoUpperMode()
    {

    }

}
