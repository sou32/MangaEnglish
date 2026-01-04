namespace MangaEnglish.Data.JsonModels;

public class ChapterJson
{
    public int ChapterNumber { get; set; }
    public string Name { get; set; }
    public List<WordJson> Words { get; set; }
}
