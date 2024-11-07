using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OgmentoAPI.Domain.Catalog.Abstractions.Repository;
using OgmentoAPI.Domain.Catalog.Abstractions.Services;
using OgmentoAPI.Domain.Catalog.Infrastructure;
using OgmentoAPI.Domain.Catalog.Infrastructure.Repository;
using OgmentoAPI.Domain.Common.Abstractions;

namespace OgmentoAPI.Domain.Catalog.Services
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddCatalog(this IServiceCollection services, string dbConnectionString)
		{
			return services.AddDbContext<CatalogDbContext>(opts => opts.UseSqlServer(dbConnectionString))
				.AddTransient<ICategoryServices, CategoryServices>()
				.AddTransient<ICategoryRepository, CategoryRepository>()
				.AddTransient<IProductServices, ProductServices>()
				.AddTransient<IProductRepository, ProductRepository>()
				.AddTransient<IAzureQueueService, AzureQueueService>();
		}
	}
}
