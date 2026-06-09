using ProductManagement.DTO;

namespace ProductManagement.Application
{
    public interface IProductRepository
    {
        Task<bool> DeleteAsync(int productId);
        Task<(IEnumerable<ProductListDTO> Items, int TotalCount)> SearchAsync(int categoryId, string SKU, int pageNumber, int pageSize, string sortColumn, string sortDirection);
        Task<ProductDTO?> GetByIdAsync(int productId);
        Task<bool> InsertAsync(ProductDTO product);
        Task<bool> UpdateAsync(ProductDTO product);
        Task<bool> CheckSKUExistedAsync(int? productId, string SKU);
    }
}