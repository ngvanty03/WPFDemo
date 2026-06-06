using ProductManagement.DTO;
using ProductManagement.Shared;

namespace ProductManagement.Application
{
    public interface IProductService
    {
        Task<bool> DeleteAsync(int productId);
        Task<PagedResult<ProductDTO>> SearchAsync(int categoryId, string SKU, int pageNumber, int pageSize, string sortColumn, bool ascending);
        Task<ProductDTO?> GetByIdAsync(int productId);
        Task<bool> InsertAsync(ProductDTO product);
        Task<bool> UpdateAsync(ProductDTO product);
    }
}