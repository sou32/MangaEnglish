namespace MangaEnglish.State;

public static class AppSettings
{
    public static int LastChapterId
    {
        get => Preferences.Get(nameof(LastChapterId), -1);
        set => Preferences.Set(nameof(LastChapterId), value);
    }
}
