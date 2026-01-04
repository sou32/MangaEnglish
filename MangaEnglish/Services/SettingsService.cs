namespace MangaEnglish.Services;

public class SettingsService
{
    private const string KeyQuizLimit = "QuizLimit";
    private const int DefaultQuizLimit = 10;

    public static int GetQuizLimit()
    {
        return Preferences.Get(KeyQuizLimit, DefaultQuizLimit);
    }

    public static void SetQuizLimit(int value)
    {
        Preferences.Set(KeyQuizLimit, value);
    }
}