using ProductManagement.DTO;

namespace ProductManagement.Application
{
    public interface IProductService
    {
        Task<bool> DeleteAsync(int productId);
        Task<IEnumerable<ProductDTO>> GetAllAsync(int categoryId, string SKU);
        Task<ProductDTO?> GetByIdAsync(int productId);
        Task<bool> InsertAsync(ProductDTO product);
        Task<bool> UpdateAsync(ProductDTO product);
    }
}