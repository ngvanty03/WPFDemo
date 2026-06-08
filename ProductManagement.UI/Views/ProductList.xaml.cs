using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductManagement.Application;
using ProductManagement.Application.Interface;
using ProductManagement.DTO;
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
       
        public ProductList(ProductListViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext=viewModel;          
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
        private async void OpenProductDetail_Click(object sender, RoutedEventArgs e)
        {           
            if (sender is Button editButton)
            {               
                var currentProduct = editButton.CommandParameter as ProductDTO;
                if (this.DataContext is ProductListViewModel mainVM)
                {                  
                    int productId = currentProduct != null ? currentProduct.Id : 0;
                    var detailVM = App.Services.GetRequiredService<ProductDetailViewModel>();
                    var subForm = new ProductDetail(productId,detailVM);                  
                    subForm.Owner = Window.GetWindow(this);
                    subForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    detailVM.CloseAction = new Action(subForm.Close);
                    if (subForm.ShowDialog() == true)
                    {
                        await mainVM.LoadProductAsyn();
                    }                    
                }
            }
        }
        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button editButton)
            {
                var currentProduct = editButton.CommandParameter as ProductDTO;
                if (this.DataContext is ProductListViewModel mainVM)
                {
                    var result = MessageBox.Show($"Do you want to delete the product SKU:{currentProduct.SKU}?", "Delete product",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        await mainVM.DeleteAsync(currentProduct);
                    }                    
                }
            }
        }
    }
}
