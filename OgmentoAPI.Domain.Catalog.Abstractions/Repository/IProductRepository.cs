using OgmentoAPI.Domain.Catalog.Abstractions.DataContext;
using OgmentoAPI.Domain.Catalog.Abstractions.Models;

namespace OgmentoAPI.Domain.Catalog.Abstractions.Repository
{
	public interface IProductRepository
	{
		Task<List<Product>> GetAllProducts();
		Task<int?> GetProductId(string sku);
		Task<List<int>> GetImages(int productId);
		Task<Product?> GetProduct(string sku);
		Task<int> UpdateProduct(Product product);
		Task<int> DeleteProduct(Product product);
		Task<int> AddProduct(Product product);
		Task<int> AddProductUploads(ProductUploads product);
		Task<int> UpdateProductUploads(ProductUploads product);
		Task<int> DeletePictureProductMapping(int pictureId);
		Task<int> DeleteProductCategoryMapping(int productId);
		Task<int> DeletePictureProductMappings(List<int> pictureIds);
		Task<List<FailedProductUpload>> GetFailedUploads();
		Task<List<int>> GetCategory(int productId);
		Task<bool> IsSkuExists(string sku);
		Task<int> AddProductCategoryMapping(List<ProductCategoryMapping> productCategories);
		Task<int> AddProductImageMapping(List<ProductImageMapping> productImageMappings);
		Task<List<int>> GetProductImageMappings(int productId);
		Task<ProductUploads> GetProductUploads(String sku);
		Task<List<ProductModel>> GetAllProducts();
		Task<List<PictureModel>> GetImages(int productId);
		Task<ProductModel> GetProduct(string sku);
		Task UpdateProduct(AddProductModel productModel);
		Task DeleteProduct(string sku);
		Task AddProduct(AddProductModel productModel);
		Task UploadProducts(List<UploadProductModel> products);
		Task UploadPictures(List<UploadPictureModel> pictures);
		Task<ProductBase> GetProduct(int productId);
	}
}
