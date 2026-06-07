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
        public ProductDetail(int productId,ProductDetailViewModel viewModel)
        {
            InitializeComponent();
            viewModel.CloseAction = new Action(this.Close);
            this.DataContext = viewModel;
            Loaded += async (_, _) =>
            {
                await viewModel.InitDataAsync(productId);
            };
        }      
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            this.DialogResult = true;
        }
    }
}
