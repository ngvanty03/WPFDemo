using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductManagement.Application;
using ProductManagement.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProductManagement.UI.ViewModels
{
    public partial class ProductDetailViewModel : ObservableObject
    {
        public Action CloseAction { get; set; }
        private readonly int _productId;
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCateService;
        public ProductDetailViewModel(int productId,IProductService productService, IProductCategoryService productCateService)
        { 
            _productService = productService;
            _productCateService = productCateService;
            _productId = productId;
            SaveCommand = new AsyncRelayCommand(SaveProductAsync);
            CloseCommand = new RelayCommand(Close);
        }
        #region "Bindding Properties"        
        [ObservableProperty]
        private ObservableCollection<ProductCategoryDTO> _categories;
        [ObservableProperty]
        private string _errorMessage;
        [ObservableProperty]
        private ProductDTO _product;
        #endregion

        #region "Command"
        public IAsyncRelayCommand SaveCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        #endregion
        #region "Methods"
        public async Task InitDataAsync()
        {
            await InitCategoryAsyn();
            await LoadProductAsync();
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
        private async Task LoadProductAsync() {
            if (_productId < 1)
            {
                Product = new ProductDTO();
                return;
            }
            Product = await _productService.GetByIdAsync(_productId);            
        }
        private async Task SaveProductAsync()
        {
            try
            {
                //validate input
                if (string.IsNullOrEmpty(Product.SKU))
                {
                    ErrorMessage = "Please input the product SKU";
                    return;
                }
                if (Product.CategoryId < 1)
                {
                    ErrorMessage = "Please select the product Category";
                    return;
                }
                if (string.IsNullOrEmpty(Product.Name))
                {
                    ErrorMessage = "Please input the product name";
                    return;
                }
                if (_productId > 0)
                    await _productService.UpdateAsync(Product);
                else
                    await _productService.InsertAsync(Product);
                Close();
            }
            catch (Exception ex) { 
                ErrorMessage = ex.Message;
            }
        }        
        public void Close()
        {
            // 2. Gọi Invoke để kích hoạt hành động đóng form từ View truyền vào
            CloseAction?.Invoke();
        }
        #endregion
    }
}
