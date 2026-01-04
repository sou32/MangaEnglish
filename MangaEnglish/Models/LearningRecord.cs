using SQLite;

namespace MangaEnglish.Models;

public class LearningRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int TitleId { get; set; }
    public int WordId { get; set; }
    public bool IsCorrect { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}