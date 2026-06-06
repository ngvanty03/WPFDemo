using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.Configuration;
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

            base.OnStartup(e);
        }
    }

}
