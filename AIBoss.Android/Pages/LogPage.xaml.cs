using AIBoss.Models;

namespace AIBoss.Pages;

public partial class LogPage : ContentPage
{
    public LogPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshLogGrid();
    }

    private void RefreshClicked(object? sender, EventArgs e) => RefreshLogGrid();

    private async void DeleteLogClicked(object? sender, EventArgs e)
    {
        if (LogCollection.SelectedItem is not LogViewItem item)
        {
            await DisplayAlert("每日学习日志", "请先在列表中选择一条记录。", "确定");
            return;
        }

        var session = MainPage.Data.WorkSessions.FirstOrDefault(s => s.Id == item.Id);
        if (session is null) return;

        var confirm = await DisplayAlert("确认删除",
            $"确定要删除这条学习记录吗？\n\n任务：{item.TaskTitle}\n日期：{item.Date}\n\n删除后无法恢复。",
            "删除", "取消");
        if (!confirm) return;

        MainPage.Data.WorkSessions.Remove(session);
        MainPage.SaveData();
        RefreshLogGrid();
    }

    private async void LogTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is LogViewItem item)
        {
            var session = MainPage.Data.WorkSessions.FirstOrDefault(s => s.Id == item.Id);
            if (session is null) return;

            var page = new SessionDetailPage(session);
            await Navigation.PushModalAsync(new NavigationPage(page));
            MainPage.SaveData();
            RefreshLogGrid();
        }
    }

    internal void RefreshLogGrid()
    {
        var items = MainPage.Data.WorkSessions
            .OrderByDescending(s => s.StartedAt)
            .Select(s => new LogViewItem
            {
                Id = s.Id,
                TaskTitle = string.IsNullOrWhiteSpace(s.TaskTitle) ? "未命名任务" : s.TaskTitle,
                Date = s.StartedAt.ToString("yyyy-MM-dd"),
                DateRange = $"{s.StartedAt:MM-dd HH:mm} ~ {s.EndedAt:MM-dd HH:mm}",
                ActualOutputSummary = Summarize(s.ActualOutput, 40, "（未填写）"),
                Duration = FormatSeconds(s.WorkedSeconds)
            })
            .ToList();
        LogCollection.ItemsSource = items;
    }

    private static string Summarize(string text, int maxLength, string emptyText)
    {
        if (string.IsNullOrWhiteSpace(text)) return emptyText;
        var singleLine = text.Replace("\r", " ").Replace("\n", " ").Trim();
        return singleLine.Length <= maxLength ? singleLine : singleLine[..maxLength] + "…";
    }

    private static string FormatSeconds(int seconds) => $"{seconds / 60:00}:{seconds % 60:00}";

    private sealed class LogViewItem
    {
        public string Id { get; init; } = string.Empty;
        public string TaskTitle { get; init; } = string.Empty;
        public string Date { get; init; } = string.Empty;
        public string DateRange { get; init; } = string.Empty;
        public string ActualOutputSummary { get; init; } = string.Empty;
        public string Duration { get; init; } = string.Empty;
    }
}
