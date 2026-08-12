using AIBoss.Models;

namespace AIBoss.Forms;

public sealed class TaskForm : Form
{
    private readonly TextBox _titleBox = new();
    private readonly TextBox _goalBox = new();
    private readonly ComboBox _statusBox = new();

    public CurrentTask Task { get; }

    public TaskForm(CurrentTask task)
    {
        Task = new CurrentTask
        {
            Title = task.Title,
            Goal = task.Goal,
            Status = task.Status
        };

        Text = "编辑当前任务";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(560, 330);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18),
            ColumnCount = 1,
            RowCount = 7
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        layout.Controls.Add(new Label { Text = "当前任务名称", AutoSize = true }, 0, 0);
        _titleBox.Dock = DockStyle.Top;
        _titleBox.Text = Task.Title;
        layout.Controls.Add(_titleBox, 0, 1);
        layout.Controls.Add(new Label { Text = "本任务目标（本轮计划产出会使用这里的内容）", AutoSize = true, Margin = new Padding(3, 12, 3, 3) }, 0, 2);
        _goalBox.Multiline = true;
        _goalBox.Dock = DockStyle.Fill;
        _goalBox.Text = Task.Goal;
        layout.Controls.Add(_goalBox, 0, 3);
        layout.Controls.Add(new Label { Text = "任务状态", AutoSize = true, Margin = new Padding(3, 12, 3, 3) }, 0, 4);
        _statusBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _statusBox.Items.AddRange(["进行中", "已完成", "暂缓"]);
        _statusBox.SelectedItem = _statusBox.Items.Contains(Task.Status) ? Task.Status : "进行中";
        layout.Controls.Add(_statusBox, 0, 5);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, AutoSize = true, Margin = new Padding(0, 16, 0, 0) };
        var saveButton = new Button { Text = "保存", DialogResult = DialogResult.None, Width = 90 };
        saveButton.Click += SaveButton_Click;
        buttons.Controls.Add(saveButton);
        buttons.Controls.Add(new Button { Text = "取消", DialogResult = DialogResult.Cancel, Width = 90 });
        layout.Controls.Add(buttons, 0, 6);

        Controls.Add(layout);
        AcceptButton = saveButton;
        CancelButton = buttons.Controls.OfType<Button>().First(button => button.Text == "取消");
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        Task.Title = _titleBox.Text.Trim();
        Task.Goal = _goalBox.Text.Trim();
        Task.Status = _statusBox.SelectedItem?.ToString() ?? "进行中";
        DialogResult = DialogResult.OK;
        Close();
    }
}
