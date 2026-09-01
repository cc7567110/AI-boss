using AIBoss.Models;

namespace AIBoss.Pages;

public partial class WorkPage : ContentPage
{
    private IDispatcherTimer? _timer;
    private bool _showingEndSheet;

    public WorkPage()
    {
        InitializeComponent();
    }

    private IDispatcherTimer Timer
    {
        get
        {
            if (_timer is null)
            {
                _timer = Dispatcher.CreateTimer();
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Tick += TimerTick;
            }
            return _timer;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshTaskDisplay();
        RefreshSessionDisplay();
    }

    public void ResumeTimer()
    {
        if (MainPage.Data.ActiveSession is { IsPaused: false })
        {
            Timer.Start();
        }
    }

    private void TimerTick(object? sender, EventArgs e) => RefreshSessionDisplay();

    private async void EditTaskClicked(object? sender, EventArgs e)
    {
        if (MainPage.Data.ActiveSession is not null)
        {
            await DisplayAlert("当前正在工作", "工作轮次进行中时不能修改当前任务。请先结束本轮。", "确定");
            return;
        }
        await Navigation.PushAsync(new TaskEditPage());
    }

    private void StartClicked(object? sender, EventArgs e)
    {
        var data = MainPage.Data;
        if (data.ActiveSession is not null)
        {
            DisplayAlert("轮次正在进行", "当前已有一个工作轮次，请继续、暂停或结束它。", "确定");
            return;
        }

        if (MainPage.RuleIsEnabled("require-task") &&
            (string.IsNullOrWhiteSpace(data.CurrentTask.Title) || string.IsNullOrWhiteSpace(data.CurrentTask.Goal)))
        {
            DisplayAlert("先确定任务", "根据当前 Boss 规则，请先填写当前任务和任务目标。", "确定");
            return;
        }

        data.ActiveSession = new ActiveSessionState
        {
            PlannedMinutes = SelectedMinutes(),
            StartedAt = DateTime.Now
        };
        MainPage.SaveData();
        Timer.Start();
        RefreshSessionDisplay();
    }

    private void PauseClicked(object? sender, EventArgs e)
    {
        var active = MainPage.Data.ActiveSession;
        if (active is null)
        {
            DisplayAlert("无法暂停", "还没有开始工作轮次。", "确定");
            return;
        }

        if (active.IsPaused)
        {
            if (active.PausedAt is not null)
            {
                active.PausedSeconds += Math.Max(0, (int)(DateTime.Now - active.PausedAt.Value).TotalSeconds);
            }
            active.IsPaused = false;
            active.PausedAt = null;
            Timer.Start();
        }
        else
        {
            active.IsPaused = true;
            active.PausedAt = DateTime.Now;
            Timer.Stop();
        }

        MainPage.SaveData();
        RefreshSessionDisplay();
    }

    private async void EndClicked(object? sender, EventArgs e)
    {
        var active = MainPage.Data.ActiveSession;
        if (active is null)
        {
            await DisplayAlert("无法结束", "还没有开始工作轮次。", "确定");
            return;
        }

        var wasAlreadyPaused = active.IsPaused;
        if (!wasAlreadyPaused)
        {
            active.IsPaused = true;
            active.PausedAt = DateTime.Now;
            Timer.Stop();
        }

        if (_showingEndSheet) return;
        _showingEndSheet = true;

        try
        {
            var requireOutput = MainPage.RuleIsEnabled("require-output");
            var actualOutput = await ShowEndSessionSheet(
                MainPage.Data.CurrentTask.Title,
                MainPage.Data.CurrentTask.Goal,
                requireOutput);

            if (actualOutput is null)
            {
                if (!wasAlreadyPaused && active.PausedAt is not null)
                {
                    active.PausedSeconds += Math.Max(0, (int)(DateTime.Now - active.PausedAt.Value).TotalSeconds);
                    active.IsPaused = false;
                    active.PausedAt = null;
                    Timer.Start();
                }
                MainPage.SaveData();
                RefreshSessionDisplay();
                return;
            }

            var endedAt = DateTime.Now;
            var output = string.IsNullOrWhiteSpace(actualOutput) ? "（未填写）" : actualOutput;
            MainPage.Data.WorkSessions.Add(new WorkSession
            {
                TaskTitle = string.IsNullOrWhiteSpace(MainPage.Data.CurrentTask.Title) ? "未命名任务" : MainPage.Data.CurrentTask.Title,
                PlannedOutput = string.IsNullOrWhiteSpace(MainPage.Data.CurrentTask.Goal) ? "未填写" : MainPage.Data.CurrentTask.Goal,
                ActualOutput = output,
                PlannedMinutes = active.PlannedMinutes,
                WorkedSeconds = WorkedSeconds(active, endedAt),
                StartedAt = active.StartedAt,
                EndedAt = endedAt
            });
            MainPage.Data.ActiveSession = null;
            Timer.Stop();
            MainPage.SaveData();
            RefreshTaskDisplay();
            RefreshSessionDisplay();
            await DisplayAlert("记录成功", "本轮已记录到每日日志。", "确定");
        }
        finally
        {
            _showingEndSheet = false;
        }
    }

    private async Task<string?> ShowEndSessionSheet(string taskTitle, string taskGoal, bool requireOutput)
    {
        var page = new SessionEndPage(taskTitle, taskGoal, requireOutput);
        await Navigation.PushModalAsync(new NavigationPage(page));
        return await page.TaskCompletionSource.Task;
    }

    internal void RefreshTaskDisplay()
    {
        var task = MainPage.Data.CurrentTask;
        TaskTitleLabel.Text = string.IsNullOrWhiteSpace(task.Title) ? "尚未设置" : task.Title;
        TaskGoalLabel.Text = string.IsNullOrWhiteSpace(task.Goal) ? "尚未设置" : task.Goal;
        TaskStatusLabel.Text = string.IsNullOrWhiteSpace(task.Status) ? "进行中" : task.Status;
    }

    internal void RefreshSessionDisplay()
    {
        var active = MainPage.Data.ActiveSession;
        if (active is null)
        {
            var planned = SelectedMinutes();
            TimerLabel.Text = $"00:00 / {planned}:00";
            SessionStatusLabel.Text = "尚未开始。请选择时长后点击「开始」。";
            StartButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
            EndButton.IsEnabled = false;
            Minutes30.IsEnabled = true;
            Minutes40.IsEnabled = true;
            Minutes45.IsEnabled = true;
            PauseButton.Text = "暂停";
            return;
        }

        var worked = WorkedSeconds(active, DateTime.Now);
        var plannedSeconds = active.PlannedMinutes * 60;
        TimerLabel.Text = $"{FormatSeconds(worked)} / {FormatSeconds(plannedSeconds)}";
        StartButton.IsEnabled = false;
        PauseButton.IsEnabled = true;
        EndButton.IsEnabled = true;
        Minutes30.IsEnabled = false;
        Minutes40.IsEnabled = false;
        Minutes45.IsEnabled = false;
        PauseButton.Text = active.IsPaused ? "继续" : "暂停";
        SessionStatusLabel.Text = active.IsPaused
            ? "本轮已暂停。点击「继续」后继续计时。"
            : worked >= plannedSeconds
                ? "计划时长已到，请点击「结束」并记录实际产出。"
                : $"正在工作中：本轮计划 {active.PlannedMinutes} 分钟。";
    }

    private int SelectedMinutes() => Minutes30.IsChecked ? 30 : Minutes45.IsChecked ? 45 : 40;

    private static int WorkedSeconds(ActiveSessionState active, DateTime now)
    {
        var end = active.IsPaused && active.PausedAt is not null ? active.PausedAt.Value : now;
        return Math.Max(0, (int)(end - active.StartedAt).TotalSeconds - active.PausedSeconds);
    }

    private static string FormatSeconds(int seconds) => $"{seconds / 60:00}:{seconds % 60:00}";
}
