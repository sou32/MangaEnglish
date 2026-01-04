using MangaEnglish.Services;
using MangaEnglish.Models;
namespace MangaEnglish.Pages;

public partial class TitleSelectPage : ContentPage
{
    private readonly DatabaseService _db;

    public TitleSelectPage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var titles = await _db.GetAllTitlesAsync();
        TitleCollection.ItemsSource = titles ?? new List<Title>();
    }

    private async void TitleTapped(object sender, SelectionChangedEventArgs e)
    {
        var selected = e.CurrentSelection.FirstOrDefault() as Title;
        if (selected == null)
            return;

        // 一度選んだあと、ハイライト残るのイヤならクリア
        TitleCollection.SelectedItem = null;

        await Shell.Current.GoToAsync($"chapters?titleId={selected.Id}");
    }
}