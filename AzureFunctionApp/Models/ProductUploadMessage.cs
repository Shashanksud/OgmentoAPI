
namespace AzureFunctionApp.Models
{
	public class ProductUploadMessage
	{
		public List<UploadProductModel> Products {  get; set; }
		public Guid FileUploadUid {  get; set; }
	}
}
