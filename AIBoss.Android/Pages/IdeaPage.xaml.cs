using AIBoss.Models;

namespace AIBoss.Pages;

public partial class IdeaPage : ContentPage
{
    public IdeaPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshIdeaList();
    }

    private async void SaveIdeaClicked(object? sender, EventArgs e)
    {
        var content = IdeaEditor.Text?.Trim();
        if (string.IsNullOrWhiteSpace(content))
        {
            await DisplayAlert("IDEA BOX", "请先写下想法。", "确定");
            return;
        }

        MainPage.Data.Ideas.Add(new IdeaItem { Content = content, CreatedAt = DateTime.Now });
        IdeaEditor.Text = string.Empty;
        MainPage.SaveData();
        RefreshIdeaList();
    }

    private async void DeleteIdeaClicked(object? sender, EventArgs e)
    {
        if (IdeaCollection.SelectedItem is not IdeaItem idea)
        {
            await DisplayAlert("IDEA BOX", "请先在列表中选择一条想法。", "确定");
            return;
        }

        var confirm = await DisplayAlert("确认删除", "确定要删除这条想法吗？删除后无法恢复。", "删除", "取消");
        if (!confirm) return;

        MainPage.Data.Ideas.Remove(idea);
        MainPage.SaveData();
        RefreshIdeaList();
    }

    private void IdeaSelected(object? sender, SelectionChangedEventArgs e)
    {
    }

    private async void IdeaTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is IdeaItem idea)
        {
            var page = new IdeaDetailPage(idea);
            await Navigation.PushModalAsync(new NavigationPage(page));
            MainPage.SaveData();
            RefreshIdeaList();
        }
    }

    internal void RefreshIdeaList()
    {
        var sorted = MainPage.Data.Ideas.OrderByDescending(i => i.CreatedAt).ToList();
        IdeaCollection.ItemsSource = sorted;
        IdeaCountLabel.Text = sorted.Count == 0 ? "暂无想法" : $"共 {sorted.Count} 条想法";
    }
}
