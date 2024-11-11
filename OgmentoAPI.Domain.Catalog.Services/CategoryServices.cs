using Microsoft.AspNetCore.Http;
using OgmentoAPI.Domain.Catalog.Abstractions.DataContext;
using OgmentoAPI.Domain.Catalog.Abstractions.Models;
using OgmentoAPI.Domain.Catalog.Abstractions.Repository;
using OgmentoAPI.Domain.Catalog.Abstractions.Services;
using OgmentoAPI.Domain.Common.Abstractions.CustomExceptions;
using OgmentoAPI.Domain.Common.Abstractions.Dto;
using System.Text.Json;


namespace OgmentoAPI.Domain.Catalog.Services
{
	public class CategoryServices: ICategoryServices
	{
		private readonly ICategoryRepository _categoryRepository;
		public CategoryServices(ICategoryRepository categoryRepository)
		{
			_categoryRepository = categoryRepository;
		}
		public Guid GetCategoryUid(int? categoryId)
		{
			Guid? categoryUid = _categoryRepository.GetCategoryUid(categoryId);
			if (categoryUid == null)
			{
				throw new EntityNotFoundException("Category Not Found");
			}
			return categoryUid.Value;
		}
		public string ValidateCategories(List<int> categoryIds)
		{
			List<Category> categories = categoryIds.Select(x=>_categoryRepository.GetCategory(x).GetAwaiter().GetResult()).ToList();
			int parentCategoryCount = categories.Where(x => x.ParentCategoryId == 1).Count();
			if (parentCategoryCount != 1)
			{
				return "There should be only one main category.";
			}
			else
			{
				int parentCategoryId = categories.First(x => x.ParentCategoryId == 1).CategoryID;
				List<int> subCategoryIds = categories.Where(x => x.ParentCategoryId == parentCategoryId).Select(x=>x.CategoryID).ToList();
				List<int> subSubCategoryIds = new List<int>();
				foreach (int subCategoryId in subCategoryIds)
				{
					subSubCategoryIds.AddRange(categories.Where(x => x.ParentCategoryId == subCategoryId).Select(x => x.CategoryID).ToList());
				}
				if (subSubCategoryIds.Count + subCategoryIds.Count != categoryIds.Count - 1)
				{
					List<int> invalidCategoryIds = categoryIds.Where(x => x != parentCategoryId && !subCategoryIds.Contains(x) && !subSubCategoryIds.Contains(x)).ToList();
					return $"{invalidCategoryIds} are not subcategories of {parentCategoryId}";
				}
				return string.Empty;
			}
		}
		public async Task<int> GetCategoryId(Guid categoryUid)
		{
			int? categoryId = await _categoryRepository.GetCategoryIdAsync(categoryUid);
			if (categoryId == null)
			{
				throw new EntityNotFoundException($"Category with UID: {categoryUid} not found in database");
			}
			return categoryId.Value;
		}
		public async Task<List<CategoryModel>> GetSubCategories(int categoryId)
		{
			List<Category> subCategories = await _categoryRepository.GetSubCategories(categoryId);
			List<CategoryModel> catgories = subCategories.Select(x => new CategoryModel
			{
				CategoryName = x.CategoryName,
				CategoryId = x.CategoryID,
				CategoryUid = x.CategoryUid,
				ParentCategoryId = x.ParentCategoryId,
				ParentCategoryUid = GetCategoryUid(x.ParentCategoryId),
				SubCategories = _categoryRepository.CheckSubCategoriesExists(x.CategoryID)
										? GetSubCategories(x.CategoryID).GetAwaiter().GetResult()
										: [],
			}).ToList();
			return catgories;
		}
		public async Task<List<CategoryModel>> GetAllCategories()
		{
			List<Category> parentCategories = await _categoryRepository.GetSubCategories(1);
			List<CategoryModel> categories = parentCategories.Select(x => GetCategory(x.CategoryUid).GetAwaiter().GetResult()).ToList();
			return categories;
		}
		public async Task<CategoryModel> GetCategory(Guid categoryUid)
		{
			int categoryId = await GetCategoryId(categoryUid);
			Category category = await _categoryRepository.GetCategory(categoryId);
			return new CategoryModel
			{
				CategoryId = category.CategoryID,
				CategoryName = category.CategoryName,
				CategoryUid = category.CategoryUid,
				ParentCategoryId = category.ParentCategoryId,
				ParentCategoryUid = GetCategoryUid(category.ParentCategoryId),
				SubCategories = GetSubCategories(categoryId).GetAwaiter().GetResult()
			};
		}
		
		public async Task<ResponseDto> DeleteCategory(Guid categoryUid)
		{
			int categoryId = await GetCategoryId(categoryUid);
			List<int> categoryIds = new List<int> { categoryId };
			if (!_categoryRepository.IsSafeDelete(categoryId))
			{
				throw new ValidationException($"Category:{categoryUid} cannot be deleted as it is mapped with a product");
			}
			if (_categoryRepository.CheckSubCategoriesExists(categoryId))
			{
				List<int> subCategoryIds = (await _categoryRepository.GetSubCategories(categoryId)).Select(x => x.CategoryID).ToList();
				categoryIds.AddRange(subCategoryIds);
				foreach (int subCategoryId in subCategoryIds)
				{
					if (_categoryRepository.CheckSubCategoriesExists(subCategoryId))
					{
						List<int> subSubCategoryIds = (await _categoryRepository.GetSubCategories(subCategoryId)).Select(x => x.CategoryID).ToList();
						categoryIds.AddRange(subSubCategoryIds);
					}
				}
			}
			int rowsAffected = await _categoryRepository.DeleteCategories(categoryIds);
			return new ResponseDto
			{
				IsSuccess = (rowsAffected > 0),
				ErrorMessage = (rowsAffected == 0) ? "Zero rows Updated" : "No Error"
			};
		}
		public async Task<ResponseDto> UpdateCategory(Guid categoryUid, string categoryName)
		{
			int categoryId = await GetCategoryId(categoryUid);
			Category category = await _categoryRepository.GetCategory(categoryId);
			category.CategoryName = categoryName;
			int rowsAffected = await _categoryRepository.UpdateCategory(category);
			return new ResponseDto
			{
				IsSuccess = (rowsAffected > 0),
				ErrorMessage = (rowsAffected == 0) ? "Zero rows Updated" : "No Error"
			};
		}
		private async Task<ResponseDto> AddSubCategories(List<UploadCategoryModel> subCategories, int parentId)
		{
			bool success = false;
			foreach (UploadCategoryModel subCategory in subCategories)
			{
				if (_categoryRepository.CategoryAlreadyExists(subCategory.CategoryName))
				{
					throw new ValidationException($"{subCategory.CategoryName} Already Exists.");
				}
				(Category addedCategory, int rowsAffected) = await _categoryRepository.AddCategory(subCategory.CategoryName, parentId);
				if (rowsAffected == 0)
				{
					throw new DatabaseOperationException($"Unable to add category {subCategory.CategoryName} ");
				}
				int categoryId = addedCategory.CategoryID;
				if (subCategory.SubCategories.Count > 0)
				{
					success = (rowsAffected > 0) || (await AddSubCategories(subCategory.SubCategories, categoryId)).IsSuccess;
				}
			}
			return new ResponseDto
			{
				IsSuccess = success,
				ErrorMessage = (success)? "No Error":"Error in adding categories"
			};

		}
		public async Task<ResponseDto> UploadCategories(IFormFile categoryJson)
		{
			bool success = false;
			string categoryJsonContent;
			using (var reader = new StreamReader(categoryJson.OpenReadStream()))
			{
				categoryJsonContent = await reader.ReadToEndAsync();
			}

			List<UploadCategoryModel>? categories = JsonSerializer.Deserialize<List<UploadCategoryModel>>(categoryJsonContent, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			});

			if (categories == null)
			{
				throw new ValidationException("Json Deserialization failed, categories list is null.");
			}
			foreach (UploadCategoryModel category in categories)
			{
				if (_categoryRepository.CategoryAlreadyExists(category.CategoryName))
				{
					throw new ValidationException($"{category.CategoryName} Already Exists.");
				}
				(Category addedCategory, int rowsAffected) = await _categoryRepository.AddCategory(category.CategoryName, 1);
				if (rowsAffected == 0)
				{
					throw new DatabaseOperationException($"Unable to add category {category.CategoryName} ");
				}
				int categoryId = addedCategory.CategoryID;

				if (category.SubCategories.Count > 0)
				{
					success = (rowsAffected > 0) || (await AddSubCategories(category.SubCategories, categoryId)).IsSuccess;
				}
			}
			return new ResponseDto
			{
				IsSuccess = success,
				ErrorMessage = (success) ? "No Error" : "Error in adding categories"
			};
		}
		public async Task<CategoryModel> AddCategory(CategoryModel categoryModel)
		{

			if (_categoryRepository.CategoryAlreadyExists(categoryModel.CategoryName))
			{
				throw new ValidationException($"{categoryModel.CategoryName} Already Exists.");
			}
			if (categoryModel.ParentCategoryUid == Guid.Empty)
			{
				categoryModel.ParentCategoryId = 1;
				categoryModel.ParentCategoryUid = GetCategoryUid(categoryModel.ParentCategoryId);
			}
			else
			{
				categoryModel.ParentCategoryId = await GetCategoryId(categoryModel.ParentCategoryUid);
			}
			(Category category, int rowsAffected) = await _categoryRepository.AddCategory(categoryModel.CategoryName, categoryModel.ParentCategoryId.Value);
			categoryModel.CategoryUid = category.CategoryUid;
			categoryModel.CategoryId = category.CategoryID;
			if (rowsAffected == 0)
			{
				throw new DatabaseOperationException($"Category {categoryModel.CategoryName} could not be Added.");
			}
			return categoryModel;

		}
		public async Task<CategoryModel> GetCategoryForProduct(Guid categoryUid)
		{
			int categoryId = await GetCategoryId(categoryUid);
			Category category = await _categoryRepository.GetCategory(categoryId);
			return new CategoryModel()
			{
				CategoryId = category.CategoryID,
				CategoryName = category.CategoryName,
				CategoryUid = category.CategoryUid,
				ParentCategoryId = category.ParentCategoryId,
				ParentCategoryUid = GetCategoryUid(category.ParentCategoryId),
				SubCategories = [],
			};
		}
	}
}
