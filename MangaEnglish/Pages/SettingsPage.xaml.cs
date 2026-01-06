using Microsoft.Maui.Controls;

namespace MangaEnglish.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();


        RegisterTap(ModeNormal, () => SelectMode(0));
        RegisterTap(ModeYesNo, () => SelectMode(1));
        RegisterTap(ModeQuiz, () => SelectMode(2));

        SaveButton.Clicked += SaveSettings;

        LoadSettings();
    }

    private void RegisterTap(View v, Action action)
    {
        var tap = new TapGestureRecognizer();
        tap.Tapped += (s, e) => action();
        v.GestureRecognizers.Add(tap);
    }

    // ===============================
    // 設定ロード
    // ===============================
    private void LoadSettings()
    {
        int mode = Preferences.Get("LearningMode", 0);
        int limit = Preferences.Get("StudyLimit", 20);
        int autoNext = Preferences.Get("AutoNext", 0);
        
        SelectMode(mode);

        QuizLimitEntry.Text = limit.ToString();
        AutoNextEntry.Text = autoNext.ToString();

        CurrentModeLabel.Text = $"モード：{ModeName(mode)}";
        CurrentQuizLimitLabel.Text = $"問題数：{limit} 問";
        CurrentAutoLabel.Text = autoNext > 0 ? $"自動Next：{autoNext} 秒" : "自動Next：無効";
    }

    private string ModeName(int mode) => mode switch
    {
        0 => "Next / Prev",
        1 => "Yes / No",
        2 => "四択クイズ",
        _ => "不明"
    };

    // ===============================
    // モード選択 UI
    // ===============================
    private void SelectMode(int mode)
    {
        Preferences.Set("LearningMode", mode);

        ModeNormal.StrokeThickness = mode == 0 ? 3 : 1;
        ModeYesNo.StrokeThickness = mode == 1 ? 3 : 1;
        ModeQuiz.StrokeThickness = mode == 2 ? 3 : 1;

        CurrentModeLabel.Text = $"モード：{ModeName(mode)}";
    }

    // ===============================
    // 保存
    // ===============================
    private void SaveSettings(object sender, EventArgs e)
    {
        // 問題数
        if (int.TryParse(QuizLimitEntry.Text, out int limit) && limit > 0)
            Preferences.Set("StudyLimit", limit);

        // 自動Next
        if (int.TryParse(AutoNextEntry.Text, out int sec) && sec >= 0)
            Preferences.Set("AutoNext", sec);

        LoadSettings();

        DisplayAlert("保存完了", "設定を保存しました。", "OK");
    }
}
