using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OgmentoAPI.Domain.Catalog.Abstractions.DataContext;
using OgmentoAPI.Domain.Catalog.Abstractions.Dto;
using OgmentoAPI.Domain.Catalog.Abstractions.Models;
using OgmentoAPI.Domain.Catalog.Abstractions.Repository;
using OgmentoAPI.Domain.Catalog.Abstractions.Services;
using OgmentoAPI.Domain.Catalog.Services.Shared;
using OgmentoAPI.Domain.Common.Abstractions.CustomExceptions;
using OgmentoAPI.Domain.Common.Abstractions.Dto;
using OgmentoAPI.Domain.Common.Abstractions.Models;
using OgmentoAPI.Domain.Common.Abstractions.Services;
using OgmentoAPI.Domain.Common.Abstractions.CustomExceptions;
using System.Globalization;

namespace OgmentoAPI.Domain.Catalog.Services
{
	public class ProductServices: IProductServices
	{
		private readonly IProductRepository _productRepository;
		private readonly IPictureService _pictureServices;
		private readonly IAzureQueueService _azureQueueService;
		private readonly ICategoryServices _categoryServices;
		public ProductServices(IProductRepository productRepository, ICategoryServices categoryServices, IPictureService pictureServices, IAzureQueueService azureQueueService)
		{
			_productRepository = productRepository;
			_pictureServices = pictureServices;
			_azureQueueService = azureQueueService;
			_categoryServices = categoryServices;
		}
		private async Task<List<PictureModel>> GetImages(int productId)
		{
			List<int> pictureIds = await _productRepository.GetImages(productId);
			List<PictureModel> pictureModels = await _pictureServices.GetPictures(pictureIds);
			return pictureModels;
		}
		private async Task<CategoryModel> GetCategory(int productId)
		{
			List<int> productCategoryIds = await _productRepository.GetCategory(productId);
			List<Guid> productCategoryUids = productCategoryIds.Select(x => _categoryServices.GetCategoryUid(x)).ToList();
			List<CategoryModel> productCategories = new List<CategoryModel>();
			foreach (Guid productCategoryUid in productCategoryUids)
			{
				productCategories.Add(await _categoryServices.GetCategoryForProduct(productCategoryUid));
			}
			CategoryModel category = productCategories.Single(x => x.ParentCategoryId == 1);
			category.ParentCategoryUid = new Guid();
			category.SubCategories = productCategories.Where(x => x.ParentCategoryId == category.CategoryId).ToList();
			foreach (CategoryModel subCategory in category.SubCategories)
			{
				subCategory.SubCategories = productCategories.Where(x => x.ParentCategoryId == subCategory.CategoryId).ToList();
			}
			return category;
		}
		public async Task<ProductModel> GetProduct(string sku)
		{
			Product? product = await _productRepository.GetProduct(sku);
			if (product == null)
			{
				throw new EntityNotFoundException($"Product {sku} not found.");
			}
			ProductModel productModel = new ProductModel()
			{
				ProductId = product.ProductID,
				ProductName = product.ProductName,
				SkuCode = sku,
				Images = GetImages(product.ProductID).GetAwaiter().GetResult(),
				Category = GetCategory(product.ProductID).GetAwaiter().GetResult(),
				Price = product.Price,
				ExpiryDate = product.ExpiryDate,
				ProductDescription = product.ProductDescription,
				Weight = product.Weight,
				LoyaltyPoints = product.LoyaltyPoints ?? 0,

			};
			return productModel;
		}
		public async Task<List<ProductModel>> GetAllProducts()
		{
			List<Product> products = await _productRepository.GetAllProducts();
			List<ProductModel> productModel = products.Select(x => new ProductModel
			{
				ProductId = x.ProductID,
				ProductName = x.ProductName,
				SkuCode = x.SkuCode,
				LoyaltyPoints = x.LoyaltyPoints ?? 0,
				Price = x.Price,
				ExpiryDate = x.ExpiryDate,
				ProductDescription = x.ProductDescription,
				Weight = x.Weight,
				Images = GetImages(x.ProductID).GetAwaiter().GetResult(),
				Category = GetCategory(x.ProductID).GetAwaiter().GetResult(),
			}).ToList();
			return productModel;
		}
		public async Task<bool> IsSkuExists(string sku)
		{
			return await _productRepository.IsSkuExists(sku);
		}
		private async Task AddProductCategoryMapping(List<int> CategoryIds, int productId)
		{
			List<ProductCategoryMapping> productCategories = CategoryIds.Select(x => new ProductCategoryMapping
			{
				ProductId = productId,
				CategoryId = x
			}).ToList();
			int rowsAffected = await _productRepository.AddProductCategoryMapping(productCategories);
			if (rowsAffected == 0)
			{
				throw new DatabaseOperationException($"Unable to Add the Product Category mapping.");
			}
		}
		private async Task AddProductImageMapping(List<PictureModel> pictures, int productId)
		{
			List<ProductImageMapping> productImageMappings = new List<ProductImageMapping>();
			foreach (PictureModel picture in pictures)
			{

				PictureModel pictureModel = await _pictureServices.AddPicture(picture);
				ProductImageMapping productImageMapping = new ProductImageMapping()
				{
					ProductId = productId,
					ImageId = pictureModel.PictureId
				};
				productImageMappings.Add(productImageMapping);
			}
			int rowsAffected = await _productRepository.AddProductImageMapping(productImageMappings);
			if (rowsAffected == 0)
			{
				throw new DatabaseOperationException($"Unable to Add the Product Category mapping.");
			}
		}
		public async Task<ResponseDto> AddProduct(AddProductModel productModel)
		{
			if (await IsSkuExists(productModel.SkuCode))
			{
				throw new ValidationException($"Product with skucode: {productModel.SkuCode} already exists. Please give different code.");
			}
		public async Task<ProductBase> GetProduct(int productId)
		{
			return await _productRepository.GetProduct(productId);
		}

			Product product = new Product()
			{
				SkuCode = productModel.SkuCode,
				Price = productModel.Price,
				ProductDescription = productModel.ProductDescription,
				ProductName = productModel.ProductName,
				LoyaltyPoints = productModel.LoyaltyPoints,
				ExpiryDate = productModel.ExpiryDate,
				Weight = productModel.Weight,
			};
			int rowsAffected = await _productRepository.AddProduct(product);
			if (rowsAffected == 0)
			{
				throw new DatabaseOperationException($"unable to add Product {productModel.SkuCode}.");
			}
			ProductModel productAdded = await GetProduct(productModel.SkuCode);

			if (productModel.Categories.Count != 0)
			{
				List<int> categoryIds = new List<int>();
				foreach (Guid categoryUid in productModel.Categories)
				{
					categoryIds.Add(await _categoryServices.GetCategoryId(categoryUid));
				}
				await AddProductCategoryMapping(categoryIds, productAdded.ProductId);
			}
			if (productModel.Images.Count != 0)
			{
				await AddProductImageMapping(productModel.Images, productAdded.ProductId);
			}
			return new ResponseDto
			{
				IsSuccess = (rowsAffected > 0),
				ErrorMessage = (rowsAffected == 0) ? "Zero rows Updated" : "No Error"
			};
		}
		public async Task SaveProductUpload(UploadProductModel product)
		{
			if (await IsSkuExists(product.SkuCode))
			{
				ProductUploads productUploads = await _productRepository.GetProductUploads(product.SkuCode);
				if (productUploads.IsSuccess)
				{
					productUploads.IsSuccess = false;
					productUploads.ExceptionMessage = $"Product {product.SkuCode} already exists in the database.";
				}
				else
				{
					productUploads.ExceptionMessage += $"Product {product.SkuCode} already exists in the database.";
				}
				await _productRepository.UpdateProductUploads(productUploads);
			}
			else
			{
				AddProductModel productModel = new AddProductModel
				{
					Price = product.Price,
					ExpiryDate = product.ExpiryDate,
					ProductDescription = product.ProductDescription,
					ProductName = product.ProductName,
					SkuCode = product.SkuCode,
					LoyaltyPoints = product.LoyaltyPoints,
					Weight = product.Weight,
				};
				await AddProduct(productModel);
				int? productId = (await _productRepository.GetProduct(product.SkuCode))?.ProductID;
				if (productId == null)
				{
					throw new DatabaseOperationException("Product did not get added in the database.");
				}
				await AddProductCategoryMapping(product.CategoryIds, productId.Value);
			}
		}
		public async Task<ResponseDto> DeletePicture(string hash)
		{
			int? pictureId = await _pictureServices.GetPictureId(hash);
			int rowsAffected = await _productRepository.DeletePictureProductMapping(pictureId.Value);
			await _pictureServices.DeletePicture(hash);
			return new ResponseDto
			{
				IsSuccess = (rowsAffected > 0),
				ErrorMessage = (rowsAffected == 0) ? "Zero rows deleted" : "No Error"
			};
		}

		public async Task<ResponseDto> DeleteProduct(string sku)
		{
			Product? product = await _productRepository.GetProduct(sku);
			if (product == null)
			{
				throw new EntityNotFoundException($"Product {sku} cannot be found.");
			}
			int rowsAffected = await _productRepository.DeleteProductCategoryMapping(product.ProductID);
			if (rowsAffected == 0)
			{
				throw new DatabaseOperationException($"Unable to Delete Product {sku} Category Mappings.");
			}
			List<int> pictureIds = await _productRepository.GetProductImageMappings(product.ProductID);
			if (pictureIds.Count != 0)
			{
				rowsAffected = await _productRepository.DeletePictureProductMappings(pictureIds);
				if(rowsAffected == 0)
				{
					throw new DatabaseOperationException($"Unable to Delete Product {sku} Image Mappings.");
				}
				await _pictureServices.DeletePictures(pictureIds);
			}
			rowsAffected = await _productRepository.DeleteProduct(product);
			if (rowsAffected == 0)
			{
				throw new DatabaseOperationException($"Unable to Delete Product {sku}.");
			}
			return new ResponseDto
			{
				IsSuccess = (rowsAffected > 0),
				ErrorMessage = (rowsAffected == 0) ? "Zero rows deleted" : "No Error"
			};
		}
		public async Task<ResponseDto> UpdateProduct(AddProductModel productModel)
		{
			Product? product = await _productRepository.GetProduct(productModel.SkuCode);
			if (product == null)
			{
				throw new EntityNotFoundException($"Product {productModel.SkuCode} not found.");
			}
			product.ProductName = productModel.ProductName;
			product.ProductDescription = productModel.ProductDescription;
			product.Price = productModel.Price;
			product.ExpiryDate = productModel.ExpiryDate;
			product.LoyaltyPoints = productModel.LoyaltyPoints;
			product.Weight = productModel.Weight;
			int rowsAffected = await _productRepository.DeleteProductCategoryMapping(product.ProductID);
			if (rowsAffected == 0)
			{
				throw new DatabaseOperationException($"Unable to Delete previous product Category mappings {productModel.SkuCode}.");
			}
			List<int> categoryIds = new List<int>();
			foreach (Guid categoryUid in productModel.Categories)
			{
				categoryIds.Add(await _categoryServices.GetCategoryId(categoryUid));
			}
			await AddProductCategoryMapping(categoryIds, product.ProductID);
			await AddProductImageMapping(productModel.Images, product.ProductID);
			rowsAffected = await _productRepository.UpdateProduct(product);
			if (rowsAffected == 0)
			{
				throw new DatabaseOperationException($"Unable to Update Product {productModel.SkuCode}.");
			}
			return new ResponseDto
			{
				IsSuccess = (rowsAffected > 0),
				ErrorMessage = (rowsAffected == 0) ? "Zero rows deleted" : "No Error"
			};
		}
		public async Task UploadPictures(IFormFile csvFile)
		{
			List<UploadPictureModel> pictures = CatalogHelper.UploadCsvFile<UploadPictureModel, UploadPictureModelMap>(csvFile);
			List<ProductImageMapping> productImages = new List<ProductImageMapping>();
			foreach (UploadPictureModel picture in pictures)
			{
				PictureModel pictureModel = await _pictureServices.AddPicture(new PictureModel
				{
					FileName = picture.FileName,
					MimeType = picture.MimeType,
					BinaryData = Convert.FromBase64String(picture.Base64EncodedData)
				});
				int? productId = (await _productRepository.GetProduct(picture.SkuCode))?.ProductID;
				if (productId == null)
				{
					throw new EntityNotFoundException($"Product {picture.SkuCode} doesn't exist");
				}
				productImages.Add(new ProductImageMapping
				{
					ProductId = productId.Value,
					ImageId = pictureModel.PictureId,
				});
			}
			await _productRepository.AddProductImageMapping(productImages);
		}

		public async Task UploadProducts(IFormFile csvFile)
		{
			List<UploadProductModel> products = CatalogHelper.UploadCsvFile<UploadProductModel, UploadProductModelMap>(csvFile);
			foreach (UploadProductModel product in products)
			{
				string productString = JsonConvert.SerializeObject(product);
				ProductUploads productUpload = new ProductUploads
				{
					Product = productString,
					IsSuccess = false,
					Sku = product.SkuCode
				};
				
				int rowAffected = await _productRepository.AddProductUploads(productUpload);
				
				try
				{
					await _azureQueueService.AddMessageAsync(productString);
					productUpload.IsSuccess = true;
				}
				catch (Exception ex)
				{
					productUpload.ExceptionMessage = ex.Message;
				}
				finally
				{
					await _productRepository.UpdateProductUploads(productUpload);
				}
			}
		}

		public async Task<List<FailedProductUpload>> FailedProductUploads()
		{
			return await _productRepository.GetFailedUploads();
		}
	}
}
