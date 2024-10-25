namespace OgmentoAPI.Domain.Catalog.Abstractions.Models
{
	public class UploadCategoryModel
	{
		public string CategoryName { get; set; }
		public List<UploadCategoryModel> SubCategories { get; set; } = [];
	}
}
