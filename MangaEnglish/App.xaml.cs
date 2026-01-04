using MangaEnglish.Services;

namespace MangaEnglish;

public partial class App : Application
{
    private readonly DatabaseService _db;
    private readonly JsonDataInitializer _initializer;

    // ★ MAUI が必ず使う引数なしコンストラクタ（これが無いと黒画面）
    public App()
    {
        InitializeComponent();
        
        _db = IPlatformApplication.Current.Services.GetService<DatabaseService>();
        _initializer = IPlatformApplication.Current.Services.GetService<JsonDataInitializer>();

        MainPage = new AppShell();

        _ = InitAsync();
    }
    
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
        if (_db != null)
            await _db.InitializeAsync();

        if (_initializer != null)
            await _initializer.InitializeAsync();

        Console.WriteLine("DB + JSON 初期化完了");
    }
    
}