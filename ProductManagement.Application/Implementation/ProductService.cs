using ProductManagement.Application.Exceptions;
using ProductManagement.DTO;
using ProductManagement.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Application
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }
        public async Task<bool> DeleteAsync(int productId)
        {
            return await _repo.DeleteAsync(productId).ConfigureAwait(false);
        }
        public async Task<IEnumerable<ProductDTO>> GetAllAsync(int categoryId, string SKU)
        {
            return await _repo.GetAllAsync(categoryId, SKU).ConfigureAwait(false);
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
