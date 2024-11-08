using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OgmentoAPI.Domain.Catalog.Abstractions.DataContext;
using OgmentoAPI.Domain.Catalog.Abstractions.Repository;
using OgmentoAPI.Domain.Common.Abstractions.CustomExceptions;

namespace OgmentoAPI.Domain.Catalog.Infrastructure.Repository
{
	public class CategoryRepository: ICategoryRepository
	{
		private readonly CatalogDbContext _dbContext;
		
		public CategoryRepository(CatalogDbContext dbContext)
		{
			_dbContext = dbContext;
		}
		
		public async Task<int?> GetCategoryIdAsync(Guid categoryUid)
		{
			return (await _dbContext.Category.FirstOrDefaultAsync(x => x.CategoryUid == categoryUid))?.CategoryID;
		}
		public Guid? GetCategoryUid(int? categoryId)
		{
			if (categoryId == null)
			{
				return _dbContext.Category.AsNoTracking().Single(x => x.CategoryName == "All Products")?.CategoryUid;
			}
			return  _dbContext.Category.AsNoTracking().Single(x => x.CategoryID == categoryId)?.CategoryUid;
		}
		public async Task<List<Category>> GetSubCategories(int categoryId)
		{
			return await _dbContext.Category.AsNoTracking().Where(x => x.ParentCategoryId == categoryId).ToListAsync();
		}
		
		public bool CheckSubCategoriesExists(int categoryId)
		{
			return _dbContext.Category.Any(x => x.ParentCategoryId == categoryId);
		}
		public async Task<Category> GetCategory(int categoryId)
		{
			return await _dbContext.Category.SingleAsync(x => x.CategoryID == categoryId);
		}
		public bool IsSafeDelete(int categoryId)
		{
			return !_dbContext.ProductCategoryMapping.Any(x => x.CategoryId == categoryId);
		}
		public async Task<int> DeleteCategories(List<int> categoryIds)
		{
			try
			{
				return await _dbContext.Category.Where(x => categoryIds.Contains(x.CategoryID)).ExecuteDeleteAsync();
			}
			catch (DbUpdateException ex)
			{
				throw new DatabaseOperationException($"Error occurred while deleting category: {ex}");
			}
		}
		public async Task<int> UpdateCategory(Category category)
		{
			try
			{
				_dbContext.Category.Update(category);
				return await _dbContext.SaveChangesAsync();
			}
			catch (DbUpdateException ex)
			{
				throw new DatabaseOperationException($"Error occurred while Updating category: {ex}");
			}
		}
		public async Task<(Category,int)> AddCategory(string categoryName, int parentCategoryId)
		{
			try
			{
				Category category = new Category()
				{
					CategoryName = categoryName,
					ParentCategoryId = parentCategoryId,
					CategoryUid = Guid.NewGuid()
				};
				EntityEntry<Category> entity = _dbContext.Category.Add(category);
				int rowsAffected = await _dbContext.SaveChangesAsync();
				return (entity.Entity, rowsAffected);
			}
			catch (DbUpdateException ex)
			{
				throw new DatabaseOperationException($"Error occurred while Adding category: {ex}");
			}
		}
		public bool CategoryAlreadyExists(string categoryName) {
			return _dbContext.Category.Any(x => x.CategoryName.ToLower() == categoryName.ToLower());
		}
	}
}

