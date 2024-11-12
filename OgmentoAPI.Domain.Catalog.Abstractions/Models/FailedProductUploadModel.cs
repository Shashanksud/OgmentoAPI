
namespace OgmentoAPI.Domain.Catalog.Abstractions.Models
{
	public class FailedProductUploadModel
	{
		public string Product { get; set; }
		public int RowNumber { get; set; }
		public string ExceptionMessage { get; set; }
	}
}
