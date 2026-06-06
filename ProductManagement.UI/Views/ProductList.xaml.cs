using Microsoft.Extensions.Logging;
using ProductManagement.Application;
using ProductManagement.Application.Interface;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProductManagement.UI.Views
{
    /// <summary>
    /// Interaction logic for ProductList.xaml
    /// </summary>
    public partial class ProductList : UserControl
    {
        public ProductList(IDialogService dialogService,IProductService productService, IProductCategoryService productCateService,
            ILogger<ProductDetailViewModel> productDetailVMLogger, ILogger<ProductListViewModel> productListVMLogger)
        {
            InitializeComponent();
            this.DataContext = new ProductListViewModel(dialogService,productService, productCateService, productDetailVMLogger, productListVMLogger);
            this.Loaded += ProductList_Loaded;
        }

        private async void ProductList_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ProductListViewModel viewModel)
            {
                await viewModel.InitDataAsync();
            }
            txtSKU.Focus();
        }
        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true; // prevent in memory sorting

            if (DataContext is ProductListViewModel vm)
                vm.SortCommand.Execute(e);
        }
    }
}
