using ProductManagement.Application;
using ProductManagement.UI.ViewModel;
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
        public ProductList(IProductService productService, IProductCategoryService productCateService)
        {
            InitializeComponent();
            this.DataContext = new ProductListViewModel(productService, productCateService);
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
    }
}
