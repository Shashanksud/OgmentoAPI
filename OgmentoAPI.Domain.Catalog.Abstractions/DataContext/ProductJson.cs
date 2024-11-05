
namespace OgmentoAPI.Domain.Catalog.Abstractions.DataContext
{
	public class ProductJson
	{
		public int ProductJsonId { get; set; }
		public string Product { get; set; }
		public string Sku { get; set; }
		public bool IsSuccess { get; set; }
		public string? ExceptionMessage { get; set; } = null;
	}
}
