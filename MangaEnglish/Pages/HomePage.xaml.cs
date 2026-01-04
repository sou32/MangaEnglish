using MangaEnglish.Services;
using Microsoft.Maui.Controls;

namespace MangaEnglish.Pages;

public partial class HomePage : ContentPage
{
    private readonly DatabaseService _db;

    public HomePage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;

        StartLearningButton.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => StartLearningTapped())
        });

        SelectTitleButton.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => Shell.Current.GoToAsync("titles"))
        });

        SettingsButton.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => Shell.Current.GoToAsync("settings"))
        });
    }

    private async void StartLearningTapped()
    {
        var chapterId = Preferences.Get("SelectedChapterId", 0);

        if (chapterId == 0)
        {
            await DisplayAlert("チャプター未選択", "作品 → チャプターを選んでください。", "OK");
            return;
        }

        await Shell.Current.GoToAsync($"learning?chapterId={chapterId}");
    }
}