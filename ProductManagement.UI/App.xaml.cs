using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Configuration;
using System.Data;
using System.Windows;
namespace ProductManagement.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .CreateLogger();
            Log.Information("Application started");
            base.OnStartup(e);
        }
    }

}
