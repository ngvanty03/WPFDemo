using ProductManagement.DTO;

namespace ProductManagement.Application
{
    public interface IProductCategoryRepository
    {
        Task<IEnumerable<ProductCategoryDTO>> GetAllActiveAsync();
    }
}