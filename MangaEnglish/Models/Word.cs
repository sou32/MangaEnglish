using SQLite;

namespace MangaEnglish.Models;

public class Word
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int ChapterId { get; set; }

    public string English { get; set; } = string.Empty;
    public string Japanese { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
