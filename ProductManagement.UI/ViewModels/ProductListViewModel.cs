using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductManagement.Application;
using ProductManagement.DTO;
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
        #region "Command"
        public ICommand SearchCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand EditCommand { get; set; }
        #endregion
        public ProductListViewModel(IProductService productService, IProductCategoryService productCateService)
        {
            _productService = productService;
            _productCateService = productCateService;
            SearchCommand = new AsyncRelayCommand(LoadProductAsyn);
            ClearCommand = new AsyncRelayCommand(ClearAsyn);
            EditCommand = new AsyncRelayCommand<ProductDTO>(ShowProductDetail);
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
            var result = await _productService.GetAllAsync(SelectedCategoryId, SearchSKU);            
            Products = new ObservableCollection<ProductDTO>(result);
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
            var subForm= new ProductDetail(_productCateService,_productService);
            subForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bool? result = subForm.ShowDialog();
            if (result != null && result.Value)
            {
                await LoadProductAsyn();
            }
        }
        #endregion
    }
}
