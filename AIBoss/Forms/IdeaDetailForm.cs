using AIBoss.Models;

namespace AIBoss.Forms;

public sealed class IdeaDetailForm : Form
{
    private readonly TextBox _contentBox = new();
    private bool _deleted;

    public string EditedContent => _contentBox.Text.Trim();
    public bool IsDeleted => _deleted;

    public IdeaDetailForm(IdeaItem idea)
    {
        Text = "想法详情";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(620, 440);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18),
            ColumnCount = 1,
            RowCount = 4
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        layout.Controls.Add(new Label
        {
            Text = $"创建时间：{idea.CreatedAt:yyyy-MM-dd HH:mm}",
            AutoSize = true,
            Font = new Font("Microsoft YaHei UI", 9)
        }, 0, 0);
        _contentBox.Multiline = true;
        _contentBox.ScrollBars = ScrollBars.Vertical;
        _contentBox.Dock = DockStyle.Fill;
        _contentBox.Margin = new Padding(3, 10, 3, 10);
        _contentBox.Text = idea.Content;
        layout.Controls.Add(_contentBox, 0, 1);

        var deleteButton = new Button { Text = "删除想法", Width = 110 };
        deleteButton.Click += DeleteButton_Click;
        layout.Controls.Add(deleteButton, 0, 2);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, AutoSize = true, Margin = new Padding(0, 12, 0, 0) };
        var saveButton = new Button { Text = "保存修改", Width = 100 };
        saveButton.Click += SaveButton_Click;
        buttons.Controls.Add(saveButton);
        buttons.Controls.Add(new Button { Text = "取消", DialogResult = DialogResult.Cancel, Width = 100 });
        layout.Controls.Add(buttons, 0, 3);

        Controls.Add(layout);
        AcceptButton = saveButton;
        CancelButton = buttons.Controls.OfType<Button>().First(button => button.Text == "取消");
        _contentBox.Select();
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EditedContent))
        {
            MessageBox.Show("想法内容不能为空。", "IDEA BOX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _contentBox.Focus();
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private void DeleteButton_Click(object? sender, EventArgs e)
    {
        var confirm = MessageBox.Show("确定要删除这条想法吗？删除后无法恢复。", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes)
        {
            return;
        }

        _deleted = true;
        DialogResult = DialogResult.OK;
        Close();
    }
}
