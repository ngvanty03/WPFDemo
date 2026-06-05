using ProductManagement.DTO;

namespace ProductManagement.Infrastructure
{
    public interface IProductCategoryRepository
    {
        Task<IEnumerable<ProductCategoryDTO>> GetAllActiveAsync();
    }
}