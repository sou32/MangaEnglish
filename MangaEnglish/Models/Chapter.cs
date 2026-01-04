using SQLite;

namespace MangaEnglish.Models;

public class Chapter
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int TitleId { get; set; }
    
    public int ChapterNumber { get; set; }
    public string Name { get; set; } = string.Empty;
}