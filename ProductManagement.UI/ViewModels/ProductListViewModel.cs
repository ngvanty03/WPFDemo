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
            _productService = productService;
            _productCateService = productCateService;
            _dialogService = dialogService;
            _productDetailVMLogger = productDetailVMLogger;
            _productListVMLogger = productListVMLogger;
            SearchCommand = new AsyncRelayCommand(SearchProductAsyn);
            ClearCommand = new AsyncRelayCommand(ClearAsyn);            
            NextPageCommand = new AsyncRelayCommand(NextPageAsync);
            PrevPageCommand = new AsyncRelayCommand(PrevPageAsync);
            SortCommand = new AsyncRelayCommand<DataGridSortingEventArgs>(SortAsync);
            EditCommand = new AsyncRelayCommand<ProductDTO>(ShowProductDetailAsync);
            AddNewCommand = new AsyncRelayCommand<ProductDTO>(ShowProductDetailAsync);
            DeleteCommand = new AsyncRelayCommand<ProductDTO>(DeleteAsyn);
            ResetPageData();
        }

        #region "Properties"
        private readonly ILogger<ProductDetailViewModel> _productDetailVMLogger;
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

        #region "Command"
        public IAsyncRelayCommand SearchCommand { get; set; }
        public IAsyncRelayCommand ClearCommand { get; set; }        
        public IAsyncRelayCommand NextPageCommand { get; }
        public IAsyncRelayCommand PrevPageCommand { get; }
        public IAsyncRelayCommand<DataGridSortingEventArgs> SortCommand { get; }
        public IAsyncRelayCommand EditCommand { get; set; }
        public IAsyncRelayCommand AddNewCommand { get; set; }
        public IAsyncRelayCommand DeleteCommand { get; set; }
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
            var result = await _productCateService.GetAllActiveAsync();
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
            var result = await _productService.SearchAsync(SelectedCategoryId, SearchSKU, PagingParameter.CurrentPage, PagingParameter.PageSize, SortingParameter.SortColumn, SortingParameter.SortDirection);
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
                await _productService.DeleteAsync(product.Id);
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
        /// reset data when loading or sorting 
        /// </summary>
        private void ResetPageData()
        {
            SortingParameter.SortColumn = "Name";//default sorting
            SortingParameter.SortDirection = "ASC";
            PagingParameter.CurrentPage = 1;
        }
        /// <summary>
        /// Open product detail popup
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task ShowProductDetailAsync(ProductDTO? product)
        {
            var subFormOpen = _dialogService.ShowProductDetailForm(product?.Id ?? 0);//product is null --> get value = 0 
            if (subFormOpen != null && subFormOpen.Value)
            {
                await LoadProductAsyn();
            }
        }
        /// <summary>
        /// Delete product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task DeleteAsyn(ProductDTO? product)
        {
            var confirm = _dialogService.Confirm($"Do you want to delete the product SKU:{product.SKU}?");
            if (confirm)
            {
                await _productService.DeleteAsync(product.Id);
                await LoadProductAsyn();
            }
        }
        #endregion
    }
}
