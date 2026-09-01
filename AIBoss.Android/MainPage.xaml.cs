using AIBoss.Models;
using AIBoss.Services;

namespace AIBoss;

public partial class MainPage : TabbedPage
{
    public static AppData Data { get; private set; } = new();
    public static LocalDataService DataService { get; } = new();

    public Pages.WorkPage WorkPageInstance { get; private set; } = null!;

    public MainPage()
    {
        InitializeComponent();
        Data = DataService.Load();

        WorkPageInstance = WorkPage;

        if (Data.ActiveSession is { IsPaused: false })
        {
            WorkPage.ResumeTimer();
        }
    }

    public static void SaveData() => DataService.Save(Data);

    public static bool RuleIsEnabled(string code) =>
        Data.BossRules.FirstOrDefault(r => r.Code == code)?.IsEnabled ?? true;
}
