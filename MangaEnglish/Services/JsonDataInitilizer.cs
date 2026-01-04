using System.Text.Json;
using MangaEnglish.Data.JsonModels;
using MangaEnglish.Models;
using MangaEnglish.Services.Interfaces;

namespace MangaEnglish.Services;

public class JsonDataInitializer : IDataInitializer
{
    private readonly DatabaseService _db;

    public JsonDataInitializer(DatabaseService db)
    {
        _db = db;
    }

    public async Task InitializeAsync()
    {
        // タイトルが既にあるなら初期化しない
        var titleCount = await _db.GetTitleCountAsync();
        if (titleCount > 0)
            return;

        // JSON 読み込み
        using var stream = await FileSystem.OpenAppPackageFileAsync("words_sample.json");
        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // ★ RootJson を読む
        var root = JsonSerializer.Deserialize<RootJson>(json, options);
        if (root == null || root.Chapters == null || root.Chapters.Count == 0)
            return;

        // ★ Title 作成
        var title = new Title
        {
            Name = root.Title,
            Description = $"{root.Title} の英単語セット"
        };
        await _db.AddTitleAsync(title);

        // ★ Chapters → Words の流れで DB に入れる
        foreach (var ch in root.Chapters)
        {
            var chapter = new Chapter
            {
                TitleId = title.Id,
                ChapterNumber = ch.ChapterNumber,
                Name = ch.Name
            };
            await _db.AddChapterAsync(chapter);

            foreach (var w in ch.Words)
            {
                await _db.AddWordAsync(new Word
                {
                    ChapterId = chapter.Id,
                    English = w.English,
                    Japanese = w.Japanese
                });
            }
        }

        Console.WriteLine($"JSON Import 完了：{root.Chapters.Count} chapters");
    }
}