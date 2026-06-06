using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
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
        public Action CloseAction { get; set; }//event to close form
        
        public ProductDetailViewModel(int productId,IProductService productService, IProductCategoryService productCateService, ILogger<ProductDetailViewModel> logger)
        { 
            _productService = productService;
            _productCateService = productCateService;
            _productId = productId;
            _logger = logger;
            SaveCommand = new AsyncRelayCommand(SaveProductAsync);
            CloseCommand = new RelayCommand(Close);
        }

        #region "Private properties"
        private readonly ILogger<ProductDetailViewModel> _logger;
        private readonly int _productId;
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCateService;
        #endregion

        #region "Binding Properties"       
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

        #region "Functions"
        /// <summary>
        /// Init data when loading page
        /// </summary>
        /// <returns></returns>
        public async Task InitDataAsync()
        {
            try
            {
                await InitCategoryAsyn();
                await LoadProductAsync();
            }
            catch (Exception ex) {
                ErrorMessage = "An error occurred while loading data";
                _logger.LogError(ex.ToString());
            }
        }
        /// <summary>
        /// Load category data
        /// </summary>
        /// <returns></returns>
        private async Task InitCategoryAsyn()
        {
            var result = await _productCateService.GetAllActiveAsync();           
            var tempList = new List<ProductCategoryDTO>
            {
                new ProductCategoryDTO { Id = 0, Name = "- - -" }
            };            
            tempList.AddRange(result);            
            Categories = new ObservableCollection<ProductCategoryDTO>(tempList);
        }
        /// <summary>
        /// Load product from DB
        /// </summary>
        /// <returns></returns>
        private async Task LoadProductAsync() {
            if (_productId < 1)
            {
                Product = new ProductDTO();
                return;
            }
            Product = await _productService.GetByIdAsync(_productId);            
        }
        /// <summary>
        /// Save to DB
        /// </summary>
        /// <returns></returns>
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
                ErrorMessage = "An error occurred while saving data";
                _logger.LogError(ex.ToString());
            }
        }        
        /// <summary>
        /// Close button event
        /// </summary>
        public void Close()
        {            
            CloseAction?.Invoke();
        }
        #endregion
    }
}
