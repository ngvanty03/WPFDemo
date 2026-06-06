using ProductManagement.Application.Interface;
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
        public bool Confirm(string message, string title = "Confirm") {
            var result = MessageBox.Show(message, title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
    }
}
