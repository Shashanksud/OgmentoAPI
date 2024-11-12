using System.Security.Principal;

namespace OgmentoAPI.Domain.Catalog.Abstractions.DataContext
{
	public class ProductUploads
	{
		public int ProductUploadsId { get; set; }
		public Guid ProductUploadsGuid { get; set; } = Guid.NewGuid();
		public string Status { get; set; } = "Pending";
	}
}
