using Serilog;

namespace SimpleStore.API.Services
{
    public static class AppExtension
    {
        public static IHostBuilder SerilogConfiguration(this IHostBuilder host)
        {
            return host.UseSerilog((context, loggerConfig) =>
            {
                loggerConfig.WriteTo.Console();
            });
        }
    }
}
