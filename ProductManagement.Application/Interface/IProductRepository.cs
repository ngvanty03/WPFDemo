using ProductManagement.DTO;

namespace ProductManagement.Application
{
    public interface IProductRepository
    {
        Task<bool> DeleteAsync(int productId);
        Task<(IEnumerable<ProductDTO> Items, int TotalCount)> SearchAsync(int categoryId, string SKU, int pageNumber, int pageSize, string sortColumn, bool ascending);
        Task<ProductDTO?> GetByIdAsync(int productId);
        Task<bool> InsertAsync(ProductDTO product);
        Task<bool> UpdateAsync(ProductDTO product);
        Task<bool> CheckSKUExistedAsync(int? productId, string SKU);
    }
}