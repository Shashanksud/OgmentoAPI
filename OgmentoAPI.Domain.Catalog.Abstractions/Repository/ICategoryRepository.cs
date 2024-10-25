using OgmentoAPI.Domain.Catalog.Abstractions.DataContext;
using OgmentoAPI.Domain.Catalog.Abstractions.Models;

namespace OgmentoAPI.Domain.Catalog.Abstractions.Repository
{
	public interface ICategoryRepository
	{
		Task<int?> GetCategoryIdAsync(Guid categoryUid);
		Task<List<Category>> GetSubCategories(int categoryId);
		Task<Category> GetCategory(int categoryId);
		Task<int> DeleteCategories(List<int> categoryIds);
		Task<int> UpdateCategory(Category category);
		Task<(Category,int)> AddCategory(string categoryName, int parentCategoryId);
		Guid? GetCategoryUid(int? categoryId);
		bool CheckSubCategoriesExists(int categoryId);
		bool IsSafeDelete(int categoryId);
		bool CategoryAlreadyExists(string categoryName);
	}
}
