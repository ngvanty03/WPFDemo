using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ProductManagement.Application;
using ProductManagement.DTO;
using System.Text;
namespace ProductManagement.Infrastructure
{
    public class ProductRepository : IProductRepository
    {
        private readonly DatabaseOptions _DBOptions;
        private readonly ILogger<ProductRepository> _logger;
        public ProductRepository(ILogger<ProductRepository> logger, DatabaseOptions DBOptions)
        {
            _DBOptions = DBOptions;
            _logger = logger;
        }
        /// <summary>
        /// Insert a new product to DB
        /// </summary>
        /// <param name="product">The input parameter</param>
        /// <returns>true/false</returns>
        public async Task<bool> InsertAsync(ProductDTO product)
        {
            var sql = @"INSERT INTO [Product]([CategoryId],[SKU],[Name],[IsActive],[Description])
                        VALUES(@CategoryId,@SKU,@Name,@IsActive,@Description)";
            using (var connection = new SqlConnection(_DBOptions.DBConnectionString))
            {
                await connection.OpenAsync();
                var result = await connection.ExecuteAsync(sql,
                    new
                    {
                        CategoryId = product.CategoryId,
                        SKU = product.SKU,
                        Name = product.Name,
                        IsActive = product.IsActive,
                        Description = product.Description
                    });
                return result > 0;
            }
        }
        /// <summary>
        /// Update product information
        /// </summary>
        /// <param name="product">The product need to update</param>
        /// <returns>true/false</returns>
        public async Task<bool> UpdateAsync(ProductDTO product)
        {
            var sql = @"Update Product set [CategoryId]=@CategoryId,[SKU]=@SKU,[Name]=@Name,
                        [IsActive]=@IsActive,[Description]=@Description where Id=@Id";
            using (var connection = new SqlConnection(_DBOptions.DBConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                var result = await connection.ExecuteAsync(sql,
                    new
                    {
                        CategoryId = product.CategoryId,
                        SKU = product.SKU,
                        Name = product.Name,
                        IsActive = product.IsActive,
                        Description = product.Description,
                        Id = product.Id
                    }).ConfigureAwait(false);
                return result > 0;
            }
        }
        /// <summary>
        /// Delete a product out of DB
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>true/false</returns>
        public async Task<bool> DeleteAsync(int productId)
        {
            var sql = @"Delete from Product where Id=@Id";
            using (var connection = new SqlConnection(_DBOptions.DBConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                var result = await connection.ExecuteAsync(sql,
                    new
                    {
                        Id = productId
                    }).ConfigureAwait(false);
                return result > 0;
            }
        }
        /// <summary>
        /// get product by Id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>an Product object</returns>
        public async Task<ProductDTO?> GetByIdAsync(int productId)
        {
            var sql = "Select Id,CategoryId,SKU,Name,IsActive,Description from Product where Id=@Id";
            using (var connection = new SqlConnection(_DBOptions.DBConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                var result = await connection.QueryFirstOrDefaultAsync<ProductDTO>(sql, new { Id = productId }).ConfigureAwait(false);
                if (result != null)
                    return result;
            }
            return null;
        }

        /// <summary>
        /// Get all product in DB
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="SKU"></param>
        /// <returns></returns>
        public async Task<(IEnumerable<ProductDTO> Items, int TotalCount)> SearchAsync(int categoryId, string SKU, int pageNumber, int pageSize, string sortColumn, string sortDirection)
        {
            if (pageNumber < 1)
                pageNumber = 1;
            int offset = (pageNumber - 1) * pageSize;
            var parameters = new DynamicParameters();
            var where = new StringBuilder("WHERE 1=1");
            if (categoryId > 0)
            {
                where.Append(" and CategoryId=@CategoryId");
                parameters.Add("CategoryId", categoryId);
            }
            if (!string.IsNullOrEmpty(SKU))
            {
                where.Append(" and SKU LIKE @SKU");
                parameters.Add("SKU", $"%{SKU}%");
            }
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);
            var sql = $@"SELECT COUNT(*)  FROM Product {where};
                         SELECT Id,SKU,Name,IsActive from Product 
                         {where}
                         ORDER BY {sortColumn} {sortDirection}
                         OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            using (var connection = new SqlConnection(_DBOptions.DBConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                var dbResult= await connection.QueryMultipleAsync(sql, parameters).ConfigureAwait(false);
                var totalCount = await dbResult.ReadFirstAsync<int>().ConfigureAwait(false);
                var items = await dbResult.ReadAsync<ProductDTO>().ConfigureAwait(false);
                return (items,totalCount);
            }
        }
        public async Task<bool> CheckSKUExistedAsync(int? productId, string SKU)
        {
            var parameters = new DynamicParameters();
            parameters.Add("SKU", SKU);
            var sql = "Select 1 from Product where SKU=@SKU";
            if (productId != null)
            {
                //edit case
                sql += " and Id<>@Id";
                parameters.Add("Id", productId);
            }
            using (var connection = new SqlConnection(_DBOptions.DBConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                var result = await connection.ExecuteScalarAsync<int>(sql, parameters).ConfigureAwait(false);
                return result > 0;
            }
        }        
    }
}
