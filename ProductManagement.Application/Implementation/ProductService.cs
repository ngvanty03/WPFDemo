using Microsoft.Extensions.Logging;
using ProductManagement.Application.Exceptions;
using ProductManagement.DTO;
namespace ProductManagement.Application
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly ILogger<ProductService> _logger;
        public ProductService(ILogger<ProductService> logger, IProductRepository repo)
        {
            _repo = repo;
            _logger= logger;
        }
        public async Task<bool> DeleteAsync(int productId)
        {
            return await _repo.DeleteAsync(productId).ConfigureAwait(false);
        }
        public async Task<(IEnumerable<ProductDTO> Items, int TotalCount)> SearchAsync(int categoryId, string SKU, int pageNumber, int pageSize)
        {
            var result= await _repo.SearchAsync(categoryId, SKU,pageNumber,pageSize).ConfigureAwait(false);
            /*return new PagedResult<ProductDTO>
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                Page = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize)
            };*/
            return result;
        }
        public async Task<ProductDTO?> GetByIdAsync(int productId)
        {
            return await _repo.GetByIdAsync(productId).ConfigureAwait(false);
        }
        public async Task<bool> InsertAsync(ProductDTO product)
        {
            if (await _repo.CheckSKUExistedAsync(null, product.SKU).ConfigureAwait(false))
                throw new BusinessException($"Could not insert because the SKU:{product.SKU} already existed.");
            return await _repo.InsertAsync(product).ConfigureAwait(false);
        }
        public async Task<bool> UpdateAsync(ProductDTO product)
        {
            if (await _repo.CheckSKUExistedAsync(product.Id, product.SKU).ConfigureAwait(false))
                throw new BusinessException($"Could not update because the SKU:{product.SKU} already existed.");
            return await _repo.UpdateAsync(product).ConfigureAwait(false);
        }

    }
}
