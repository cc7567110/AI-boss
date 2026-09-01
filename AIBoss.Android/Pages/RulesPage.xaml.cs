namespace AIBoss.Pages;

public partial class RulesPage : ContentPage
{
    public RulesPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RulesCollection.ItemsSource = MainPage.Data.BossRules;
    }

    private async void SaveRulesClicked(object? sender, EventArgs e)
    {
        MainPage.SaveData();
        await DisplayAlert("保存成功", "Boss 规则已保存。", "确定");
    }
}
