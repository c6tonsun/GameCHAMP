using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPuzzlePiece
{
    PuzzleMaster PuzzleMaster { get; set; }
    bool IsSendingSignal();
    bool IsButtonLocked();
    void SetButtonLocked(bool value);
}
