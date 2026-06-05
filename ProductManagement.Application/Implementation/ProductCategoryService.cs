using Dapper;
using Microsoft.Data.SqlClient;
using ProductManagement.DTO;
using ProductManagement.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Application
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _repo;
        public ProductCategoryService(IProductCategoryRepository repo)
        {
            _repo = repo;
        }
        public async Task<IEnumerable<ProductCategoryDTO>> GetAllActiveAsync()
        {
            return await _repo.GetAllActiveAsync();
        }
    }
}
