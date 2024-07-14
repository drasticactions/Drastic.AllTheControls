namespace MauiControlsApp;

public partial class App : Application
{
	public App(IServiceProvider provider)
	{
		InitializeComponent();

		var mainPage = provider.GetRequiredService<MainPage>();
		MainPage = new NavigationPage(mainPage);
	}
}
