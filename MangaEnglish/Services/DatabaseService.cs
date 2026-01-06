using MangaEnglish.Models;
using SQLite;
using System.Linq;

namespace MangaEnglish.Services;

public class DatabaseService(string dbPath)
{
    private readonly SQLiteAsyncConnection _db = new(dbPath);

    public async Task InitializeAsync()
    {
        await _db.CreateTableAsync<Title>();
        await _db.CreateTableAsync<Chapter>();
        await _db.CreateTableAsync<Word>();
        await _db.CreateTableAsync<History>();
    }

    // ===== Counts =====
    public Task<int> GetTitleCountAsync()
        => _db.Table<Title>().CountAsync();

    public Task<int> GetChapterCountAsync()
        => _db.Table<Chapter>().CountAsync();

    // ===== Insert =====
    public Task<int> AddTitleAsync(Title title)
        => _db.InsertAsync(title);

    public Task<int> AddChapterAsync(Chapter chapter)
        => _db.InsertAsync(chapter);

    public Task<int> AddWordAsync(Word word)
        => _db.InsertAsync(word);

    public Task<int> AddHistoryAsync(History h)
        => _db.InsertAsync(h);

    // ===== Titles / Chapters =====
    public Task<List<Title>> GetAllTitlesAsync()
        => _db.Table<Title>().ToListAsync();

    public Task<List<Chapter>> GetAllChaptersAsync()
        => _db.Table<Chapter>().ToListAsync();

    public Task<List<Chapter>> GetChaptersByTitleIdAsync(int titleId)
        => _db.Table<Chapter>()
            .Where(c => c.TitleId == titleId)
            .ToListAsync();

    public Task<Chapter> GetChapterByIdAsync(int id)
        => _db.Table<Chapter>()
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();

    // ===== Words =====
    public Task<List<Word>> GetWordsByChapterIdAsync(int chapterId)
        => _db.Table<Word>()
            .Where(w => w.ChapterId == chapterId)
            .ToListAsync();

    public async Task<List<Word>> GetWordsByTitleIdAsync(int titleId)
    {
        var chapters = await GetChaptersByTitleIdAsync(titleId);
        if (chapters.Count == 0)
            return new List<Word>();

        var chapterIds = chapters.Select(c => c.Id).ToList();

        return await _db.Table<Word>()
            .Where(w => chapterIds.Contains(w.ChapterId))
            .ToListAsync();
    }

    public Task<List<Word>> GetAllWordsAsync()
        => _db.Table<Word>().ToListAsync();

    public async Task<int> GetTodayLearnedCountAsync()
    {
        var today = DateTime.Today;

        var histories = await _db.Table<History>()
            .Where(h => h.LearnedAt >= today)
            .ToListAsync();

        return histories
            .Select(h => h.WordId)
            .Distinct()
            .Count();
    }
}