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
        public ProductListViewModel(IDialogService dialogService,IProductService productService, IProductCategoryService productCateService, ILogger<ProductDetailViewModel> productDetailVMLogger, ILogger<ProductListViewModel> productListVMLogger)
        {
            ProductService = productService;
            ProductCateService = productCateService;
            _dialogService = dialogService;
            ProductDetailVMLogger = productDetailVMLogger;
            _productListVMLogger = productListVMLogger;
            SearchCommand = new AsyncRelayCommand(SearchProductAsyn);
            ClearCommand = new AsyncRelayCommand(ClearAsyn);            
            NextPageCommand = new AsyncRelayCommand(NextPageAsync);
            PrevPageCommand = new AsyncRelayCommand(PrevPageAsync);
            SortCommand = new AsyncRelayCommand<DataGridSortingEventArgs>(SortAsync);
            ResetPageData();
        }

        #region "Properties"
        public readonly ILogger<ProductDetailViewModel> ProductDetailVMLogger;
        private readonly ILogger<ProductListViewModel> _productListVMLogger;
        public readonly IProductService ProductService;
        public readonly IProductCategoryService ProductCateService;
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

        #region "Command"
        public IAsyncRelayCommand SearchCommand { get; set; }
        public IAsyncRelayCommand ClearCommand { get; set; }        
        public IAsyncRelayCommand NextPageCommand { get; }
        public IAsyncRelayCommand PrevPageCommand { get; }
        public IAsyncRelayCommand<DataGridSortingEventArgs> SortCommand { get; }
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
                await InitCategoryAsyn();
                await LoadProductAsyn();
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
        private async Task InitCategoryAsyn()
        {
            var result = await ProductCateService.GetAllActiveAsync();
            var tempList = new List<ProductCategoryDTO>
            {
                new ProductCategoryDTO { Id = 0, Name = "- - -" }
            };
            tempList.AddRange(result);
            Categories = new ObservableCollection<ProductCategoryDTO>(tempList);
        }        
        /// <summary>
        /// Search button event
        /// </summary>
        /// <returns></returns>
        private async Task SearchProductAsyn()
        {
            IsLoading = true;
            try
            {
                ResetPageData();
                await LoadProductAsyn();
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
        /// Load product from DB
        /// </summary>
        /// <returns></returns>
        public async Task LoadProductAsyn()
        {
            var result = await ProductService.SearchAsync(SelectedCategoryId, SearchSKU, PagingParameter.CurrentPage, PagingParameter.PageSize, SortingParameter.SortColumn, SortingParameter.SortDirection);
            Products = new ObservableCollection<ProductDTO>(result.Items);
            FoundData = Products.Count > 0;
            PagingParameter.UpdateState(result.TotalCount);
        }
        /// <summary>
        /// Paging event:  next page event
        /// </summary>
        /// <returns></returns>
        private async Task NextPageAsync()
        {
            IsLoading = true;
            try
            {
                PagingParameter.NextPage();
                await LoadProductAsyn();
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
        private async Task PrevPageAsync()
        {
            IsLoading = true;
            try
            {
                PagingParameter.PrevPage();
                await LoadProductAsyn();
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
        private async Task ClearAsyn() {
            IsLoading = true;   
            try
            {
                SelectedCategoryId = 0;
                SearchSKU = "";
                ResetPageData();
                await LoadProductAsyn();
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
        /// Delete button event
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task DeleteAsync(ProductDTO? product)
        {
            IsLoading = true;
            try
            {
                await ProductService.DeleteAsync(product.Id);
                await LoadProductAsyn();
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
        /// Sort event
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task SortAsync(DataGridSortingEventArgs? e)
        {
            if (e is null) return;           
            IsLoading = true;
            try
            {
                SortingParameter.UpdateState(e.Column.SortMemberPath);
                PagingParameter.CurrentPage = 1;
                await LoadProductAsyn();
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
        /// Allow AddNew command
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private bool CanAddNew(ProductDTO product)
        {
            return !IsLoading;
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
    }
}
