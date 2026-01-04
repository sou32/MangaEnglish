using MangaEnglish.Models;
using MangaEnglish.Services;

namespace MangaEnglish.Pages;

[QueryProperty(nameof(TitleId), "titleId")]
public partial class ChapterSelectPage : ContentPage
{
    private readonly DatabaseService _db;

    public int TitleId { get; set; }

    public ChapterSelectPage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // タイトル ID が無い場合はとりあえず全部出す
        List<Chapter> chapters;
        if (TitleId > 0)
        {
            chapters = await _db.GetChaptersByTitleIdAsync(TitleId);
        }
        else
        {
            chapters = await _db.GetAllChaptersAsync();
        }

        ChapterCollection.ItemsSource = chapters ?? new List<Chapter>();

        // 現在選択中のチャプターを表示
        var selectedChapterId = Preferences.Get("SelectedChapterId", 0);
        if (selectedChapterId == 0 || chapters == null || chapters.Count == 0)
        {
            CurrentChapterLabel.Text = "現在のチャプター：未選択";
            return;
        }

        var current = chapters.FirstOrDefault(c => c.Id == selectedChapterId);
        if (current != null)
        {
            CurrentChapterLabel.Text = $"現在のチャプター：{current.Name}";
        }
        else
        {
            CurrentChapterLabel.Text = "現在のチャプター：未選択";
        }
    }

    private async void ChapterTapped(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Chapter selected)
            return;

        // 選択状態クリア（ハイライト残したくない場合）
        ChapterCollection.SelectedItem = null;

        // 選択保存
        Preferences.Set("SelectedChapterId", selected.Id);

        // ラベル更新
        CurrentChapterLabel.Text = $"現在のチャプター：{selected.Name}";

        // ホームへ戻る
        await Shell.Current.GoToAsync("//home");
    }
}