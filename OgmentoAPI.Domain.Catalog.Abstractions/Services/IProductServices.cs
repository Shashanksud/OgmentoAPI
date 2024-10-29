using Microsoft.AspNetCore.Http;
using OgmentoAPI.Domain.Catalog.Abstractions.Models;
using OgmentoAPI.Domain.Common.Abstractions.Dto;

namespace OgmentoAPI.Domain.Catalog.Abstractions.Services
{
	public interface IProductServices
	{

		Task<List<ProductModel>> GetAllProducts();
		Task<ProductModel> GetProduct(string sku);
		Task<ResponseDto> UpdateProduct(AddProductModel productModel);
		Task<ResponseDto> DeleteProduct(string sku);
		Task<ResponseDto> AddProduct(AddProductModel productModel);
		Task UploadPictures(IFormFile csvFile);
		Task UploadProducts(IFormFile csvFile);
		Task<ResponseDto> DeletePicture(string hash);
		Task SaveProductUpload(UploadProductModel product);
		Task<bool> IsSkuExists(string sku);
		Task<List<FailedProductUpload>> FailedProductUploads();
		Task<List<ProductModel>> GetAllProducts();
		Task<ProductModel> GetProduct(string sku);
		Task UpdateProduct(AddProductModel product);
		Task DeleteProduct(string sku);
		Task<ProductModel> AddProduct(AddProductModel product);
		Task UploadProducts(IFormFile csvFile);
		Task UploadPictures(IFormFile csvFile);
		Task<ProductBase> GetProduct(int productId);
		Task<int> GetProductId(string sku);
	}
}
