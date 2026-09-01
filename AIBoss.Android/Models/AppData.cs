namespace AIBoss.Models;

public sealed class AppData
{
    public CurrentTask CurrentTask { get; set; } = new();
    public List<WorkSession> WorkSessions { get; set; } = [];
    public List<IdeaItem> Ideas { get; set; } = [];
    public List<BossRule> BossRules { get; set; } = BossRule.CreateDefaults();
    public ActiveSessionState? ActiveSession { get; set; }

    public void Normalize()
    {
        CurrentTask ??= new CurrentTask();
        WorkSessions ??= [];
        Ideas ??= [];
        BossRules ??= [];
        if (BossRules.Count == 0)
        {
            BossRules = BossRule.CreateDefaults();
        }
    }
}

public sealed class CurrentTask
{
    public string Title { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public string Status { get; set; } = "进行中";
}

public sealed class WorkSession
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string TaskTitle { get; set; } = string.Empty;
    public string PlannedOutput { get; set; } = string.Empty;
    public string ActualOutput { get; set; } = string.Empty;
    public int PlannedMinutes { get; set; }
    public int WorkedSeconds { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
}

public sealed class ActiveSessionState
{
    public int PlannedMinutes { get; set; }
    public DateTime StartedAt { get; set; }
    public int PausedSeconds { get; set; }
    public bool IsPaused { get; set; }
    public DateTime? PausedAt { get; set; }
}

public sealed class IdeaItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public override string ToString()
    {
        var singleLine = string.IsNullOrWhiteSpace(Content)
            ? string.Empty
            : Content.Replace("\r", " ").Replace("\n", " ").Trim();
        if (singleLine.Length > 40)
        {
            singleLine = singleLine[..40] + "…";
        }
        return $"{CreatedAt:yyyy-MM-dd HH:mm}  |  {singleLine}";
    }
}

public sealed class BossRule
{
    public string Code { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;

    public static List<BossRule> CreateDefaults() =>
    [
        new BossRule
        {
            Code = "require-task",
            Content = "开始工作轮次前，必须填写当前任务和任务目标。",
            IsEnabled = true
        },
        new BossRule
        {
            Code = "require-output",
            Content = "结束工作轮次时，必须填写实际产出。",
            IsEnabled = true
        },
        new BossRule
        {
            Code = "one-session",
            Content = "同一时间只能进行一个工作轮次。",
            IsEnabled = true
        }
    ];
}
