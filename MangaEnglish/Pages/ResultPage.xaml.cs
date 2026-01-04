namespace MangaEnglish.Pages;

[QueryProperty(nameof(LearnedCount), "count")]
public partial class ResultPage : ContentPage
{
    public int LearnedCount { get; set; }

    public ResultPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LearnedCountLabel.Text = $"今日の学習：{LearnedCount}語";
    }

    private async void RetryClicked(object sender, EventArgs e)
    {
        var chapterId = Preferences.Get("SelectedChapterId", 0);

        if (chapterId == 0)
        {
            await Shell.Current.GoToAsync("//home");
            return;
        }

        await Shell.Current.GoToAsync($"learning?chapterId={chapterId}");
    }

    private async void HomeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//home");
    }
}