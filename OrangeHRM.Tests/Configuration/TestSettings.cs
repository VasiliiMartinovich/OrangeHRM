using Microsoft.Extensions.Configuration;

namespace OrangeHRM.Tests.Configuration;

public static class TestSettings
{
    private static readonly IConfigurationRoot Configuration =
        new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

    public static string BaseUrl =>
        Configuration["BaseUrl"]
        ?? throw new InvalidOperationException("BaseUrl is missing in appsettings.json");

    public static string Browser =>
        Configuration["Browser"]
        ?? throw new InvalidOperationException("Browser is missing in appsettings.json");

    public static bool Headless =>
        Configuration.GetValue<bool>("Headless");

    public static int TimeoutSeconds =>
        Configuration.GetValue<int>("TimeoutSeconds");
}