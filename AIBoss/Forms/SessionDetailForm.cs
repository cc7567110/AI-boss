using AIBoss.Models;

namespace AIBoss.Forms;

public sealed class SessionDetailForm : Form
{
    private readonly TextBox _taskBox = new();
    private readonly TextBox _plannedBox = new();
    private readonly TextBox _actualBox = new();
    private bool _deleted;

    public WorkSession EditedSession { get; }
    public bool IsDeleted => _deleted;

    public SessionDetailForm(WorkSession session)
    {
        EditedSession = new WorkSession
        {
            Id = session.Id,
            TaskTitle = session.TaskTitle,
            PlannedOutput = session.PlannedOutput,
            ActualOutput = session.ActualOutput,
            PlannedMinutes = session.PlannedMinutes,
            WorkedSeconds = session.WorkedSeconds,
            StartedAt = session.StartedAt,
            EndedAt = session.EndedAt
        };

        Text = "学习日志详情";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(680, 600);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18),
            ColumnCount = 1,
            RowCount = 9
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var minutes = session.WorkedSeconds / 60;
        var seconds = session.WorkedSeconds % 60;
        layout.Controls.Add(new Label
        {
            Text = $"记录时间：{session.StartedAt:yyyy-MM-dd HH:mm} ～ {session.EndedAt:yyyy-MM-dd HH:mm}（实际时长 {minutes} 分 {seconds} 秒）",
            AutoSize = true
        }, 0, 0);

        layout.Controls.Add(new Label { Text = "任务名称", AutoSize = true, Margin = new Padding(3, 14, 3, 3) }, 0, 1);
        _taskBox.Dock = DockStyle.Fill;
        _taskBox.Text = session.TaskTitle;
        layout.Controls.Add(_taskBox, 0, 2);

        layout.Controls.Add(new Label { Text = "计划产出（来自当时任务目标）", AutoSize = true, Margin = new Padding(3, 14, 3, 3) }, 0, 3);
        _plannedBox.Multiline = true;
        _plannedBox.ScrollBars = ScrollBars.Vertical;
        _plannedBox.Dock = DockStyle.Fill;
        _plannedBox.Text = session.PlannedOutput;
        layout.Controls.Add(_plannedBox, 0, 4);

        layout.Controls.Add(new Label { Text = "实际产出", AutoSize = true, Margin = new Padding(3, 14, 3, 3) }, 0, 5);
        _actualBox.Multiline = true;
        _actualBox.ScrollBars = ScrollBars.Vertical;
        _actualBox.Dock = DockStyle.Fill;
        _actualBox.Text = session.ActualOutput;
        layout.Controls.Add(_actualBox, 0, 6);

        var deleteButton = new Button { Text = "删除这条记录", Width = 120, Margin = new Padding(3, 16, 3, 3) };
        deleteButton.Click += DeleteButton_Click;
        layout.Controls.Add(deleteButton, 0, 7);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, AutoSize = true, Margin = new Padding(0, 14, 0, 0) };
        var saveButton = new Button { Text = "保存修改", Width = 100 };
        saveButton.Click += SaveButton_Click;
        buttons.Controls.Add(saveButton);
        buttons.Controls.Add(new Button { Text = "取消", DialogResult = DialogResult.Cancel, Width = 100 });
        layout.Controls.Add(buttons, 0, 8);

        Controls.Add(layout);
        AcceptButton = saveButton;
        CancelButton = buttons.Controls.OfType<Button>().First(button => button.Text == "取消");
        _taskBox.Select();
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        EditedSession.TaskTitle = string.IsNullOrWhiteSpace(_taskBox.Text) ? "未命名任务" : _taskBox.Text.Trim();
        EditedSession.PlannedOutput = string.IsNullOrWhiteSpace(_plannedBox.Text) ? "未填写" : _plannedBox.Text.Trim();
        EditedSession.ActualOutput = string.IsNullOrWhiteSpace(_actualBox.Text) ? "（未填写）" : _actualBox.Text.Trim();
        DialogResult = DialogResult.OK;
        Close();
    }

    private void DeleteButton_Click(object? sender, EventArgs e)
    {
        var confirm = MessageBox.Show("确定要删除这条学习记录吗？删除后无法恢复。", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes)
        {
            return;
        }

        _deleted = true;
        DialogResult = DialogResult.OK;
        Close();
    }
}
