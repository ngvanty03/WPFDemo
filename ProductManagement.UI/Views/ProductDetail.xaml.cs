using Microsoft.Extensions.Logging;
using ProductManagement.Application;
using ProductManagement.UI.ViewModel;
using ProductManagement.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProductManagement.UI.Views
{
    /// <summary>
    /// Interaction logic for ProductDetail.xaml
    /// </summary>
    public partial class ProductDetail : Window
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductService _productService;
        public ProductDetail(int productId,IProductCategoryService productCategoryService, IProductService productService, ILogger<ProductDetailViewModel> _logger)
        {
            InitializeComponent();
            _productCategoryService = productCategoryService;
            _productService = productService;
            var viewModel=new ProductDetailViewModel(productId,productService, productCategoryService, _logger);
            this.Loaded += ProductDetail_Loaded;
            viewModel.CloseAction = new Action(this.Close);
            this.DataContext = viewModel;
        }

        private async void ProductDetail_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ProductDetailViewModel viewModel)
            {
                await viewModel.InitDataAsync();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            this.DialogResult = true;
        }
    }
}
