public interface IPuzzlePiece
{
    PuzzleMaster PuzzleMaster { get; set; }
    bool IsSendingSignal();
    void CheckSingalChanged();
}
