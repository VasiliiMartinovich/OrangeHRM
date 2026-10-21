using log4net;
using log4net.Config;

namespace OrangeHRM.Tests.Infrastructure;

public static class Log4NetConfig
{
    private static readonly Lazy<bool> Configuration = new(() =>
    {
        var logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");

        Directory.CreateDirectory(logDirectory);

        GlobalContext.Properties["LogDirectory"] = logDirectory;

        var configFile = new FileInfo(
            Path.Combine(
                AppContext.BaseDirectory,
                "log4net.config"));

        if (!configFile.Exists)
        {
            throw new FileNotFoundException("log4net.config was not found.", configFile.FullName);
        }

        var repository = LogManager.GetRepository(typeof(Log4NetConfig).Assembly);

        XmlConfigurator.Configure(repository, configFile);

        return true;
    });

    public static void Configure()
    {
        _ = Configuration.Value;
    }
}