using ProductManagement.DTO;

namespace ProductManagement.Application
{
    public interface IProductCategoryService
    {
        Task<IEnumerable<ProductCategoryDTO>> GetAllActiveAsync();
    }
}