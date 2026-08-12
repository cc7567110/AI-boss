namespace AIBoss.Forms;

public sealed class SessionResultForm : Form
{
    private readonly TextBox _actualOutputBox = new();
    private readonly bool _requireOutput;

    public string ActualOutput => _actualOutputBox.Text.Trim();

    public SessionResultForm(string taskTitle, string plannedOutput, bool requireOutput)
    {
        _requireOutput = requireOutput;
        Text = "结束工作轮次";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(580, 330);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18),
            ColumnCount = 1,
            RowCount = 5
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        layout.Controls.Add(new Label { Text = $"当前任务：{taskTitle}", AutoSize = true, Font = new Font("Microsoft YaHei UI", 10, FontStyle.Bold) }, 0, 0);
        layout.Controls.Add(new Label { Text = $"计划产出：{(string.IsNullOrWhiteSpace(plannedOutput) ? "未填写" : plannedOutput)}", AutoSize = true, MaximumSize = new Size(520, 0), Margin = new Padding(3, 10, 3, 8) }, 0, 1);
        layout.Controls.Add(new Label { Text = requireOutput ? "请填写本轮实际产出（必填）" : "请填写本轮实际产出（可选）", AutoSize = true }, 0, 2);
        _actualOutputBox.Multiline = true;
        _actualOutputBox.Dock = DockStyle.Fill;
        layout.Controls.Add(_actualOutputBox, 0, 3);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, AutoSize = true, Margin = new Padding(0, 16, 0, 0) };
        var finishButton = new Button { Text = "保存并结束", Width = 110 };
        finishButton.Click += FinishButton_Click;
        buttons.Controls.Add(finishButton);
        buttons.Controls.Add(new Button { Text = "返回继续工作", DialogResult = DialogResult.Cancel, Width = 110 });
        layout.Controls.Add(buttons, 0, 4);

        Controls.Add(layout);
        AcceptButton = finishButton;
        CancelButton = buttons.Controls.OfType<Button>().First(button => button.Text == "返回继续工作");
    }

    private void FinishButton_Click(object? sender, EventArgs e)
    {
        if (_requireOutput && string.IsNullOrWhiteSpace(ActualOutput))
        {
            MessageBox.Show("根据当前 Boss 规则，请先填写实际产出。", "需要记录产出", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _actualOutputBox.Focus();
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}
