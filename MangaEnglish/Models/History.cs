using SQLite;

namespace MangaEnglish.Models;

public class History
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int WordId { get; set; }

    public DateTime LearnedAt { get; set; }
}