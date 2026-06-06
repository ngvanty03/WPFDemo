using Moq;
using ProductManagement.Application;
using ProductManagement.Application.Interface;
using ProductManagement.DTO;
using ProductManagement.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace ProductManagement.Test
{
    public class ProductDetailViewModelUnitTest
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly Mock<IProductCategoryService> _mockProductCateService;

        public ProductDetailViewModelUnitTest()
        {
            SynchronizationContext.SetSynchronizationContext(
                new SynchronizationContext());

            _mockProductService = new Mock<IProductService>();
            _mockProductCateService = new Mock<IProductCategoryService>();
        }
        [Fact(DisplayName = "Show require message when creating product with the product SKU is empty")]
        public async Task AsyncSaveCommand_ShouldRequire_SKU()
        {           
            var viewModel = new ProductDetailViewModel(1, _mockProductService.Object, _mockProductCateService.Object);
            viewModel.Product = new ProductDTO()
            {
                SKU="",
                Name="iPhone 17",
                CategoryId=1,
                Description="",
                IsActive=true,
            };
            await viewModel.SaveCommand.ExecuteAsync(null);
            Assert.Equal("Please input the product SKU", viewModel.ErrorMessage);           
        }
        [Fact(DisplayName = "Show require message when creating product with the product name is empty")]
        public async Task AsyncSaveCommand_ShouldRequire_Name()
        {           
            var viewModel = new ProductDetailViewModel(1, _mockProductService.Object, _mockProductCateService.Object);
            viewModel.Product = new ProductDTO()
            {
                SKU = "1000",
                Name = "",
                CategoryId = 1,
                Description = "",
                IsActive = true,
            };
            await viewModel.SaveCommand.ExecuteAsync(null);
            Assert.Equal("Please input the product name", viewModel.ErrorMessage);
        }
    }
}
