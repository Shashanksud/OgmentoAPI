
using System.ComponentModel.DataAnnotations;

namespace OgmentoAPI.Domain.Catalog.Abstractions.DataContext
{
	public class FailedProductUploads
	{
		[Key]
		public int FailedUploadsId { get; set; }
		public string Product { get; set; }
		public int RowNumber {  get; set; }
		public string ExceptionMessage { get; set; }
	}
}
