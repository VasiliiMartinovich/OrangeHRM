    using OpenQA.Selenium;

    namespace OrangeHRM.Framework.Core.Drivers;

    public static class DriverManager
    {
        [ThreadStatic]
        private static IWebDriver? _driver;
        public static int TimeoutSeconds { get; set; } = 10;

        public static IWebDriver Driver => _driver ?? throw new InvalidOperationException(
                "WebDriver has not been initialized for the current thread.");

        public static void SetDriver(IWebDriver driver)
        {
            ArgumentNullException.ThrowIfNull(driver);
            _driver = driver;
        }

        public static void QuitDriver()
        {
            try
            {
                _driver?.Quit();
            }
            finally
            {
                _driver = null;
            }
        }
    }