using UnityEngine;

public class PuzzleMaster : MonoBehaviour
{
    private SlideDoor _door;
    private bool openDoor;
    private IPuzzlePiece[] _pieces;

    private void Start()
    {
        _door = GetComponentInChildren<SlideDoor>();
        _pieces = GetComponentsInChildren<IPuzzlePiece>();

        foreach (IPuzzlePiece piece in _pieces)
            piece.PuzzleMaster = this;

        _door.isInteractable = _pieces.Length == 0;
    }

    public void CheckPuzzlePieces()
    {
        openDoor = true;

        foreach (IPuzzlePiece piece in _pieces)
        {
            if (!piece.IsSendingSignal())
            {
                openDoor = false;
                break;
            }
        }

        if (openDoor)
            _door.Open();
        else
            _door.Close();
    }
}
