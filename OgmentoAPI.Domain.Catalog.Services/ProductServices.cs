using Azure.Storage.Queues.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using OgmentoAPI.Domain.Catalog.Abstractions.Dto;
using OgmentoAPI.Domain.Catalog.Abstractions.Models;
using OgmentoAPI.Domain.Catalog.Abstractions.Repository;
using OgmentoAPI.Domain.Catalog.Abstractions.Services;
using OgmentoAPI.Domain.Catalog.Services.Shared;
using OgmentoAPI.Domain.Common.Abstractions.Services;
using OgmentoAPI.Domain.Common.Services;
using System.Globalization;

namespace OgmentoAPI.Domain.Catalog.Services
{
	public class ProductServices: IProductServices
	{
		private readonly IProductRepository _productRepository;
		private readonly IPictureService _pictureServices;
		private readonly IAzureQueueService _azureQueueService;

		public ProductServices(IProductRepository productRepository, IPictureService pictureServices, IAzureQueueService azureQueueService)
		{
			_productRepository = productRepository;
			_pictureServices = pictureServices;
			_azureQueueService = azureQueueService;
		}
		public async Task<ProductModel> AddProduct(AddProductModel product)
		{
			await _productRepository.AddProduct(product);
			return (await _productRepository.GetProduct(product.SkuCode));
		}

		public async Task DeletePicture(string hash)
		{
			int? pictureId = await _pictureServices.GetPictureId(hash);
			await _productRepository.DeletePictureProductMapping(pictureId.Value);
			await _pictureServices.DeletePicture(hash);
		}

		public async Task DeleteProduct(string sku)
		{
			await _productRepository.DeleteProduct(sku);
		}
		public async Task<List<ProductModel>> GetAllProducts()
		{
			return await _productRepository.GetAllProducts();
		}
		public async Task<ProductModel> GetProduct(string sku)
		{
			return await _productRepository.GetProduct(sku);
		}
		public async Task UpdateProduct(AddProductModel product)
		{
			await _productRepository.UpdateProduct(product);
		}

		

		public async Task UploadPictures(IFormFile csvFile)
		{
			await CatalogHelper.UploadCsvFile<UploadPictureModel, UploadPictureModelMap>(csvFile, _productRepository.UploadPictures);
		}

		public async Task<List<FailedProductUpload>> UploadProducts(IFormFile csvFile)
		{
			await CatalogHelper.UploadCsvFile<UploadProductModel, UploadProductModelMap>(csvFile, _productRepository.UploadProducts);
			return await _productRepository.GetFailedUploads();
		}
		public async Task AddProduct(UploadProductModel product)
		{
			await _productRepository.AddProduct(product);
	}
		
}
}
