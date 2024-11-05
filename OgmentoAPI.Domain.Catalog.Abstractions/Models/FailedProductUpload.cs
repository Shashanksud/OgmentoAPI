
namespace OgmentoAPI.Domain.Catalog.Abstractions.Models
{
	public class FailedProductUpload
	{
		public string Sku { get; set; }
		public string ExceptionMessage { get; set; }
	}
}
