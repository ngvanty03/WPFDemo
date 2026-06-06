using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ProductManagement.Application;
using ProductManagement.Application.Interface;
using ProductManagement.DTO;
using ProductManagement.Infrastructure;
using ProductManagement.UI.Utils;
using ProductManagement.UI.Views;
using ProductManagement.UI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Text;
using System.Windows.Controls;
using System.Windows.Input;
namespace ProductManagement.UI.ViewModel
{
    public partial class ProductListViewModel : ObservableObject
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCateService;
        private readonly IDialogService _dialogService;
        #region "Command"
        public IAsyncRelayCommand SearchCommand { get; set; }
        public IAsyncRelayCommand ClearCommand { get; set; }
        public IAsyncRelayCommand EditCommand { get; set; }
        public IAsyncRelayCommand AddNewCommand { get; set; }
        public IAsyncRelayCommand DeleteCommand { get; set; }
        #endregion
        public ProductListViewModel(IDialogService dialogService,IProductService productService, IProductCategoryService productCateService)
        {
            _productService = productService;
            _productCateService = productCateService;
            _dialogService = dialogService;
            SearchCommand = new AsyncRelayCommand(SearchProductAsyn);
            ClearCommand = new AsyncRelayCommand(ClearAsyn);
            EditCommand = new AsyncRelayCommand<ProductDTO>(ShowProductDetailAsync, CanAddNew);
            AddNewCommand = new AsyncRelayCommand<ProductDTO>(ShowProductDetailAsync, CanAddNew);
            DeleteCommand=new AsyncRelayCommand<ProductDTO>(DeleteAsyn, CanAddNew);
            NextPageCommand = new AsyncRelayCommand(NextPageAsync);
            PrevPageCommand = new AsyncRelayCommand(PrevPageAsync);
            SortCommand = new AsyncRelayCommand<DataGridSortingEventArgs>(SortAsync);
            ResetPageData();
        }

        #region "Bindding Properties"
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
        // Tracks if the search service is actively running
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddNewCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
        private bool _isLoading=false;
        [ObservableProperty] private PagingParameters _pagingParameter = new();
        public IAsyncRelayCommand NextPageCommand { get; }
        public IAsyncRelayCommand PrevPageCommand { get; }

        [ObservableProperty] private SortingParameters _sortingParameter = new();

        public IAsyncRelayCommand<DataGridSortingEventArgs> SortCommand { get; }
        #endregion      

        #region "Functions"
        private async Task SortAsync(DataGridSortingEventArgs? e)
        {
            if (e is null) return;
            IsLoading=true;
            SortingParameter.UpdateState(e.Column.SortMemberPath);
            PagingParameter.CurrentPage = 1; // reset về trang 1 khi sort
            await LoadProductAsyn();
            IsLoading = false;
        }
        public async Task InitDataAsync()
        {        
            IsLoading=true;
            await InitCategoryAsyn();
            await LoadProductAsyn();
            IsLoading = false;
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
        private async Task NextPageAsync()
        {
            IsLoading = true;
            PagingParameter.CurrentPage++;
            await LoadProductAsyn();
            IsLoading = false;
        }

        private async Task PrevPageAsync()
        {
            IsLoading = true;
            PagingParameter.CurrentPage--;
            await LoadProductAsyn();
            IsLoading = false;
        }
        private async Task SearchProductAsyn()
        {
            IsLoading = true;
            ResetPageData();
            await LoadProductAsyn();
            IsLoading = false;
        }
        private async Task LoadProductAsyn() {            
            var result = await _productService.SearchAsync(SelectedCategoryId, SearchSKU, PagingParameter.CurrentPage, PagingParameter.PageSize, SortingParameter.SortColumn, SortingParameter.SortDirection);            
            Products = new ObservableCollection<ProductDTO>(result.Items);
            FoundData = Products.Count > 0;
            PagingParameter.UpdateState(result.TotalCount);
        }
        private async Task ClearAsyn() {
            IsLoading = true;
            SelectedCategoryId = 0;
            SearchSKU = "";
            ResetPageData();
            await LoadProductAsyn();
            IsLoading = false;
        }
        public async Task ShowProductDetailAsync(ProductDTO product) 
        { 
            var subForm= new ProductDetail(product!=null?product.Id:0,_productCateService,_productService);
            subForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bool? result = subForm.ShowDialog();
            if (result != null && result.Value)
            {
                await LoadProductAsyn();
            }
        }
        public async Task DeleteAsyn(ProductDTO? product)
        {
            var confirm = _dialogService.Confirm($"Do you want to delete the product SKU:{product.SKU}?");
            if (confirm)
            {
                await _productService.DeleteAsync(product.Id);
                await LoadProductAsyn();
            }
        }
        private bool CanAddNew(ProductDTO product)
        {
            return !IsLoading;
        }
        private void ResetPageData()
        {
            SortingParameter.SortColumn = "Name";//default sorting
            SortingParameter.SortDirection = "ASC";
            PagingParameter.CurrentPage = 1;
        }

        #endregion
    }
}
