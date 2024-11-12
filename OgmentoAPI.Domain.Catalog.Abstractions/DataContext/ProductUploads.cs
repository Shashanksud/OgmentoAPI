using System.Security.Principal;

namespace OgmentoAPI.Domain.Catalog.Abstractions.DataContext
{
	public class ProductUploads
	{
		public int ProductUploadsId { get; set; }
		public string FileName {  get; set; }
		public Guid ProductUploadsGuid { get; set; } = Guid.NewGuid();
		public string Status { get; set; } = "Pending";
		public DateTime CreatedOn { get; set; } = DateTime.Now;
	}
}
