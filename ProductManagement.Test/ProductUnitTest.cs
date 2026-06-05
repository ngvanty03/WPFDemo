using Moq;
using ProductManagement.Application;
using ProductManagement.Application.Exceptions;
using ProductManagement.DTO;
using ProductManagement.Infrastructure;
using Xunit;
namespace ProductManagement.Test
{
    public class ProductUnitTest
    {
        [Fact(DisplayName = "Insert product should throw exception when SKU already exists")]
        public async Task InsertAsync_Should_ThrowBusinessException_WhenSkuAlreadyExists()
        {
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(x => x.CheckSKUExistedAsync(null, "A001"))
           .ReturnsAsync(true);
            var service = new ProductService(repoMock.Object);
            var product = new ProductDTO
            {
                SKU = "A001"
            };
            var ex = await Assert.ThrowsAsync<BusinessException>(
           () => service.InsertAsync(product));
            Assert.Equal(@"Could not insert because the SKU:A001 already existed.", ex.Message);
        }
        [Fact(DisplayName = "Insert product successfully when SKU does not exist")]
        public async Task InsertAsync_Success_WhenSkuNotExists()
        {
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(x => x.CheckSKUExistedAsync(null, "A001"))
           .ReturnsAsync(false);
            repoMock.Setup(x => x.InsertAsync(It.IsAny<ProductDTO>()))
        .ReturnsAsync(true);
            var service = new ProductService(repoMock.Object);
            var product = new ProductDTO
            {
                SKU = "A001"
            };
            var result = await service.InsertAsync(product);
            Assert.Equal(true, result);
        }
    }
}
