using AIBoss.Forms;
using AIBoss.Models;
using AIBoss.Services;

namespace AIBoss;

public sealed class MainForm : Form
{
    private readonly LocalDataService _dataService = new();
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 1000 };
    private AppData _data = new();

    private readonly Label _taskTitleLabel = new();
    private readonly Label _taskGoalLabel = new();
    private readonly Label _taskStatusLabel = new();
    private readonly Label _timerLabel = new();
    private readonly Label _sessionStatusLabel = new();
    private readonly RadioButton _minutes30 = new() { Text = "30 分钟", AutoSize = true };
    private readonly RadioButton _minutes40 = new() { Text = "40 分钟", AutoSize = true, Checked = true };
    private readonly RadioButton _minutes45 = new() { Text = "45 分钟", AutoSize = true };
    private readonly Button _startButton = new() { Text = "开始", Width = 100, Height = 36 };
    private readonly Button _pauseButton = new() { Text = "暂停", Width = 100, Height = 36 };
    private readonly Button _endButton = new() { Text = "结束", Width = 100, Height = 36 };
    private readonly TextBox _ideaBox = new() { Multiline = true, Height = 100 };
    private readonly ListBox _ideaList = new();
    private readonly DataGridView _logGrid = new();
    private readonly DataGridView _rulesGrid = new();

    public MainForm()
    {
        Text = "AI Boss V0.2 - 学习执行助手";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 680);
        Size = new Size(980, 760);
        Font = new Font("Microsoft YaHei UI", 10);

        BuildInterface();
        _data = _dataService.Load();
        _timer.Tick += Timer_Tick;
        FormClosing += MainForm_FormClosing;

        RefreshAll();
        if (_data.ActiveSession is { IsPaused: false })
        {
            _timer.Start();
        }
    }

    private void BuildInterface()
    {
        var exportButton = new Button { Text = "导出数据", Dock = DockStyle.Right, Width = 120 };
        exportButton.Click += ExportButton_Click;
        var dataPathButton = new Button { Text = "打开数据文件夹", Dock = DockStyle.Right, Width = 140 };
        dataPathButton.Click += DataPathButton_Click;
        var titleLabel = new Label { Text = "AI Boss V0.2（离线本地版）", Dock = DockStyle.Left, AutoSize = true, Font = new Font("Microsoft YaHei UI", 12, FontStyle.Bold), Padding = new Padding(12, 14, 0, 0) };
        var header = new Panel { Dock = DockStyle.Top, Height = 54 };
        header.Controls.Add(exportButton);
        header.Controls.Add(dataPathButton);
        header.Controls.Add(titleLabel);

        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(BuildWorkTab());
        tabs.TabPages.Add(BuildIdeaTab());
        tabs.TabPages.Add(BuildLogTab());
        tabs.TabPages.Add(BuildRulesTab());

        Controls.Add(tabs);
        Controls.Add(header);
    }

    private TabPage BuildWorkTab()
    {
        var tab = new TabPage("今日执行") { Padding = new Padding(14) };
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };

        var taskGroup = new GroupBox { Text = "当前任务", Width = 870, Height = 175 };
        var taskTable = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 2, RowCount = 4 };
        taskTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92));
        taskTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        taskTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        taskTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        taskTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        taskTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        taskTable.Controls.Add(new Label { Text = "任务名称：", AutoSize = true }, 0, 0);
        _taskTitleLabel.AutoSize = true;
        _taskTitleLabel.Font = new Font("Microsoft YaHei UI", 10, FontStyle.Bold);
        taskTable.Controls.Add(_taskTitleLabel, 1, 0);
        taskTable.Controls.Add(new Label { Text = "任务目标：", AutoSize = true, Margin = new Padding(3, 12, 3, 3) }, 0, 1);
        _taskGoalLabel.AutoSize = true;
        _taskGoalLabel.MaximumSize = new Size(710, 0);
        _taskGoalLabel.Margin = new Padding(3, 12, 3, 3);
        taskTable.Controls.Add(_taskGoalLabel, 1, 1);
        taskTable.Controls.Add(new Label { Text = "任务状态：", AutoSize = true }, 0, 2);
        _taskStatusLabel.AutoSize = true;
        taskTable.Controls.Add(_taskStatusLabel, 1, 2);
        var editTaskButton = new Button { Text = "编辑当前任务", Width = 125 };
        editTaskButton.Click += EditTaskButton_Click;
        taskTable.Controls.Add(editTaskButton, 1, 3);
        taskGroup.Controls.Add(taskTable);

        var sessionGroup = new GroupBox { Text = "当前工作轮次", Width = 870, Height = 300, Margin = new Padding(3, 14, 3, 3) };
        var sessionPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(18), FlowDirection = FlowDirection.TopDown, WrapContents = false };
        var durationPanel = new FlowLayoutPanel { AutoSize = true };
        durationPanel.Controls.Add(new Label { Text = "计划时长：", AutoSize = true, Margin = new Padding(3, 7, 8, 3) });
        durationPanel.Controls.Add(_minutes30);
        durationPanel.Controls.Add(_minutes40);
        durationPanel.Controls.Add(_minutes45);
        _timerLabel.Text = "00:00 / 40:00";
        _timerLabel.AutoSize = true;
        _timerLabel.Font = new Font("Microsoft YaHei UI", 28, FontStyle.Bold);
        _sessionStatusLabel.AutoSize = true;
        _sessionStatusLabel.MaximumSize = new Size(790, 0);
        var actionPanel = new FlowLayoutPanel { AutoSize = true, Margin = new Padding(0, 8, 0, 0) };
        _startButton.Click += StartButton_Click;
        _pauseButton.Click += PauseButton_Click;
        _endButton.Click += EndButton_Click;
        actionPanel.Controls.AddRange([_startButton, _pauseButton, _endButton]);
        sessionPanel.Controls.Add(durationPanel);
        sessionPanel.Controls.Add(_timerLabel);
        sessionPanel.Controls.Add(_sessionStatusLabel);
        sessionPanel.Controls.Add(actionPanel);
        sessionGroup.Controls.Add(sessionPanel);

        var reminder = new Label
        {
            Text = "使用顺序：填写当前任务 → 选择时长 → 开始 → 完成后点击结束并记录实际产出。",
            AutoSize = true,
            MaximumSize = new Size(850, 0),
            Margin = new Padding(3, 18, 3, 3)
        };
        panel.Controls.Add(taskGroup);
        panel.Controls.Add(sessionGroup);
        panel.Controls.Add(reminder);
        tab.Controls.Add(panel);
        return tab;
    }

    private TabPage BuildIdeaTab()
    {
        var tab = new TabPage("IDEA BOX") { Padding = new Padding(14) };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5 };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(new Label { Text = "想到与当前任务无关的事，快速写在这里。保存后即可继续专注。双击列表中的想法可查看、编辑或删除全文。", AutoSize = true }, 0, 0);
        _ideaBox.Dock = DockStyle.Fill;
        _ideaBox.Margin = new Padding(3, 10, 3, 8);
        layout.Controls.Add(_ideaBox, 0, 1);
        var buttons = new FlowLayoutPanel { AutoSize = true };
        var saveIdeaButton = new Button { Text = "保存想法", Width = 110 };
        saveIdeaButton.Click += SaveIdeaButton_Click;
        buttons.Controls.Add(saveIdeaButton);
        var deleteIdeaButton = new Button { Text = "删除想法", Width = 110, Margin = new Padding(8, 0, 0, 0) };
        deleteIdeaButton.Click += DeleteIdeaButton_Click;
        buttons.Controls.Add(deleteIdeaButton);
        layout.Controls.Add(buttons, 0, 2);
        _ideaList.Dock = DockStyle.Fill;
        _ideaList.Margin = new Padding(3, 14, 3, 3);
        _ideaList.DoubleClick += IdeaList_DoubleClick;
        layout.Controls.Add(_ideaList, 0, 3);
        layout.Controls.Add(new Label { Text = "想法会自动保存到本机，不会影响正在进行的计时。", AutoSize = true, Margin = new Padding(3, 8, 3, 3) }, 0, 4);
        tab.Controls.Add(layout);
        return tab;
    }

    private TabPage BuildLogTab()
    {
        var tab = new TabPage("每日学习日志") { Padding = new Padding(14) };
        var refreshButton = new Button { Text = "刷新记录", Dock = DockStyle.Top, Height = 34 };
        refreshButton.Click += (_, _) => RefreshLogGrid();
        var hint = new Label
        {
            Text = "双击任意记录可查看完整内容、编辑或删除。列表仅显示摘要，长文本请在详情窗口中查看。",
            Dock = DockStyle.Top,
            Height = 34,
            Padding = new Padding(0, 8, 0, 4)
        };
        var deleteLogButton = new Button { Text = "删除选中记录", Dock = DockStyle.Bottom, Height = 38 };
        deleteLogButton.Click += DeleteLogButton_Click;
        _logGrid.Dock = DockStyle.Fill;
        _logGrid.ReadOnly = true;
        _logGrid.AllowUserToAddRows = false;
        _logGrid.AllowUserToDeleteRows = false;
        _logGrid.RowHeadersVisible = false;
        _logGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _logGrid.AutoGenerateColumns = false;
        _logGrid.CellDoubleClick += LogGrid_CellDoubleClick;
        _logGrid.Columns.AddRange(
            CreateLogColumn("日期", nameof(LogRow.Date), 85),
            CreateLogColumn("任务", nameof(LogRow.Task), 120),
            CreateLogColumn("计划产出", nameof(LogRow.PlannedOutput), 120),
            CreateLogColumn("实际产出", nameof(LogRow.ActualOutput), 120),
            CreateLogColumn("开始时间", nameof(LogRow.StartedAt), 115),
            CreateLogColumn("结束时间", nameof(LogRow.EndedAt), 115),
            CreateLogColumn("实际时长", nameof(LogRow.Duration), 70));
        tab.Controls.Add(_logGrid);
        tab.Controls.Add(deleteLogButton);
        tab.Controls.Add(refreshButton);
        tab.Controls.Add(hint);
        return tab;
    }

    private TabPage BuildRulesTab()
    {
        var tab = new TabPage("Boss 规则") { Padding = new Padding(14) };
        var instructions = new Label
        {
            Text = "以下是 V0.1 的固定监督规则。你可以修改文字或取消勾选，但程序不会自动新增或改写规则。",
            Dock = DockStyle.Top,
            Height = 42,
            Padding = new Padding(0, 0, 0, 8)
        };
        var saveRulesButton = new Button { Text = "保存规则", Dock = DockStyle.Bottom, Height = 38 };
        saveRulesButton.Click += SaveRulesButton_Click;
        _rulesGrid.Dock = DockStyle.Fill;
        _rulesGrid.AutoGenerateColumns = false;
        _rulesGrid.AllowUserToAddRows = false;
        _rulesGrid.AllowUserToDeleteRows = false;
        _rulesGrid.RowHeadersVisible = false;
        _rulesGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _rulesGrid.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "启用", DataPropertyName = nameof(BossRule.IsEnabled), FillWeight = 18 });
        _rulesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "规则内容", DataPropertyName = nameof(BossRule.Content), FillWeight = 82 });
        tab.Controls.Add(_rulesGrid);
        tab.Controls.Add(saveRulesButton);
        tab.Controls.Add(instructions);
        return tab;
    }

    private static DataGridViewTextBoxColumn CreateLogColumn(string header, string propertyName, float width) =>
        new() { HeaderText = header, DataPropertyName = propertyName, FillWeight = width };

    private void EditTaskButton_Click(object? sender, EventArgs e)
    {
        if (_data.ActiveSession is not null)
        {
            MessageBox.Show("工作轮次进行中时不能修改当前任务。请先结束本轮。", "当前正在工作", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var form = new TaskForm(_data.CurrentTask);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            _data.CurrentTask = form.Task;
            SaveData();
            RefreshTaskDisplay();
        }
    }

    private void StartButton_Click(object? sender, EventArgs e)
    {
        if (_data.ActiveSession is not null)
        {
            MessageBox.Show("当前已有一个工作轮次，请继续、暂停或结束它。", "轮次正在进行", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (RuleIsEnabled("require-task") &&
            (string.IsNullOrWhiteSpace(_data.CurrentTask.Title) || string.IsNullOrWhiteSpace(_data.CurrentTask.Goal)))
        {
            MessageBox.Show("根据当前 Boss 规则，请先填写当前任务和任务目标。", "先确定任务", MessageBoxButtons.OK, MessageBoxIcon.Information);
            EditTaskButton_Click(sender, e);
            return;
        }

        _data.ActiveSession = new ActiveSessionState
        {
            PlannedMinutes = SelectedMinutes(),
            StartedAt = DateTime.Now
        };
        SaveData();
        _timer.Start();
        RefreshSessionDisplay();
    }

    private void PauseButton_Click(object? sender, EventArgs e)
    {
        var active = _data.ActiveSession;
        if (active is null)
        {
            MessageBox.Show("还没有开始工作轮次。", "无法暂停", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            _timer.Start();
        }
        else
        {
            PauseActiveSession();
        }

        SaveData();
        RefreshSessionDisplay();
    }

    private void EndButton_Click(object? sender, EventArgs e)
    {
        var active = _data.ActiveSession;
        if (active is null)
        {
            MessageBox.Show("还没有开始工作轮次。", "无法结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // 记录产出时先暂停，避免填写文字的时间被算作实际工作时间。
        var wasAlreadyPaused = active.IsPaused;
        if (!wasAlreadyPaused)
        {
            PauseActiveSession();
        }

        using var form = new SessionResultForm(_data.CurrentTask.Title, _data.CurrentTask.Goal, RuleIsEnabled("require-output"));
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            if (!wasAlreadyPaused && active.PausedAt is not null)
            {
                active.PausedSeconds += Math.Max(0, (int)(DateTime.Now - active.PausedAt.Value).TotalSeconds);
                active.IsPaused = false;
                active.PausedAt = null;
                _timer.Start();
            }
            SaveData();
            RefreshSessionDisplay();
            return;
        }

        var endedAt = DateTime.Now;
        var actualOutput = string.IsNullOrWhiteSpace(form.ActualOutput) ? "（未填写）" : form.ActualOutput;
        _data.WorkSessions.Add(new WorkSession
        {
            TaskTitle = string.IsNullOrWhiteSpace(_data.CurrentTask.Title) ? "未命名任务" : _data.CurrentTask.Title,
            PlannedOutput = string.IsNullOrWhiteSpace(_data.CurrentTask.Goal) ? "未填写" : _data.CurrentTask.Goal,
            ActualOutput = actualOutput,
            PlannedMinutes = active.PlannedMinutes,
            WorkedSeconds = WorkedSeconds(active, endedAt),
            StartedAt = active.StartedAt,
            EndedAt = endedAt
        });
        _data.ActiveSession = null;
        _timer.Stop();
        SaveData();
        RefreshAll();
        MessageBox.Show("本轮已记录到每日日志。", "记录成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void SaveIdeaButton_Click(object? sender, EventArgs e)
    {
        var content = _ideaBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(content))
        {
            MessageBox.Show("请先写下想法。", "IDEA BOX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _ideaBox.Focus();
            return;
        }

        _data.Ideas.Add(new IdeaItem { Content = content, CreatedAt = DateTime.Now });
        _ideaBox.Clear();
        SaveData();
        RefreshIdeaList();
        _ideaBox.Focus();
    }

    private void IdeaList_DoubleClick(object? sender, EventArgs e)
    {
        if (_ideaList.SelectedItem is not IdeaItem idea)
        {
            return;
        }
        OpenIdeaDetail(idea);
    }

    private void OpenIdeaDetail(IdeaItem idea)
    {
        using var form = new IdeaDetailForm(idea);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (form.IsDeleted)
        {
            _data.Ideas.Remove(idea);
        }
        else
        {
            idea.Content = form.EditedContent;
        }
        SaveData();
        RefreshIdeaList();
    }

    private void DeleteIdeaButton_Click(object? sender, EventArgs e)
    {
        if (_ideaList.SelectedItem is not IdeaItem idea)
        {
            MessageBox.Show("请先在列表中选择一条想法。", "IDEA BOX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirm = MessageBox.Show("确定要删除这条想法吗？删除后无法恢复。", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes)
        {
            return;
        }

        _data.Ideas.Remove(idea);
        SaveData();
        RefreshIdeaList();
    }

    private void SaveRulesButton_Click(object? sender, EventArgs e)
    {
        _rulesGrid.EndEdit();
        SaveData();
        MessageBox.Show("Boss 规则已保存。", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ExportButton_Click(object? sender, EventArgs e)
    {
        SaveData();
        using var dialog = new SaveFileDialog
        {
            Title = "导出 AI Boss 全部数据",
            Filter = "JSON 数据文件 (*.json)|*.json",
            FileName = $"AI-Boss-备份-{DateTime.Now:yyyyMMdd-HHmmss}.json"
        };
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        File.WriteAllText(dialog.FileName, _dataService.Serialize(_data));
        MessageBox.Show("全部数据已导出为 JSON 文件。", "导出成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void DataPathButton_Click(object? sender, EventArgs e)
    {
        Directory.CreateDirectory(_dataService.DataDirectory);
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = _dataService.DataDirectory,
            UseShellExecute = true
        });
    }

    private void Timer_Tick(object? sender, EventArgs e) => RefreshSessionDisplay();

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_data.ActiveSession is { IsPaused: false })
        {
            PauseActiveSession();
        }
        SaveData();
    }

    private void PauseActiveSession()
    {
        var active = _data.ActiveSession;
        if (active is null || active.IsPaused)
        {
            return;
        }
        active.IsPaused = true;
        active.PausedAt = DateTime.Now;
        _timer.Stop();
    }

    private void RefreshAll()
    {
        RefreshTaskDisplay();
        RefreshSessionDisplay();
        RefreshIdeaList();
        RefreshLogGrid();
        _rulesGrid.DataSource = _data.BossRules;
    }

    private void RefreshTaskDisplay()
    {
        var task = _data.CurrentTask;
        _taskTitleLabel.Text = string.IsNullOrWhiteSpace(task.Title) ? "尚未设置" : task.Title;
        _taskGoalLabel.Text = string.IsNullOrWhiteSpace(task.Goal) ? "尚未设置" : task.Goal;
        _taskStatusLabel.Text = string.IsNullOrWhiteSpace(task.Status) ? "进行中" : task.Status;
    }

    private void RefreshSessionDisplay()
    {
        var active = _data.ActiveSession;
        if (active is null)
        {
            var planned = SelectedMinutes();
            _timerLabel.Text = $"00:00 / {planned}:00";
            _sessionStatusLabel.Text = "尚未开始。请选择时长后点击“开始”。";
            _startButton.Enabled = true;
            _pauseButton.Enabled = false;
            _endButton.Enabled = false;
            _minutes30.Enabled = _minutes40.Enabled = _minutes45.Enabled = true;
            _pauseButton.Text = "暂停";
            return;
        }

        var worked = WorkedSeconds(active, DateTime.Now);
        var plannedSeconds = active.PlannedMinutes * 60;
        _timerLabel.Text = $"{FormatSeconds(worked)} / {FormatSeconds(plannedSeconds)}";
        _startButton.Enabled = false;
        _pauseButton.Enabled = true;
        _endButton.Enabled = true;
        _minutes30.Enabled = _minutes40.Enabled = _minutes45.Enabled = false;
        _pauseButton.Text = active.IsPaused ? "继续" : "暂停";
        _sessionStatusLabel.Text = active.IsPaused
            ? "本轮已暂停。点击“继续”后继续计时。"
            : worked >= plannedSeconds
                ? "计划时长已到，请点击“结束”并记录实际产出。"
                : $"正在工作中：本轮计划 {active.PlannedMinutes} 分钟。";
    }

    private void RefreshIdeaList()
    {
        _ideaList.BeginUpdate();
        _ideaList.Items.Clear();
        foreach (var idea in _data.Ideas.OrderByDescending(item => item.CreatedAt))
        {
            _ideaList.Items.Add(idea);
        }
        _ideaList.EndUpdate();
    }

    private void RefreshLogGrid()
    {
        _logGrid.DataSource = _data.WorkSessions
            .OrderByDescending(session => session.StartedAt)
            .Select(session => new LogRow
            {
                Id = session.Id,
                Date = session.StartedAt.ToString("yyyy-MM-dd"),
                Task = Summarize(session.TaskTitle, 24, "未命名任务"),
                PlannedOutput = Summarize(session.PlannedOutput, 24, "未填写"),
                ActualOutput = Summarize(session.ActualOutput, 24, "（未填写）"),
                StartedAt = session.StartedAt.ToString("yyyy-MM-dd HH:mm"),
                EndedAt = session.EndedAt.ToString("yyyy-MM-dd HH:mm"),
                Duration = FormatSeconds(session.WorkedSeconds)
            })
            .ToList();
    }

    private void LogGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _logGrid.Rows.Count)
        {
            return;
        }
        if (_logGrid.Rows[e.RowIndex].DataBoundItem is not LogRow row)
        {
            return;
        }

        var session = _data.WorkSessions.FirstOrDefault(item => item.Id == row.Id);
        if (session is null)
        {
            return;
        }
        OpenSessionDetail(session);
    }

    private void OpenSessionDetail(WorkSession session)
    {
        using var form = new SessionDetailForm(session);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (form.IsDeleted)
        {
            _data.WorkSessions.Remove(session);
        }
        else
        {
            var edited = form.EditedSession;
            session.TaskTitle = edited.TaskTitle;
            session.PlannedOutput = edited.PlannedOutput;
            session.ActualOutput = edited.ActualOutput;
        }
        SaveData();
        RefreshLogGrid();
    }

    private void DeleteLogButton_Click(object? sender, EventArgs e)
    {
        if (_logGrid.CurrentRow?.DataBoundItem is not LogRow row)
        {
            MessageBox.Show("请先在列表中选择一条记录。", "每日学习日志", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var session = _data.WorkSessions.FirstOrDefault(item => item.Id == row.Id);
        if (session is null)
        {
            return;
        }

        var confirm = MessageBox.Show($"确定要删除这条学习记录吗？\n\n任务：{row.Task}\n日期：{row.Date}\n\n删除后无法恢复。", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes)
        {
            return;
        }

        _data.WorkSessions.Remove(session);
        SaveData();
        RefreshLogGrid();
    }

    private static string Summarize(string text, int maxLength, string emptyText)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return emptyText;
        }

        var singleLine = text.Replace("\r", " ").Replace("\n", " ").Trim();
        return singleLine.Length <= maxLength ? singleLine : singleLine[..maxLength] + "…";
    }

    private int SelectedMinutes() => _minutes30.Checked ? 30 : _minutes45.Checked ? 45 : 40;

    private bool RuleIsEnabled(string code) => _data.BossRules.FirstOrDefault(rule => rule.Code == code)?.IsEnabled ?? true;

    private static int WorkedSeconds(ActiveSessionState active, DateTime now)
    {
        var end = active.IsPaused && active.PausedAt is not null ? active.PausedAt.Value : now;
        return Math.Max(0, (int)(end - active.StartedAt).TotalSeconds - active.PausedSeconds);
    }

    private static string FormatSeconds(int seconds) => $"{seconds / 60:00}:{seconds % 60:00}";

    private void SaveData() => _dataService.Save(_data);

    private sealed class LogRow
    {
        public string Id { get; init; } = string.Empty;
        public string Date { get; init; } = string.Empty;
        public string Task { get; init; } = string.Empty;
        public string PlannedOutput { get; init; } = string.Empty;
        public string ActualOutput { get; init; } = string.Empty;
        public string StartedAt { get; init; } = string.Empty;
        public string EndedAt { get; init; } = string.Empty;
        public string Duration { get; init; } = string.Empty;
    }
}
