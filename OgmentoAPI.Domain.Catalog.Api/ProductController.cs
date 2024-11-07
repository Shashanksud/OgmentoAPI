using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OgmentoAPI.Domain.Catalog.Abstractions.Dto;
using OgmentoAPI.Domain.Catalog.Abstractions.Models;
using OgmentoAPI.Domain.Catalog.Abstractions.Services;
using OgmentoAPI.Domain.Common.Abstractions;

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
			var products = await _productServices.GetAllProducts();
			return Ok(products.ToDto());
		}

		[HttpGet]
		[Route("{sku}")]
		public async Task<IActionResult> GetProduct(string sku)
		{
			if (string.IsNullOrEmpty(sku))
				throw new InvalidDataException("sku cannot be null or empty.");
			var product = await _productServices.GetProduct(sku);
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
			var addedProduct = await _productServices.AddProduct(addProductDto.ToModel());
			return Ok(addedProduct.ToDto());
		}

		[HttpPost]
		[Route("csv")]
		public async Task<IActionResult> UploadProducts(IFormFile file)
		{
			if (file == null || file.Length == 0)
				throw new InvalidOperationException("The uploaded file is either null or empty. Please upload a valid CSV file.");
			return Ok(await _productServices.UploadProducts(file));
		}
		[HttpPost]
		[Route("uploadproduct")]
		public async Task<IActionResult> AddProduct(UploadProductModel product)
		{
			await _productServices.AddProduct(product);
			return Ok();
		}
		[HttpDelete]
		[Route("picture/{hash}")]
		public async Task<IActionResult> DeletePicture(string hash)
		{
			if (!string.IsNullOrEmpty(hash)) {
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
				return NotFound("Sample CSV file not found.");

			byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(sampleCsvPath);
			string fileName = Path.GetFileName(sampleCsvPath);
			return File(fileBytes, "text/csv", fileName);
		}
	}
}
