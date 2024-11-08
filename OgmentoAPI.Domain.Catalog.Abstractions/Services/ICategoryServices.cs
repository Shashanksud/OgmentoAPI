using Microsoft.AspNetCore.Http;
using OgmentoAPI.Domain.Catalog.Abstractions.Models;
using OgmentoAPI.Domain.Common.Abstractions.Dto;

namespace OgmentoAPI.Domain.Catalog.Abstractions.Services
{
	public interface ICategoryServices
	{
		Task<int> GetCategoryId(Guid categoryUid);
		Task<List<CategoryModel>> GetAllCategories();
		Task<CategoryModel> GetCategory(Guid categoryUid);
		Task<ResponseDto> DeleteCategory(Guid categoryUid);
		Task<ResponseDto> UpdateCategory(Guid categoryUid, string categoryName);
		Task<ResponseDto> UploadCategories(IFormFile categoryJson);
		Task<CategoryModel> AddCategory(CategoryModel categoryModel);
		Guid GetCategoryUid(int? categoryId);
		Task<CategoryModel> GetCategoryForProduct(Guid categoryUid);
		string ValidateCategories(List<int> categoryIds);
	}
}
