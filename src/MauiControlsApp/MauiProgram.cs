using Drastic.AllTheControls.ViewModels;
using Drastic.AppToolbox.Services;
using Microsoft.Extensions.Logging;

namespace MauiControlsApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<IAppDispatcher, AppDispatcher>()
            .AddSingleton<IErrorHandler, ErrorHandler>()
            .AddSingleton<IAsyncCommandFactory, AsyncCommandFactory>()
            .AddSingleton<TextListViewModel>()
            .AddSingleton<TextListPage>()
            .AddSingleton<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
