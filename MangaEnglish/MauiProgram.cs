using MangaEnglish.Services;
using Microsoft.Extensions.Logging;

namespace MangaEnglish;

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

        builder.Services.AddSingleton<DatabaseService>(s =>
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "MangaEnglish.db3");
            return new DatabaseService(dbPath);
        });

        builder.Services.AddSingleton<JsonDataInitializer>();
        

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
