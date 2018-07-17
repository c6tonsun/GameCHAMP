using UnityEngine;

public class PuzzleMaster : MonoBehaviour
{
    public IPuzzlePiece[] pieces;
    public SlideDoor door;
    private bool openDoor;

    private void Start()
    {
        foreach (IPuzzlePiece piece in pieces)
            piece.PuzzleMaster = this;
    }

    public void CheckPuzzlePieces()
    {
        foreach (IPuzzlePiece piece in pieces)
        {
            if (!piece.IsSendingSignal())
            {
                openDoor = false;
                break;
            }
        }

        if (openDoor)
            door.Open();
        else
            door.Close();
    }
}
