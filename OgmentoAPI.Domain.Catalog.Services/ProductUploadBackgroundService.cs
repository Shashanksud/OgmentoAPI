using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OgmentoAPI.Domain.Catalog.Abstractions.Models;
using OgmentoAPI.Domain.Catalog.Abstractions.Services;

namespace OgmentoAPI.Domain.Catalog.Services
{
	public class ProductUploadBackgroundService : BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;

		public ProductUploadBackgroundService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}
		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			return Task.CompletedTask;
		}
		public async Task UploadProductsInBackground(List<UploadProductModel> products)
		{
			using (var scope = _serviceProvider.CreateScope())
			{
				IProductServices productService = scope.ServiceProvider.GetRequiredService<IProductServices>();
				await productService.UploadProducts(products);
			}
		}

		
	}

}
