using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Application.Interface
{
    public interface IDialogService
    {
        bool Confirm(string message, string title = "Confirm");
    }
}
