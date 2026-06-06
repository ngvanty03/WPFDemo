using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ProductCategoryService> _logger;
        public ProductCategoryService(ILogger<ProductCategoryService> logger, IProductCategoryRepository repo)
        {
            _repo = repo;
            _logger = logger;
        }
        public async Task<IEnumerable<ProductCategoryDTO>> GetAllActiveAsync()
        {
            return await _repo.GetAllActiveAsync();
        }
    }
}
