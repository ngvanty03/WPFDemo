using CommunityToolkit.Mvvm.ComponentModel;
using ProductManagement.Application;
using ProductManagement.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.UI.ViewModel
{
    public partial class ProductListViewModel : ObservableObject
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCateService;

        public ProductListViewModel(IProductService productService, IProductCategoryService productCateService)
        {
            _productService = productService;
            _productCateService = productCateService;
        }
        [ObservableProperty]
        private ObservableCollection<ProductCategoryDTO> _categories;
        [ObservableProperty]
        private int _selectedCategoryId;
        #region "Functions"
        public async Task InitDataAsync()
        {
            await InitCategoryAsyn();
        }
        private async Task InitCategoryAsyn()
        {
            var result = await _productCateService.GetAllActiveAsync();
            // 1. Create a standard temporary list
            var tempList = new List<ProductCategoryDTO>
            {
                new ProductCategoryDTO { Id = 0, Name = "- - -" }
            };
            // 2. Add your results to the temp list
            tempList.AddRange(result);
            // 3. Assign a brand new ObservableCollection to the public property
            Categories = new ObservableCollection<ProductCategoryDTO>(tempList);
        }
        #endregion
    }
}
