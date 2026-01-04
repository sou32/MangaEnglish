using MangaEnglish.Services;
using MangaEnglish.Models;

namespace MangaEnglish.Pages;

[QueryProperty(nameof(ChapterId), "chapterId")]
public partial class LearningPage : ContentPage
{
    private readonly DatabaseService _db;

    public int ChapterId { get; set; }

    private List<Word> _words = new();
    private int _index;

    private int mode; // 0 normal, 1 yes/no, 2 quiz
    private int quizLimit;
    private int quizCount;

    private int autoNextSeconds;
    private CancellationTokenSource? autoCTS;

    private Word? _currentQuizWord;

    public LearningPage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;

        // スワイプ
        var swipeLeft = new SwipeGestureRecognizer { Direction = SwipeDirection.Left };
        swipeLeft.Swiped += (s, e) => NextWord();
        RootLayout.GestureRecognizers.Add(swipeLeft);

        var swipeRight = new SwipeGestureRecognizer { Direction = SwipeDirection.Right };
        swipeRight.Swiped += (s, e) => PrevWord();
        RootLayout.GestureRecognizers.Add(swipeRight);

        // ボタン
        NextButton.Clicked += (s, e) => NextWord();
        PrevButton.Clicked += (s, e) => PrevWord();

        YesButton.Clicked += YesClicked;
        NoButton.Clicked += NoClicked;

        // 四択
        Choice1.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => QuizSelect(1)) });
        Choice2.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => QuizSelect(2)) });
        Choice3.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => QuizSelect(3)) });
        Choice4.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => QuizSelect(4)) });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (ChapterId == 0)
            ChapterId = Preferences.Get("SelectedChapterId", 0);

        mode = Preferences.Get("LearningMode", 0);
        quizLimit = Preferences.Get("StudyLimit", 20); // ← 修正済み
        autoNextSeconds = Preferences.Get("AutoNext", 0);

        _words = await _db.GetWordsByChapterIdAsync(ChapterId);

        // ランダム化
        _words = _words.OrderBy(_ => Guid.NewGuid()).ToList();

        _index = 0;
        quizCount = 0;

        ApplyModeUI();

        if (mode == 2)
            StartQuiz();
        else
            ShowWordRandomized(_words[_index]);

        StartAutoNextLoop();
    }

    private void ApplyModeUI()
    {
        NormalPanel.IsVisible = mode == 0;
        YesNoPanel.IsVisible = mode == 1;
        QuizPanel.IsVisible = mode == 2;
    }

    // ===== Yes/No =====
    private void ShowWordRandomized(Word w)
    {
        EnglishLabel.Text = w.English;

        bool showCorrect = Random.Shared.Next(2) == 0;

        JapaneseLabel.Text = showCorrect
            ? w.Japanese
            : _words.Where(x => x.Id != w.Id)
                    .OrderBy(_ => Guid.NewGuid())
                    .First().Japanese;

        ProgressLabel.Text = $"{_index + 1} / {_words.Count}";
    }

    private async void YesClicked(object sender, EventArgs e)
        => await AnswerYesNo(JapaneseLabel.Text == _words[_index].Japanese);

    private async void NoClicked(object sender, EventArgs e)
        => await AnswerYesNo(JapaneseLabel.Text != _words[_index].Japanese);

    private async Task AnswerYesNo(bool correct)
    {
        if (correct) await AnimateCorrect();
        else await AnimateWrong();

        await _db.AddHistoryAsync(new History
        {
            WordId = _words[_index].Id,
            LearnedAt = DateTime.Now
        });

        if (++_index < _words.Count)
            ShowWordRandomized(_words[_index]);
        else
            await Finish();
    }

    // ===== Next / Prev =====

    private async void NextWord()
    {
        if (mode != 0) return;

        autoCTS?.Cancel();

        if (_index < _words.Count - 1)
        {
            _index++;
            await AnimateSlide(-70);
            ShowWordRandomized(_words[_index]);
        }
        else
        {
            await Finish();
            return;
        }

        StartAutoNextLoop();
    }

    private async void PrevWord()
    {
        if (mode != 0) return;

        autoCTS?.Cancel();

        if (_index > 0)
        {
            _index--;
            await AnimateSlide(70);
            ShowWordRandomized(_words[_index]);
        }

        StartAutoNextLoop();
    }

    private async Task AnimateSlide(int offset)
    {
        await WordCard.TranslateTo(offset, 0, 130, Easing.CubicOut);
        await WordCard.TranslateTo(0, 0, 160, Easing.CubicIn);
    }

    // ===== AutoNext =====

    private void StartAutoNextLoop()
    {
        if (autoNextSeconds <= 0 || mode != 0)
            return;

        autoCTS?.Cancel();
        autoCTS = new CancellationTokenSource();

        Device.StartTimer(TimeSpan.FromSeconds(autoNextSeconds), () =>
        {
            if (autoCTS.IsCancellationRequested)
                return false;

            MainThread.BeginInvokeOnMainThread(() => NextWord());
            return true;
        });
    }

    // ===== 四択 =====

    private void StartQuiz()
    {
        if (quizCount >= quizLimit)
        {
            Finish();
            return;
        }

        quizCount++;
        ProgressLabel.Text = $"{quizCount} / {quizLimit}";

        _currentQuizWord = _words[Random.Shared.Next(_words.Count)];

        EnglishLabel.Text = _currentQuizWord.English;
        JapaneseLabel.Text = "";

        var choices = _words
            .OrderBy(_ => Guid.NewGuid())
            .Take(4)
            .Select(x => x.Japanese)
            .ToList();

        if (!choices.Contains(_currentQuizWord.Japanese))
            choices[Random.Shared.Next(4)] = _currentQuizWord.Japanese;

        Choice1Text.Text = choices[0];
        Choice2Text.Text = choices[1];
        Choice3Text.Text = choices[2];
        Choice4Text.Text = choices[3];
    }

    private async void QuizSelect(int index)
    {
        string selected = index switch
        {
            1 => Choice1Text.Text,
            2 => Choice2Text.Text,
            3 => Choice3Text.Text,
            4 => Choice4Text.Text,
            _ => ""
        };

        bool correct = selected == _currentQuizWord!.Japanese;

        var btn = index switch
        {
            1 => Choice1,
            2 => Choice2,
            3 => Choice3,
            4 => Choice4,
            _ => null
        };

        if (btn == null) return;

        if (correct) await AnimateChoiceCorrect(btn);
        else await AnimateChoiceWrong(btn);

        if (correct) await AnimateCorrect();
        else await AnimateWrong();

        await _db.AddHistoryAsync(new History
        {
            WordId = _currentQuizWord.Id,
            LearnedAt = DateTime.Now
        });

        StartQuiz();
    }

    // ===== アニメ =====

    private async Task AnimateChoiceCorrect(Border b)
    {
        await b.ScaleTo(1.06, 120, Easing.CubicOut);
        await b.ScaleTo(1.0, 140, Easing.CubicIn);
    }

    private async Task AnimateChoiceWrong(Border b)
    {
        await b.TranslateTo(-15, 0, 70);
        await b.TranslateTo(15, 0, 70);
        await b.TranslateTo(-8, 0, 60);
        await b.TranslateTo(8, 0, 60);
        await b.TranslateTo(0, 0, 50);
    }

    private async Task AnimateCorrect()
    {
        var old = WordCard.BackgroundColor;
        WordCard.BackgroundColor = Color.FromArgb("#4ADE80");

        await WordCard.ScaleTo(1.08, 140);
        await WordCard.ScaleTo(1.0, 140);

        WordCard.BackgroundColor = old;
    }

    private async Task AnimateWrong()
    {
        var old = WordCard.BackgroundColor;
        WordCard.BackgroundColor = Color.FromArgb("#F87171");

        await Task.Delay(450);

        WordCard.BackgroundColor = old;
    }

    private async Task Finish()
    {
        int count = await _db.GetTodayLearnedCountAsync();
        await Shell.Current.GoToAsync($"result?count={count}");
    }
}
