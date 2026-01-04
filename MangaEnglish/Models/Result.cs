namespace MangaEnglish.Models;

public class Result
{
    public int ResultId { get; set; }
    public int WordId { get; set; }
    public bool IsCorrect { get; set; }
    public DateTime Timestamp { get; set; }
}