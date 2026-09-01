namespace AIBoss.Pages;

public partial class SessionEndPage : ContentPage
{
    public TaskCompletionSource<string?> TaskCompletionSource { get; } = new();
    private readonly bool _requireOutput;

    public SessionEndPage(string taskTitle, string taskGoal, bool requireOutput)
    {
        InitializeComponent();
        _requireOutput = requireOutput;
        TaskTitleLabel.Text = string.IsNullOrWhiteSpace(taskTitle) ? "未命名任务" : taskTitle;
        PlannedOutputLabel.Text = string.IsNullOrWhiteSpace(taskGoal) ? "未填写" : taskGoal;
    }

    private async void FinishClicked(object? sender, EventArgs e)
    {
        var output = ActualOutputEditor.Text?.Trim();
        if (_requireOutput && string.IsNullOrWhiteSpace(output))
        {
            await DisplayAlert("需要填写", "根据 Boss 规则，必须填写实际产出才能结束本轮。", "确定");
            return;
        }

        TaskCompletionSource.TrySetResult(output ?? string.Empty);
        await Navigation.PopModalAsync();
    }

    private async void CancelClicked(object? sender, EventArgs e)
    {
        TaskCompletionSource.TrySetResult(null);
        await Navigation.PopModalAsync();
    }
}
