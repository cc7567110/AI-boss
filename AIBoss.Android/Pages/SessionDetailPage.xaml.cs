using AIBoss.Models;

namespace AIBoss.Pages;

public partial class SessionDetailPage : ContentPage
{
    private readonly WorkSession _session;

    public SessionDetailPage(WorkSession session)
    {
        InitializeComponent();
        _session = session;

        var minutes = session.WorkedSeconds / 60;
        var seconds = session.WorkedSeconds % 60;
        TimeRangeLabel.Text = $"{session.StartedAt:yyyy-MM-dd HH:mm} ~ {session.EndedAt:yyyy-MM-dd HH:mm}";
        DurationLabel.Text = $"实际时长 {minutes} 分 {seconds} 秒";
        TaskTitleEntry.Text = session.TaskTitle;
        PlannedOutputEditor.Text = session.PlannedOutput;
        ActualOutputEditor.Text = session.ActualOutput;
    }

    private async void SaveClicked(object? sender, EventArgs e)
    {
        _session.TaskTitle = string.IsNullOrWhiteSpace(TaskTitleEntry.Text) ? "未命名任务" : TaskTitleEntry.Text.Trim();
        _session.PlannedOutput = string.IsNullOrWhiteSpace(PlannedOutputEditor.Text) ? "未填写" : PlannedOutputEditor.Text.Trim();
        _session.ActualOutput = string.IsNullOrWhiteSpace(ActualOutputEditor.Text) ? "（未填写）" : ActualOutputEditor.Text.Trim();
        MainPage.SaveData();
        await Navigation.PopModalAsync();
    }

    private async void DeleteClicked(object? sender, EventArgs e)
    {
        var confirm = await DisplayAlert("确认删除", "确定要删除这条学习记录吗？删除后无法恢复。", "删除", "取消");
        if (!confirm) return;

        MainPage.Data.WorkSessions.Remove(_session);
        MainPage.SaveData();
        await Navigation.PopModalAsync();
    }

    private async void CancelClicked(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
