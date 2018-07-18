using UnityEngine;

public class BrokenWire : VisualizedOverlaps, IPuzzlePiece
{
    public BrokenWire connectTo;
    public bool isConnected;
    private bool _oldIsConnected;

    private PuzzleMaster _puzzleMaster;
    public PuzzleMaster PuzzleMaster
    {
        get { return _puzzleMaster; }
        set { _puzzleMaster = value; }
    }

    private new void Update()
    {
        base.Update();

        _oldIsConnected = isConnected;
        isConnected = connectTo.HasMatchingColliders(_colliders);

        CheckSingalChanged();
    }

    public bool HasMatchingColliders(Collider[] colliders)
    {
        // my colliders
        foreach (Collider _col in _colliders)
        {
            // other colliders
            foreach (Collider col in colliders)
            {
                if (_col == col)
                    return true;
            }
        }

        return false;
    }

    #region IPuzzlePiece

    public bool IsSendingSignal()
    {
        return isConnected;
    }

    public void CheckSingalChanged()
    {
        if (_oldIsConnected != isConnected)
            PuzzleMaster.CheckPuzzlePieces();
    }

    #endregion
}
