using MangaEnglish.Pages;

namespace MangaEnglish;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // ルート登録（すべてのページ）
        Routing.RegisterRoute("titles", typeof(TitleSelectPage));
        Routing.RegisterRoute("chapters", typeof(ChapterSelectPage));
        Routing.RegisterRoute("learning", typeof(LearningPage));
        Routing.RegisterRoute("result", typeof(ResultPage));
        Routing.RegisterRoute("settings", typeof(SettingsPage));
    }
}