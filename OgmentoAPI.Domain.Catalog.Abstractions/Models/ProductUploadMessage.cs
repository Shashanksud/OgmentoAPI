

namespace OgmentoAPI.Domain.Catalog.Abstractions.Models
{
	public class ProductUploadMessage
	{
		public List<UploadProductModel> products { get; set; }
		public Guid FileUploadUid { get; set; }
	}
}
