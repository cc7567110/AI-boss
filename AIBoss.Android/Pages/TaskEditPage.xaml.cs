using AIBoss.Models;

namespace AIBoss.Pages;

public partial class TaskEditPage : ContentPage
{
    public TaskEditPage()
    {
        InitializeComponent();
        var task = MainPage.Data.CurrentTask;
        TitleEntry.Text = task.Title;
        GoalEditor.Text = task.Goal;
        var statuses = new[] { "进行中", "已完成", "暂缓" };
        StatusPicker.SelectedIndex = Array.IndexOf(statuses, task.Status) is var idx && idx >= 0 ? idx : 0;
    }

    private void SaveClicked(object? sender, EventArgs e)
    {
        var statuses = new[] { "进行中", "已完成", "暂缓" };
        MainPage.Data.CurrentTask = new CurrentTask
        {
            Title = TitleEntry.Text?.Trim() ?? string.Empty,
            Goal = GoalEditor.Text?.Trim() ?? string.Empty,
            Status = StatusPicker.SelectedIndex >= 0 ? statuses[StatusPicker.SelectedIndex] : "进行中"
        };
        MainPage.SaveData();

        var mainPage = (MainPage)Application.Current!.MainPage!;
        mainPage.WorkPageInstance.RefreshTaskDisplay();

        Navigation.PopAsync();
    }
}
