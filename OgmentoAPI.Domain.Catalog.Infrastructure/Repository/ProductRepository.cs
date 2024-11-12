using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OgmentoAPI.Domain.Catalog.Abstractions.DataContext;
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
		public async Task<Guid> AddProductUploadFile(ProductUploads product)
		{
			EntityEntry<ProductUploads> productEntity = await _dbContext.ProductUploads.AddAsync(product);
			await _dbContext.SaveChangesAsync();
			return productEntity.Entity.ProductUploadsGuid;
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
		public async Task<List<FailedProductUploads>> GetFailedUploads()
		{
			return await _dbContext.FailedProductUploads.ToListAsync();
		}

		public async Task<int> AddFailedProductUploads(FailedProductUploads product)
		{
			_dbContext.FailedProductUploads.Add(product);
			return await _dbContext.SaveChangesAsync();
		}
		public async Task<int> UpdateProductUploads(ProductUploads productUploads)
		{
			_dbContext.ProductUploads.Update(productUploads);
			return await _dbContext.SaveChangesAsync();
		}
		public async Task<ProductUploads> GetProductUploadsFile(Guid fileUploadUid)
		{
			return await _dbContext.ProductUploads.FirstAsync(x=>x.ProductUploadsGuid == fileUploadUid);
		}
		public async Task<int?> GetProductId(string sku)
		{
			return (await _dbContext.Product.SingleOrDefaultAsync(x => x.SkuCode == sku))?.ProductID;
		}
	}
}
