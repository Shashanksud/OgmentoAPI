using Microsoft.EntityFrameworkCore;
using OgmentoAPI.Domain.Catalog.Abstractions.DataContext;
using OgmentoAPI.Domain.Catalog.Abstractions.Models;
using OgmentoAPI.Domain.Catalog.Abstractions.Repository;

namespace OgmentoAPI.Domain.Catalog.Infrastructure.Repository
{
	public class ProductRepository: IProductRepository
	{
		private readonly CatalogDbContext _dbContext;
		public ProductRepository(CatalogDbContext dbContext)
		{
			_dbContext = dbContext;
		}
		public async Task<List<int>> GetImages(int productId)
		{
			return await _dbContext.ProductImageMapping.Where(x => x.ProductId == productId).Select(x => x.ImageId).ToListAsync();
		}
		public async Task<List<int>> GetCategory(int productId) {
			return await _dbContext.ProductCategoryMapping.Where(x => x.ProductId == productId ).Select(x=>x.CategoryId).ToListAsync();
		}
		public async Task<Product?> GetProduct(string sku)
		{
			return await _dbContext.Product.FirstOrDefaultAsync(x => x.SkuCode == sku);
		}
		public async Task<Product?> GetProduct(int productId)
		{
			return await _dbContext.Product.FirstOrDefaultAsync(x => x.ProductID == productId);
		}
		public async Task<List<Product>> GetAllProducts()
		{
			return await _dbContext.Product.ToListAsync();
		}
		
		public async Task<int> AddProductCategoryMapping(List<ProductCategoryMapping> productCategories)
		{
			await _dbContext.ProductCategoryMapping.AddRangeAsync(productCategories);
			return await _dbContext.SaveChangesAsync();
		} 
		public async Task<int> AddProductImageMapping(List<ProductImageMapping> productImageMappings)
		{
			await _dbContext.ProductImageMapping.AddRangeAsync(productImageMappings);
			return await _dbContext.SaveChangesAsync();
		}
		public async Task<int> DeleteProductCategoryMapping(int productId)
		{
			return await _dbContext.ProductCategoryMapping.Where(x => x.ProductId == productId).ExecuteDeleteAsync();
		}
		public async Task<List<int>> GetProductImageMappings(int productId)
		{
			return await _dbContext.ProductImageMapping.Where(x => x.ProductId == productId).Select(x => x.ImageId).ToListAsync();
		}
		public async Task<int> UpdateProduct(Product product)
		{
			_dbContext.Product.Update(product);
			return await _dbContext.SaveChangesAsync();
		}
		public async Task<int> DeleteProduct(Product product)
		{
			_dbContext.Product.Remove(product);
			return await _dbContext.SaveChangesAsync();
		}
		public async Task<int> AddProduct(Product product)
		{
			await _dbContext.Product.AddAsync(product);
			return await _dbContext.SaveChangesAsync();
		}
		public async Task<bool> IsSkuExists(string sku)
		{
			return await _dbContext.Product.AsNoTracking().AnyAsync(x => x.SkuCode == sku);
		}
		public async Task<int> DeletePictureProductMapping(int pictureId)
		{
			return await _dbContext.ProductImageMapping.Where(x => x.ImageId == pictureId).ExecuteDeleteAsync();
		}
		public async Task<int> DeletePictureProductMappings(List<int> pictureIds)
		{
			return await _dbContext.ProductImageMapping.Where(x => pictureIds.Contains(x.ImageId)).ExecuteDeleteAsync();
		}
		public async Task<List<FailedProductUpload>> GetFailedUploads()
		{
			return await _dbContext.ProductUploads
				.Where(p => !p.IsSuccess)
				.Select(p => new FailedProductUpload
				{
					Sku = p.Sku,
					ExceptionMessage = p.ExceptionMessage
				})
				.ToListAsync();
		}

		public async Task<int> AddProductUploads(ProductUploads product)
		{
			_dbContext.ProductUploads.Add(product);
			return await _dbContext.SaveChangesAsync();
		}
		public async Task<int> UpdateProductUploads(ProductUploads product)
		{
			_dbContext.ProductUploads.Update(product);
			return await _dbContext.SaveChangesAsync();
		}
		public async Task<ProductUploads> GetProductUploads(String sku)
		{
			return await _dbContext.ProductUploads.FirstAsync(x=>x.Sku == sku);
		}
		public async Task<int?> GetProductId(string sku)
		{
			return (await _dbContext.Product.SingleOrDefaultAsync(x => x.SkuCode == sku))?.ProductID;
		}
	}
}
