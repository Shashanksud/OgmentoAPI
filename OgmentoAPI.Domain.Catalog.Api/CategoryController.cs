using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OgmentoAPI.Domain.Catalog.Abstractions.Dto;
using OgmentoAPI.Domain.Catalog.Abstractions.Services;
using Mapster;


namespace OgmentoAPI.Domain.Catalog.Api
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]

	public class CategoryController: ControllerBase
	{
		private readonly ICategoryServices _categoryServices;
		public CategoryController(ICategoryServices categoryServices)
		{
			_categoryServices = categoryServices;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllCategories()
		{
			var categories = await _categoryServices.GetAllCategories();
			return Ok(categories.Select(category => category.ToDto()).ToList());
		}

		[HttpGet("{categoryUid}")]
		public async Task<IActionResult> GetCategory(Guid categoryUid)
		{
			var category = await _categoryServices.GetCategory(categoryUid);
			return Ok(category.ToDto());
		}

		[HttpPut]
		public async Task<IActionResult> UpdateCategory(UpdateCategoryDto request)
		{
			await _categoryServices.UpdateCategory(request.CategoryUid, request.CategoryName);
			return Ok();
		}

		[HttpDelete]
		[Route("{categoryUid}")]
		public async Task<IActionResult> DeleteCategory(Guid categoryUid)
		{
			await _categoryServices.DeleteCategory(categoryUid);
			return Ok();
		}

		[HttpPost]
		[Route("upload")]
		public async Task<IActionResult> AddCategories(List<CategoryDto> categories)
		{
			var addedCategories = await _categoryServices.AddCategories(categories.ToModel());
			return Ok(addedCategories.ToDto());
		}
		
		[HttpPost]
		public async Task<IActionResult> AddNewCategory(CategoryDto categoryDto)
		{
			var addedCategory = await _categoryServices.AddNewCategory(categoryDto.ToModel());
			return Ok(addedCategory.ToDto());
		}
	}
}
