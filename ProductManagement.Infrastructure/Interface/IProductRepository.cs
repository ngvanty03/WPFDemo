using ProductManagement.DTO;

namespace ProductManagement.Infrastructure
{
    public interface IProductRepository
    {
        Task<bool> DeleteAsync(int productId);
        Task<IEnumerable<ProductDTO>> GetAllAsync(int categoryId, string SKU);
        Task<ProductDTO?> GetByIdAsync(int productId);
        Task<bool> InsertAsync(ProductDTO product);
        Task<bool> UpdateAsync(ProductDTO product);
        Task<bool> CheckSKUExistedAsync(int? productId, string SKU);
    }
}