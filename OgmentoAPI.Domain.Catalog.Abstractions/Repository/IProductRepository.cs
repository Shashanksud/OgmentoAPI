using OgmentoAPI.Domain.Catalog.Abstractions.Models;
using OgmentoAPI.Domain.Common.Abstractions.Models;

namespace OgmentoAPI.Domain.Catalog.Abstractions.Repository
{
	public interface IProductRepository
	{
		Task<List<ProductModel>> GetAllProducts();
		Task<List<PictureModel>> GetImages(int productId);
		Task<ProductModel> GetProduct(string sku);
		Task UpdateProduct(AddProductModel productModel);
		Task DeleteProduct(string sku);
		Task AddProduct(AddProductModel productModel);
		Task UploadProducts(List<UploadProductModel> products);
		Task UploadPictures(List<UploadPictureModel> pictures);
		Task<List<FailedProductUpload>> GetFailedUploads();
		Task AddProduct(UploadProductModel product);
	}
}
