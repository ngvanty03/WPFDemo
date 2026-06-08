using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductManagement.Application;
using ProductManagement.Application.Interface;
using ProductManagement.Infrastructure;
using ProductManagement.UI.CustomDialog;
using ProductManagement.UI.ViewModel;
using ProductManagement.UI.ViewModels;
using ProductManagement.UI.Views;
using Serilog;
using System;
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
        public static IServiceProvider Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();
        }
        private void ConfigureServices(IServiceCollection services)
        {
            Configuration = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json", optional: false)
               .Build();
            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .CreateLogger();            
            services.AddLogging(builder =>
            {
                builder.AddSerilog(Log.Logger);
            });
            services.AddSingleton(new DatabaseOptions
            {
                DBConnectionString = Configuration.GetConnectionString("DefaultConnection")
            });
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<IProductCategoryRepository, ProductCategoryRepository>();
            services.AddSingleton<IProductService, ProductService>();
            services.AddSingleton<IProductCategoryService, ProductCategoryService>();          
            services.AddTransient<ProductListViewModel>();
            services.AddTransient<ProductDetailViewModel>();
        }
    }

}
