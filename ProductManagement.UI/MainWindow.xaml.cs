using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductManagement.Application;
using ProductManagement.Infrastructure;
using ProductManagement.UI.CustomDialog;
using ProductManagement.UI.ViewModel;
using ProductManagement.UI.ViewModels;
using ProductManagement.UI.Views;
using Serilog;
using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProductManagement.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var dbOptions = new DatabaseOptions
            {
                DBConnectionString = App.Configuration.GetConnectionString("DefaultConnection")

            };
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(Log.Logger);
            });
            var productRepos = new ProductRepository(loggerFactory.CreateLogger<ProductRepository>(), dbOptions);
            var productCateRepos = new ProductCategoryRepository(loggerFactory.CreateLogger<ProductCategoryRepository>(), dbOptions);
            var productService = new ProductService(loggerFactory.CreateLogger<ProductService>(), productRepos);
            var productCateService = new ProductCategoryService(loggerFactory.CreateLogger<ProductCategoryService>(),productCateRepos);
            var diaglogService = new DialogService();
            var productDetailVMLogger = loggerFactory.CreateLogger<ProductDetailViewModel>();
            var productListVMLogger = loggerFactory.CreateLogger<ProductListViewModel>();
            MainContent.Content = new ProductList(diaglogService, productService, productCateService, productDetailVMLogger,productListVMLogger);
        }
    }
}