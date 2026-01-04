using MangaEnglish.Models;
using SQLite;

namespace MangaEnglish.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;

    public DatabaseService(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);
    }

    public async Task InitializeAsync()
    {
        await _db.CreateTableAsync<Title>();
        await _db.CreateTableAsync<Chapter>();
        await _db.CreateTableAsync<Word>();
        await _db.CreateTableAsync<History>();
    }

    // 章が存在するかのチェック
    public Task<int> GetChapterCountAsync()
        => _db.Table<Chapter>().CountAsync();

    public Task<int> AddTitleAsync(Title title)
        => _db.InsertAsync(title);

    public Task<int> AddChapterAsync(Chapter chapter)
        => _db.InsertAsync(chapter);

    public Task<int> AddWordAsync(Word word)
        => _db.InsertAsync(word);

    public Task<int> AddLearningRecordAsync(LearningRecord record)
        => _db.InsertAsync(record);

    public Task<List<Word>> GetWordsByChapterAsync(int chapterId)
        => _db.Table<Word>().Where(w => w.ChapterId == chapterId).ToListAsync();

    public Task<List<Chapter>> GetAllChaptersAsync()
        => _db.Table<Chapter>().ToListAsync();
    
    public Task<List<Title>> GetAllTitlesAsync()
        => _db.Table<Title>().ToListAsync();
    
    public Task<List<Chapter>> GetChaptersByTitleAsync(int titleId)
        => _db.Table<Chapter>()
            .Where(c => c.TitleId == titleId)
            .ToListAsync();
    
    public Task<List<Title>> GetTitlesAsync()
    {
        return _db.Table<Title>().ToListAsync();
    }

    public Task<List<Chapter>> GetChaptersByTitleIdAsync(int titleId)
    {
        return _db.Table<Chapter>()
            .Where(c => c.TitleId == titleId)
            .ToListAsync();
    }

    public Task<List<Word>> GetWordsByChapterIdAsync(int chapterId)
    {
        return _db.Table<Word>()
            .Where(w => w.ChapterId == chapterId)
            .ToListAsync();
    }

    public async Task<List<Word>> GetWordsByTitleIdAsync(int titleId)
    {
        var chapters = await GetChaptersByTitleIdAsync(titleId);
        var chapterIds = chapters.Select(c => c.Id).ToList();

        return await _db.Table<Word>()
            .Where(w => chapterIds.Contains(w.ChapterId))
            .ToListAsync();
    }

    public Task<List<Word>> GetAllWordsAsync()
    {
        return _db.Table<Word>().ToListAsync();
    }
    
    public Task AddHistoryAsync(History h)
    {
        return _db.InsertAsync(h);
    }

    public Task<int> GetTodayLearnedCountAsync()
    {
        var today = DateTime.Today;

        return _db.Table<History>()
            .Where(h => h.LearnedAt >= today)
            .CountAsync();
    }
    public async Task<int> GetTitleCountAsync()
    {
        return await _db.Table<Title>().CountAsync();
    }
    public Task<Chapter> GetChapterByIdAsync(int id)
    {
        return _db.Table<Chapter>()
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();
    }

}