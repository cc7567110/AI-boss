using AIBoss.Models;

namespace AIBoss.Pages;

public partial class IdeaDetailPage : ContentPage
{
    private readonly IdeaItem _idea;

    public IdeaDetailPage(IdeaItem idea)
    {
        InitializeComponent();
        _idea = idea;
        CreatedAtLabel.Text = $"创建时间：{idea.CreatedAt:yyyy-MM-dd HH:mm}";
        ContentEditor.Text = idea.Content;
    }

    private async void SaveClicked(object? sender, EventArgs e)
    {
        var content = ContentEditor.Text?.Trim();
        if (string.IsNullOrWhiteSpace(content))
        {
            await DisplayAlert("提示", "想法内容不能为空。", "确定");
            return;
        }
        _idea.Content = content;
        MainPage.SaveData();
        await Navigation.PopModalAsync();
    }

    private async void DeleteClicked(object? sender, EventArgs e)
    {
        var confirm = await DisplayAlert("确认删除", "确定要删除这条想法吗？删除后无法恢复。", "删除", "取消");
        if (!confirm) return;

        MainPage.Data.Ideas.Remove(_idea);
        MainPage.SaveData();
        await Navigation.PopModalAsync();
    }

    private async void CancelClicked(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
