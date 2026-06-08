using Microsoft.Extensions.DependencyInjection;
using ProductManagement.Application.Interface;
using ProductManagement.DTO;
using ProductManagement.UI.ViewModels;
using ProductManagement.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProductManagement.UI.CustomDialog
{
    public class DialogService : IDialogService
    {
        /// <summary>
        /// show confirmation message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool Confirm(string message, string title = "Confirm") {
            var result = MessageBox.Show(message, title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
        /// <summary>
        /// Show the product detail popup
        /// Todo: generic this form
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public bool? ShowProductDetailForm(int productId)
        {            
            var detailVM = App.Services.GetRequiredService<ProductDetailViewModel>();
            _ = detailVM.InitDataAsync(productId);//load data and ignore the task result
            var subForm = new ProductDetail(detailVM);
            if (App.Current.MainWindow != null)
                subForm.Owner = App.Current.MainWindow;
            subForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            return subForm.ShowDialog();           
        }
        /// <summary>
        /// close the dialog window
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="dialogResult"></param>
        public void CloseDialog(object viewModel, bool dialogResult)
        {
            // get the current window belong to the viewModel
            var targetWindow = App.Current.Windows
                .Cast<Window>()
                .FirstOrDefault(w => w.DataContext == viewModel);

            if (targetWindow != null)
            {
                // close window
                targetWindow.DialogResult = dialogResult;
            }
        }
    }
}
