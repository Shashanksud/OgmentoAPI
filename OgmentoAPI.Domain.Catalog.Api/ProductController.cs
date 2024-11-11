using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using OgmentoAPI.Domain.Catalog.Abstractions.Dto;
using OgmentoAPI.Domain.Catalog.Abstractions.Models;
using OgmentoAPI.Domain.Catalog.Abstractions.Services;
using OgmentoAPI.Domain.Catalog.Services.Shared;
using OgmentoAPI.Domain.Common.Abstractions;
using OgmentoAPI.Domain.Common.Abstractions.CustomExceptions;

namespace OgmentoAPI.Domain.Catalog.Api
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class ProductController : ControllerBase
	{
		private readonly IProductServices _productServices;
		private readonly string sampleCsvRelativePath;
		
		public ProductController(IProductServices productServices, IOptions<FilePaths> filePaths)
		{
			sampleCsvRelativePath = filePaths.Value.ProductSampleCsv;
			_productServices = productServices;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllProducts()
		{
			List<ProductModel> products = await _productServices.GetAllProducts();
			return Ok(products.ToDto());
		}

		[HttpGet]
		[Route("{sku}")]
		public async Task<IActionResult> GetProduct(string sku)
		{
			if (string.IsNullOrEmpty(sku))
			{
				throw new InvalidDataException("sku cannot be null or empty.");
			}
			ProductModel product = await _productServices.GetProduct(sku);
			return Ok(product.ToDto());
		}
		[HttpPut]
		public async Task<IActionResult> UpdateProduct(AddProductDto productDto)
		{
			await _productServices.UpdateProduct(productDto.ToModel());
			return Ok();
		}

		[HttpDelete]
		[Route("{sku}")]
		public async Task<IActionResult> DeleteProduct(string sku)
		{
			if (string.IsNullOrEmpty(sku)) {
				throw new InvalidDataException("sku cannot be null or empty.");
			}
			await _productServices.DeleteProduct(sku);
			return Ok();
		}

		[HttpPost]
		public async Task<IActionResult> AddProduct(AddProductDto addProductDto)
		{
			await _productServices.AddProduct(addProductDto.ToModel());
			return Ok(await _productServices.GetProduct(addProductDto.SkuCode));
		}

		[HttpPost]
		[Route("csv")]
		public async Task<IActionResult> UploadProducts(IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				throw new InvalidOperationException("The uploaded file is either null or empty. Please upload a valid CSV file.");
			}
			try
			{
				List<UploadProductModel> products = CatalogHelper.UploadCsvFile<UploadProductModel, UploadProductModelMap>(file);
				_ = _productServices.UploadProducts(products);
			}
			catch(InvalidDataException ex)
			{
				throw new ValidationException($"{ex}");
			}
			return Ok();
		}
		[HttpPost]
		[Route("uploadproduct")]
		public async Task<IActionResult> SaveProductUpload(UploadProductModel product)
		{
			await _productServices.SaveProductUpload(product);
			return Ok();
		}
		[HttpDelete]
		[Route("picture/{hash}")]
		public async Task<IActionResult> DeletePicture(string hash)
		{
			if (string.IsNullOrEmpty(hash)) {
				throw new InvalidOperationException("Hash cannot be null or empty");
			}
			await _productServices.DeletePicture(hash);
			return Ok();

		}

		[HttpPost]
		[Route("picture/csv")]
		public async Task<IActionResult> UploadPictures(IFormFile file)
		{
			if (file == null || file.Length == 0)
				throw new InvalidOperationException("The uploaded file is either null or empty. Please upload a valid CSV file.");

			await _productServices.UploadPictures(file);
			return Ok();
		}

		[HttpGet]
		[Route("sample")]
		public async Task<IActionResult> DownloadCsvProductSample()
		{
			string sampleCsvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sampleCsvRelativePath);
			if (!System.IO.File.Exists(sampleCsvPath))
			{
				return NotFound("Sample CSV file not found.");
			}
			byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(sampleCsvPath);
			string fileName = Path.GetFileName(sampleCsvPath);
			return File(fileBytes, "text/csv", fileName);
		}
		[HttpGet]
		[Route("validate/{sku}")]
		public async Task<IActionResult> ValidateSku(string sku)
		{
			if (string.IsNullOrEmpty(sku))
			{
				throw new InvalidOperationException("sku cannot be null or empty");
			}
			return Ok(await _productServices.IsSkuExists(sku));
		}
		[HttpGet]
		[Route("upload/failed")]
		public async Task<IActionResult> GetFailedUploads()
		{
			return Ok(await _productServices.FailedProductUploads());
		}
	}
}
