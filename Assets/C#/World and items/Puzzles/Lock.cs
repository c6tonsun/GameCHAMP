using UnityEngine;

public class Lock : VisualizedOverlaps, IPuzzlePiece
{
    public GameObject key;
    private bool _isSendingSignal;
    private bool _oldIsSendingSignal;

    private PuzzleMaster _puzzleMaster;
    public PuzzleMaster PuzzleMaster
    {
        get { return _puzzleMaster; }
        set { _puzzleMaster = value; }
    }

    private new void Update()
    {
        base.Update();

        _oldIsSendingSignal = _isSendingSignal;

        _isSendingSignal = false;
        foreach (Collider col in _colliders)
        {
            if (col.gameObject == key)
            {
                _isSendingSignal = true;
                break;
            }
        }

        CheckSingalChanged();
    }

    public void CheckSingalChanged()
    {
        if (_oldIsSendingSignal != _isSendingSignal)
            _puzzleMaster.CheckPuzzlePieces();
    }

    public bool IsSendingSignal()
    {
        return _isSendingSignal;
    }
}
