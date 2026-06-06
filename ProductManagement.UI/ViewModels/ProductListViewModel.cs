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
        public ICommand EditCommand { get; set; }
        public ICommand AddNewCommand { get; set; }
        public IAsyncRelayCommand DeleteCommand { get; set; }
        #endregion
        public ProductListViewModel(IDialogService dialogService,IProductService productService, IProductCategoryService productCateService)
        {
            _productService = productService;
            _productCateService = productCateService;
            _dialogService = dialogService;
            SearchCommand = new AsyncRelayCommand(LoadProductAsyn);
            ClearCommand = new AsyncRelayCommand(ClearAsyn);
            EditCommand = new AsyncRelayCommand<ProductDTO>(ShowProductDetail);
            AddNewCommand = new AsyncRelayCommand<ProductDTO>(ShowProductDetail);
            DeleteCommand=new AsyncRelayCommand<ProductDTO>(DeleteAsyn);
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
        private bool _isLoading=false;
        #endregion      

        #region "Functions"
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
        private async Task LoadProductAsyn() {
            IsLoading = true;
            //await Task.Delay(50000);
            var result = await _productService.SearchAsync(SelectedCategoryId, SearchSKU,1,10);            
            Products = new ObservableCollection<ProductDTO>(result.Items);
            FoundData = Products.Count > 0;
            IsLoading = false;
        }
        private async Task ClearAsyn() { 
            SelectedCategoryId = 0;
            SearchSKU = "";
            await LoadProductAsyn();
        }
        public async Task ShowProductDetail(ProductDTO product) 
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
        #endregion
    }
}
