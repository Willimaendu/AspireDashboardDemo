namespace AspireDashboardDemo.Diagnostics
{
    public static partial class LoggerExtensions
    {
        [LoggerMessage(
            EventId = 11000,
            Level = LogLevel.Information,
            EventName = "CallingWeatherForecast",
            Message = "Request has been delayed for {seconds} seconds",
            SkipEnabledCheck = true)]
        public static partial void DelayedForSeconds(this ILogger logger,int seconds);
    }
}
