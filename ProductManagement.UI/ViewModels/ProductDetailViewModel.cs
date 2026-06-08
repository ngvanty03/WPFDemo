using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ProductManagement.Application;
using ProductManagement.Application.Exceptions;
using ProductManagement.Application.Interface;
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
        public ProductDetailViewModel(IDialogService dialogService,IProductService productService, IProductCategoryService productCateService, ILogger<ProductDetailViewModel> logger)
        { 
            _productService = productService;
            _productCateService = productCateService;
            _dialogService=dialogService;
            _logger = logger;
        }

        #region "Private properties"
        private readonly ILogger<ProductDetailViewModel> _logger;        
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCateService;
        private readonly IDialogService _dialogService;
        #endregion

        #region "Binding Properties"       
        [ObservableProperty]
        private ObservableCollection<ProductCategoryDTO> _categories;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private ProductDTO _product;

        [ObservableProperty]
        private bool _isLoading = false;

        #endregion

        #region "Functions"
        /// <summary>
        /// Init data when loading page
        /// </summary>
        /// <returns></returns>
        public async Task InitDataAsync(int productId)
        {
            try
            {
                IsLoading = true;
                await InitCategoryAsyn();
                await LoadProductAsync(productId);
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while loading data";
                _logger.LogError(ex.ToString());
            }
            finally {
                IsLoading = false;
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
        private async Task LoadProductAsync(int productId) {
            if (productId < 1)
            {
                Product = new ProductDTO();
                return;
            }
            Product = await _productService.GetByIdAsync(productId);            
        }
        #endregion

        #region "Command"
        /// <summary>
        /// Save to DB
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SaveProductAsync()
        {
            IsLoading = true;
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
                var result = false;
                if (Product.Id > 0)
                    result = await _productService.UpdateAsync(Product);
                else
                    result = await _productService.InsertAsync(Product);
                if (result)
                    _dialogService.CloseDialog(this, true);
            }
            catch (BusinessException ex)
            {
                ErrorMessage = ex.Message;
                _logger.LogError(ex.ToString());
            }
            catch (Exception ex) {
                ErrorMessage = "An error occurred while saving data";
                _logger.LogError(ex.ToString());
            }
            finally { IsLoading = false; }
        }
        [RelayCommand]
        private void Close()
        {
            _dialogService.CloseDialog(this, false);
        }
        #endregion
    }
}
