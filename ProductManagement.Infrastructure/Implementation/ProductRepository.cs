using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using ProductManagement.DTO;
using Microsoft.Data.SqlClient;
namespace ProductManagement.Infrastructure
{
    public class ProductRepository : IProductRepository
    {
        private readonly DatabaseOptions _DBOptions;
        public ProductRepository(DatabaseOptions DBOptions)
        {
            _DBOptions = DBOptions;
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
                await connection.OpenAsync();
                var result = await connection.ExecuteAsync(sql,
                    new
                    {
                        CategoryId = product.CategoryId,
                        SKU = product.SKU,
                        Name = product.Name,
                        IsActive = product.IsActive,
                        Description = product.Description,
                        Id = product.Id
                    });
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
                await connection.OpenAsync();
                var result = await connection.ExecuteAsync(sql,
                    new
                    {
                        Id = productId
                    });
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
                await connection.OpenAsync();
                var result = await connection.QueryFirstOrDefaultAsync<ProductDTO>(sql, new { Id = productId });
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
        public async Task<IEnumerable<ProductDTO>> GetAllAsync(int categoryId, string SKU)
        {
            var parameters = new DynamicParameters();
            var sql = "Select Id,SKU,Name,IsActive from Product where 1=1";
            if (categoryId > 0)
            {
                sql += " and CategoryId=@CategoryId";
                parameters.Add("CategoryId", categoryId);
            }
            if (!string.IsNullOrEmpty(SKU))
            {
                sql += " and SKU=@SKU";
                parameters.Add("SKU", SKU);
            }
            using (var connection = new SqlConnection(_DBOptions.DBConnectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<ProductDTO>(sql, parameters);
            }
        }
        public async Task<bool> CheckSKUExistedAsync(int? productId,string SKU)
        {
            var parameters = new DynamicParameters();
            parameters.Add("SKU", SKU);
            var sql = "Select 1 from Product where SKU=@SKU";
            if (productId!=null) {
                //edit case
                sql += " and Id<>@Id";
                parameters.Add("Id", productId);
            }
            using (var connection = new SqlConnection(_DBOptions.DBConnectionString))
            {
                await connection.OpenAsync();
                var result = await connection.ExecuteScalarAsync<int>(sql, parameters);
                return result > 0;
            }
        }
    }
}
