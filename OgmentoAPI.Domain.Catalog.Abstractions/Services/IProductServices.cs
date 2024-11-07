using Microsoft.AspNetCore.Http;
using OgmentoAPI.Domain.Catalog.Abstractions.Models;

namespace OgmentoAPI.Domain.Catalog.Abstractions.Services
{
	public interface IProductServices
	{
		
		
	
		
		
		Task<List<ProductModel>> GetAllProducts();
		Task<ProductModel> GetProduct(string sku);
		Task UpdateProduct(AddProductModel product);
		Task DeleteProduct(string sku);
		Task<ProductModel> AddProduct(AddProductModel product);
		Task<List<FailedProductUpload>> UploadProducts(IFormFile csvFile);
		Task UploadProducts(IFormFile csvFile);
		Task DeletePicture(string hash);
		Task AddProduct(UploadProductModel product);
	}
}
