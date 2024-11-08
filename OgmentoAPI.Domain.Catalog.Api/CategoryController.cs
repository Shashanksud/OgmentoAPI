using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OgmentoAPI.Domain.Catalog.Abstractions.Dto;
using OgmentoAPI.Domain.Catalog.Abstractions.Services;
using Mapster;
using OgmentoAPI.Domain.Common.Abstractions;
using OgmentoAPI.Domain.Common.Abstractions.CustomExceptions;
using OgmentoAPI.Domain.Common.Abstractions.Dto;


namespace OgmentoAPI.Domain.Catalog.Api
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]

	public class CategoryController: ControllerBase
	{
		private readonly ICategoryServices _categoryServices;
		private readonly string _categorySampleJsonPath;
		public CategoryController(ICategoryServices categoryServices, IOptions<FilePaths> filePaths)
		{
			_categoryServices = categoryServices;
			_categorySampleJsonPath = filePaths.Value.CategorySampleJson;
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
			ResponseDto response = await _categoryServices.UpdateCategory(request.CategoryUid, request.CategoryName);
			if (response.IsSuccess)
			{
				return Ok(response);
			}
			else
			{
				return BadRequest(response);
			}
		}
		[HttpDelete]
		[Route("{categoryUid}")]
		public async Task<IActionResult> DeleteCategory(Guid categoryUid)
		{
			ResponseDto response = await _categoryServices.DeleteCategory(categoryUid);
			if (response.IsSuccess)
			{
				return Ok(response);
			}
			else
			{
				return BadRequest(response);
			}
		}
		[HttpPost]
		[Route("json")]
		public async Task<IActionResult> UploadCategories(IFormFile categoryJson)
		{
			if (categoryJson != null && categoryJson.Length > 0)
			{
				ResponseDto response = await _categoryServices.UploadCategories(categoryJson);
				if (response.IsSuccess)
				{
					return Ok(response);
				}
				else
				{
					return BadRequest(response);
				}
			}
			else
			{
				throw new ValidationException("The uploaded file is either null or empty. Please upload a valid JSON file.");
			}
		}
		[HttpPost]
		public async Task<IActionResult> AddCategory(CategoryDto categoryDto)
		{
			if (categoryDto == null || string.IsNullOrEmpty(categoryDto.CategoryName))
			{
				throw new ValidationException("Category name cannot be null or empty.");
			}
			return Ok((await _categoryServices.AddCategory(categoryDto.ToModel())).ToDto());
		}
		[HttpGet]
		[Route("sample")]
		public async Task<IActionResult> DownloadSampleCategory()
		{
			string sampleJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _categorySampleJsonPath);
			if (System.IO.File.Exists(sampleJsonPath))
			{
				byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(sampleJsonPath);
				string fileName = Path.GetFileName(sampleJsonPath);

				return File(fileBytes, "application/json", fileName);
			}
			else
			{
				throw new InvalidOperationException("JSON file not found.");
			}
		}
	}
}
