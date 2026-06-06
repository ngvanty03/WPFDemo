using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ProductManagement.Application;
using ProductManagement.Application.Interface;
using ProductManagement.DTO;
using ProductManagement.Infrastructure;
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
            SearchCommand = new AsyncRelayCommand(LoadProductAsyn);
            ClearCommand = new AsyncRelayCommand(ClearAsyn);
            EditCommand = new AsyncRelayCommand<ProductDTO>(ShowProductDetailAsync, CanAddNew);
            AddNewCommand = new AsyncRelayCommand<ProductDTO>(ShowProductDetailAsync, CanAddNew);
            DeleteCommand=new AsyncRelayCommand<ProductDTO>(DeleteAsyn, CanAddNew);
            NextPageCommand = new AsyncRelayCommand(NextPageAsync);
            PrevPageCommand = new AsyncRelayCommand(PrevPageAsync);
            SortCommand = new AsyncRelayCommand<DataGridSortingEventArgs>(SortAsync);
            CurrentPage = 1;
            IsLoading = true;
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

        [ObservableProperty] private int _totalCount;
        [ObservableProperty] private int _totalPages;
        [ObservableProperty] private int _currentPage = 1;
        [ObservableProperty] private bool _hasNextPage = false;
        [ObservableProperty] private bool _hasPrevPage = false;
        private const int PageSize = 20;
        public IAsyncRelayCommand NextPageCommand { get; }
        public IAsyncRelayCommand PrevPageCommand { get; }

        [ObservableProperty]
        private string _sortColumn = "Name";     // default sort

        [ObservableProperty]
        private bool _isSortAscending = true;

        public IAsyncRelayCommand<DataGridSortingEventArgs> SortCommand { get; }
        #endregion      

        #region "Functions"
        private async Task SortAsync(DataGridSortingEventArgs? e)
        {
            if (e is null) return;

            // Đổi chiều sort nếu click cùng column
            if (SortColumn == e.Column.SortMemberPath)
                IsSortAscending = !IsSortAscending;
            else
            {
                SortColumn = e.Column.SortMemberPath;
                IsSortAscending = true;
            }

            CurrentPage = 1; // reset về trang 1 khi sort
            await LoadProductAsyn();
        }
        public async Task InitDataAsync()
        {        
            await InitCategoryAsyn();
            await LoadProductAsyn();
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
            CurrentPage++;
            await LoadProductAsyn();
        }

        private async Task PrevPageAsync()
        {
            CurrentPage--;
            await LoadProductAsyn();
        }
        private async Task LoadProductAsyn() {
            IsLoading = true;
           // CurrentPage = 1;
            var result = await _productService.SearchAsync(SelectedCategoryId, SearchSKU,CurrentPage,PageSize,SortColumn,IsSortAscending);            
            Products = new ObservableCollection<ProductDTO>(result.Items);
            FoundData = Products.Count > 0;
            TotalCount = result.TotalCount;
            TotalPages = result.TotalPages;
            HasNextPage=result.HasNextPage;
            HasPrevPage = result.HasPrevPage;
            IsLoading = false;
        }
        private async Task ClearAsyn() { 
            SelectedCategoryId = 0;
            SearchSKU = "";
            CurrentPage = 1;
            await LoadProductAsyn();
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
        private bool CanDelete()
        {
            return !IsLoading;
        }
        #endregion
    }
}
