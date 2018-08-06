using UnityEngine;

public class NormalButton : BaseSwitch, IPuzzlePiece
{
    private bool _isSendingSignal;
    private bool _oldIsSendingSignal;

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

        if (successLight != null)
            successLight.SetSuccess(_isSendingSignal);
    }

    private new void Update()
    {

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
        {
            _puzzleMaster.CheckPuzzlePieces();
            
            if (successLight != null)
                successLight.SetSuccess(_isSendingSignal);
        }
    }

    #endregion
}
