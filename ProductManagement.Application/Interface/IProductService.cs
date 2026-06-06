using ProductManagement.DTO;
using ProductManagement.Shared;

namespace ProductManagement.Application
{
    public interface IProductService
    {
        Task<bool> DeleteAsync(int productId);
        Task<(IEnumerable<ProductDTO> Items, int TotalCount)> SearchAsync(int categoryId, string SKU, int pageNumber, int pageSize, string sortColumn, string sortDirection);
        Task<ProductDTO?> GetByIdAsync(int productId);
        Task<bool> InsertAsync(ProductDTO product);
        Task<bool> UpdateAsync(ProductDTO product);
    }
}