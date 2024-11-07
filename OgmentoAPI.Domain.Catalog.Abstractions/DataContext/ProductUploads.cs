
namespace OgmentoAPI.Domain.Catalog.Abstractions.DataContext
{
	public class ProductUploads
	{
		public int ProductUploadsId { get; set; }
		public string Product { get; set; }
		public string Sku { get; set; }
		public bool IsSuccess { get; set; }
		public string? ExceptionMessage { get; set; } = null;
	}
}
