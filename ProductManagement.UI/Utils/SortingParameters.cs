using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.UI.Utils
{
    public partial class SortingParameters : ObservableObject
    {
        [ObservableProperty]
        private string _sortColumn = "";  
        [ObservableProperty]
        private string _sortDirection = "ASC";       
        
        public void UpdateState(string sortColumnParameter)
        {
            var tmpSortDirection = "ASC";
            // 1. Đổi chiều sort nếu click cùng một cột
            if (SortColumn == sortColumnParameter && SortDirection == "ASC")
            {
                tmpSortDirection = "DESC";
            }
            SortColumn=sortColumnParameter;
            SortDirection = tmpSortDirection;
        }
    }
}
