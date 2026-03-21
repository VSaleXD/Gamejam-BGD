public interface IPuzzleRound
{
    bool IsCompleted { get; }
    void BeginPuzzleRound();
    void ResetPuzzleRound();
}