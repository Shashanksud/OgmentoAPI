﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using OgmentoAPI.Domain.Catalog.Abstractions.DataContext;
using OgmentoAPI.Domain.Catalog.Abstractions.Models;
using OgmentoAPI.Domain.Catalog.Abstractions.Repository;
using OgmentoAPI.Domain.Common.Abstractions.Models;
using OgmentoAPI.Domain.Common.Abstractions.Services;

namespace OgmentoAPI.Domain.Catalog.Infrastructure.Repository
{
	public class ProductRepository: IProductRepository
	{
	
		private readonly CatalogDbContext _dbContext;
		private readonly IPictureService _pictureService;
		private readonly ICategoryRepository _categoryRepository;
		public ProductRepository(ICategoryRepository categoryRepository, CatalogDbContext dbContext, IPictureService pictureService)
		{
			_categoryRepository = categoryRepository;
			_dbContext = dbContext;
			_pictureService = pictureService;
		}
		public List<PictureModel> GetImages(int productId)
		{
			List<int> pictureIds = _dbContext.ProductImageMapping.Where(x => x.ProductId == productId).Select(x => x.ImageId).ToList();
			List<PictureModel> pictureModels = _pictureService.GetPictures(pictureIds);
			return pictureModels;
		}
		private CategoryModel GetCategory(int productId) {
			List<ProductCategoryMapping> productCategoryMappings = _dbContext.ProductCategoryMapping.Where(x => x.ProductId == productId ).ToList();
			List<CategoryModel> productCategories = new List<CategoryModel>();
			foreach(ProductCategoryMapping productCategory in productCategoryMappings)
			{
				productCategories.Add(_categoryRepository.GetCategory(productCategory.CategoryId));
			}
			CategoryModel category = productCategories.First(x => x.ParentCategoryId == 1);
			category.SubCategories = productCategories.Where(x=>x.ParentCategoryId==category.CategoryId).ToList();
			foreach(CategoryModel subCategory in category.SubCategories)
			{
				subCategory.SubCategories= productCategories.Where(x=>x.ParentCategoryId== subCategory.CategoryId).ToList();
			}
			return category;
		}

		public List<ProductModel> GetAllProducts()
		{
			List<Product> products = _dbContext.Product.ToList();
			List<ProductModel> productModel = products.Select(x => new ProductModel
			{
				ProductId = x.ProductID,
				ProductName = x.ProductName,
				SkuCode = x.SkuCode,
				LoyaltyPoints = x.LoyaltyPoints,
				Price = x.Price,
				ExpiryDate = x.ExpiryDate,
				ProductDescription = x.ProductDescription,
				Images = GetImages(x.ProductID),
				Category = GetCategory(x.ProductID)

			}).ToList();
			return productModel;
		}

		public ProductModel GetProduct(string sku)
		{
			Product product = _dbContext.Product.FirstOrDefault(x => x.SkuCode == sku);
			if (product == null) {
				throw new InvalidOperationException("Product not found.");
			}
			ProductModel productModel = new ProductModel()
			{
				ProductId= product.ProductID,
				ProductName= product.ProductName,
				SkuCode= sku,
				Images = GetImages(product.ProductID),
				Category = GetCategory(product.ProductID),
				Price = product.Price,
				ExpiryDate = product.ExpiryDate,
				ProductDescription = product.ProductDescription,
				LoyaltyPoints= product.LoyaltyPoints,
			};
			return productModel;
		}
		private async Task AddProductCategoryMapping(CategoryModel categoryModel, int productId)
		{
			List<int> categoryIds = new List<int>();
			int? primaryId = await _categoryRepository.GetCategoryId(categoryModel.CategoryUid);
			if (primaryId == null) {
				throw new InvalidOperationException("category not found.");
			}
			categoryIds.Add(primaryId.GetValueOrDefault());
			foreach(CategoryModel subCategory in categoryModel.SubCategories)
			{
				int? subCategoryId = await _categoryRepository.GetCategoryId(subCategory.CategoryUid);
				if (subCategoryId != null)
				{
					categoryIds.Add(subCategoryId.GetValueOrDefault());
				}
				foreach (CategoryModel subSubCategory in subCategory.SubCategories)
				{
					int? subSubCategoryId = await _categoryRepository.GetCategoryId(subSubCategory.CategoryUid);
					if (subSubCategoryId != null)
					{
						categoryIds.Add(subSubCategoryId.GetValueOrDefault());
					}
				}
			}
			foreach( int categoryId in categoryIds)
			{
				ProductCategoryMapping productCategory = new ProductCategoryMapping()
				{
					CategoryId = categoryId,
					ProductId = productId,
				};
				_dbContext.ProductCategoryMapping.Add(productCategory);
			}
			await _dbContext.SaveChangesAsync();
		} 
		private async Task UpdateProductImageMapping(List<PictureModel> pictures, int productId)
		{
			foreach (PictureModel picture in pictures) {
				if (picture.IsDeleted)
				{
					ProductImageMapping productImage = _dbContext.ProductImageMapping.First(x => x.ImageId == picture.Id);
					_dbContext.ProductImageMapping.Remove(productImage);
					await _dbContext.SaveChangesAsync();
					int response = await _pictureService.DeletePicture(picture.Hash);
					if (response == 0)
					{
						throw new InvalidOperationException("Picture cannot be deleted.");
					}
				}
				if (picture.IsNew)
				{
					PictureModel pictureModel = _pictureService.AddPicture(picture);
					ProductImageMapping productImageMapping = new ProductImageMapping()
					{
						ProductId = productId,
						ImageId = pictureModel.Id
					};
					_dbContext.ProductImageMapping.Add(productImageMapping);
					await _dbContext.SaveChangesAsync();
				}
			}
		}
		public async Task<ProductModel> UpdateProduct(ProductModel productModel)
		{
			Product product = _dbContext.Product.FirstOrDefault(x => x.SkuCode == productModel.SkuCode);
			if(product == null)
			{
				throw new InvalidOperationException("Product not found.");
			}
			product.ProductName = productModel.ProductName;
			product.ProductDescription = productModel.ProductDescription;
			product.Price = productModel.Price;
			product.ExpiryDate = productModel.ExpiryDate;
			product.LoyaltyPoints = productModel.LoyaltyPoints;
			_dbContext.Product.Update(product);
			await _dbContext.SaveChangesAsync();
			List<ProductCategoryMapping> productCategoryMappings = _dbContext.ProductCategoryMapping.Where(x => x.ProductId == product.ProductID).ToList();
			_dbContext.ProductCategoryMapping.RemoveRange(productCategoryMappings);
			await _dbContext.SaveChangesAsync();
			await AddProductCategoryMapping(productModel.Category,product.ProductID);
			await UpdateProductImageMapping(productModel.Images, product.ProductID);
			return productModel;
		}

		public async Task DeleteProduct(string sku)
		{
			Product product = _dbContext.Product.FirstOrDefault(x => x.SkuCode == sku);
			if(product == null)
			{
				throw new InvalidOperationException("Product cannot be found.");
			}
			List<ProductCategoryMapping> productCategoryMappings = _dbContext.ProductCategoryMapping.Where(x => x.ProductId == product.ProductID).ToList();
			_dbContext.ProductCategoryMapping.RemoveRange(productCategoryMappings);
			List<ProductImageMapping> productImageMappings = _dbContext.ProductImageMapping.Where(x => x.ProductId == product.ProductID).ToList();
			List<int> pictureIds = productImageMappings.Select(x=>x.ImageId).ToList();
			_dbContext.ProductImageMapping.RemoveRange(productImageMappings);
			_dbContext.Product.Remove(product);
			await _dbContext.SaveChangesAsync();
			await _pictureService.DeletePictures(pictureIds);
		}

		public async Task<ProductModel> AddProduct(ProductModel productModel)
		{
			bool SkuExists = _dbContext.Product.Any(x => x.SkuCode == productModel.SkuCode);
			if (SkuExists)
			{
				throw new InvalidOperationException($"Product with skucode: {productModel.SkuCode} already exists. Please give different code.");
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
			EntityEntry<Product> productEntry = _dbContext.Product.Add(product);
			await _dbContext.SaveChangesAsync();
			productModel.ProductId = productEntry.Entity.ProductID;
			await UpdateProductImageMapping(productModel.Images, productModel.ProductId);
			await AddProductCategoryMapping(productModel.Category, productModel.ProductId);
			return productModel;
		}
		public async Task<List<ProductModel>> UploadProducts(List<ProductModel> products)
		{
			foreach (ProductModel product in products) { 
				 await AddProduct(product);
			}
			return products;
		}
	}
}
