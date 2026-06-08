using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ProductManagement.Application;
using ProductManagement.Application.Interface;
using ProductManagement.DTO;
using ProductManagement.UI.Utils;
using ProductManagement.UI.ViewModels;
using ProductManagement.UI.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
namespace ProductManagement.UI.ViewModel
{
    public partial class ProductListViewModel : ObservableObject
    {        
        public ProductListViewModel(IDialogService dialogService,IProductService productService, IProductCategoryService productCateService, ILogger<ProductListViewModel> productListVMLogger)
        {
            _productService = productService;
            _productCateService = productCateService;
            _dialogService = dialogService;
            _productListVMLogger = productListVMLogger;           
            ResetPageData();
        }

        #region "Properties"        
        private readonly ILogger<ProductListViewModel> _productListVMLogger;
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCateService;
        private readonly IDialogService _dialogService;
        #endregion

        #region "Binding Properties"
        [ObservableProperty] 
        private string _searchSKU;

        [ObservableProperty] 
        private ObservableCollection<ProductCategoryDTO> _categories;

        [ObservableProperty] 
        private int _selectedCategoryId;

        [ObservableProperty] 
        private ObservableCollection<ProductDTO> _products = new();

        [ObservableProperty] 
        private bool _foundData=true;

        [ObservableProperty]      
        private bool _isLoading=false;

        [ObservableProperty] 
        private PagingParameters _pagingParameter = new();        

        [ObservableProperty] 
        private SortingParameters _sortingParameter = new();
        #endregion


        #region "Functions"
        /// <summary>
        /// Init data when loading page
        /// </summary>
        /// <returns></returns>
        public async Task InitDataAsync()
        {
            IsLoading = true;
            try
            {
                await InitCategoryAsync();
                await LoadProductAsync();
            }
            catch (Exception ex)
            {
                _productListVMLogger.LogError(ex.ToString());
            }
            finally {
                IsLoading = false;
            }
        }
        /// <summary>
        /// Load category data
        /// </summary>
        /// <returns></returns>
        private async Task InitCategoryAsync()
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
        public async Task LoadProductAsync()
        {
            var result = await _productService.SearchAsync(SelectedCategoryId, SearchSKU, PagingParameter.CurrentPage, PagingParameter.PageSize, SortingParameter.SortColumn, SortingParameter.SortDirection);
            Products = new ObservableCollection<ProductDTO>(result.Items);
            FoundData = Products.Count > 0;
            PagingParameter.UpdateState(result.TotalCount);
        }        
          
        /// <summary>
        /// reset data when loading or sorting 
        /// </summary>
        private void ResetPageData()
        {
            SortingParameter.SortColumn = "Name";//default sorting
            SortingParameter.SortDirection = "ASC";
            PagingParameter.CurrentPage = 1;
        }
        #endregion

        #region "Command"
        /// <summary>
        /// Search button event
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SearchProductAsync()
        {
            IsLoading = true;
            try
            {
                ResetPageData();
                await LoadProductAsync();
            }
            catch (Exception ex)
            {
                _productListVMLogger.LogError(ex.ToString());
            }
            finally
            {
                IsLoading = false;
            }

        }
        /// <summary>
        /// Delete product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [RelayCommand]
        public async Task DeleteAsync(ProductDTO? product)
        {
            var confirm = _dialogService.Confirm($"Do you want to delete the product SKU:{product.SKU}?");
            if (confirm)
            {
                await _productService.DeleteAsync(product.Id);
                await LoadProductAsync();
            }
        }
        /// <summary>
        /// Open product detail popup
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [RelayCommand]
        public async Task ShowProductDetailAsync(ProductDTO? product)
        {
            var subFormOpen = _dialogService.ShowProductDetailForm(product?.Id ?? 0);//product is null --> get value = 0 
            if (subFormOpen != null && subFormOpen.Value)
            {
                await LoadProductAsync();
            }
        }
        /// <summary>
        /// Sort event
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [RelayCommand]
        private async Task SortAsync(DataGridSortingEventArgs? e)
        {
            if (e is null) return;
            IsLoading = true;
            try
            {
                SortingParameter.UpdateState(e.Column.SortMemberPath);
                PagingParameter.CurrentPage = 1;
                await LoadProductAsync();
            }
            catch (Exception ex)
            {
                _productListVMLogger.LogError(ex.ToString());
            }
            finally
            {
                IsLoading = false;
            }
        }
        /// <summary>
        /// Paging event:  next page event
        /// </summary>
        /// <returns></returns>  
        [RelayCommand]
        private async Task NextPageAsync()
        {
            IsLoading = true;
            try
            {
                PagingParameter.NextPage();
                await LoadProductAsync();
            }
            catch (Exception ex)
            {
                _productListVMLogger.LogError(ex.ToString());
            }
            finally
            {
                IsLoading = false;
            }
        }
        /// <summary>
        /// Paging event:  previous page event
        /// </summary>
        /// <returns></returns>  
        [RelayCommand]
        private async Task PrevPageAsync()
        {
            IsLoading = true;
            try
            {
                PagingParameter.PrevPage();
                await LoadProductAsync();
            }
            catch (Exception ex)
            {
                _productListVMLogger.LogError(ex.ToString());
            }
            finally
            {
                IsLoading = false;
            }
        }
        /// <summary>
        /// Clean button event
        /// </summary>
        /// <returns></returns>
        /// 
        [RelayCommand]
        private async Task ClearAsync()
        {
            IsLoading = true;
            try
            {
                SelectedCategoryId = 0;
                SearchSKU = "";
                ResetPageData();
                await LoadProductAsync();
            }
            catch (Exception ex)
            {
                _productListVMLogger.LogError(ex.ToString());
            }
            finally
            {
                IsLoading = false;
            }
        }
        #endregion
    }
}
