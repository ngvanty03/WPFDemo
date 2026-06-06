using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ProductManagement.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Infrastructure
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly DatabaseOptions _DBOptions;
        private readonly ILogger<ProductCategoryRepository> _logger;
        public ProductCategoryRepository(ILogger<ProductCategoryRepository> logger,DatabaseOptions DBOptions) { 
            _DBOptions = DBOptions;
            _logger = logger;
        }
        public async Task<IEnumerable<ProductCategoryDTO>> GetAllActiveAsync()
        {
            var parameters = new DynamicParameters();
            var sql = "Select Id,Name from ProductCategory where IsActive=1";
            using (var connection = new SqlConnection(_DBOptions.DBConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                return await connection.QueryAsync<ProductCategoryDTO>(sql, parameters).ConfigureAwait(false);
            }
        }
    }
}
