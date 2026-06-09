
namespace ProductManagement.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
    public class ProductListDTO
    {
        public int Id { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
