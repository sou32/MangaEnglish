using MangaEnglish.Services;

namespace MangaEnglish;

public partial class App : Application
{
    private readonly DatabaseService _db;
    private readonly JsonDataInitializer _initializer;

    public App(DatabaseService db, JsonDataInitializer initializer)
    {
        InitializeComponent();

        _db = db;
        _initializer = initializer;

        MainPage = new AppShell();
        
        _ = InitAsync();
    }

    private async Task InitAsync()
    {
        await _db.InitializeAsync();
        await _initializer.InitializeAsync();
        Console.WriteLine("DB + JSON 初期化完了");
    }
}
