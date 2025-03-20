namespace SkyscraperGameGui;

public class PuzzlesQueue
{
    public string CurrentPuzzleString { get; set; } = "";
    public string CurrentPositionString { get; set; } = "";
    public IEnumerable<string> UpconingPuzzleStrings => upcomingPuzzleStringsQueue;

    private readonly Queue<string> upcomingPuzzleStringsQueue = new();

    public string? TryDequeuePuzzleString()
    {
        if (upcomingPuzzleStringsQueue.Count == 0)
            return null;
        return upcomingPuzzleStringsQueue.Dequeue();
    }

    public void EnqueuePuzzleString(string puzzleString)
    {
        upcomingPuzzleStringsQueue.Enqueue(puzzleString);
    }

    public void ReplaceQueue(IEnumerable<string> NewPuzzleStrings)
    {
        upcomingPuzzleStringsQueue.Clear();
        foreach (var str in NewPuzzleStrings.Where(s => !string.IsNullOrWhiteSpace(s)))
            upcomingPuzzleStringsQueue.Enqueue(str);
    }
}
