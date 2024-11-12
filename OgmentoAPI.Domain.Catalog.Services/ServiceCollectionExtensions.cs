using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OgmentoAPI.Domain.Catalog.Abstractions.Repository;
using OgmentoAPI.Domain.Catalog.Abstractions.Services;
using OgmentoAPI.Domain.Catalog.Infrastructure;
using OgmentoAPI.Domain.Catalog.Infrastructure.Repository;
using OgmentoAPI.Domain.Common.Abstractions.Repository;
using OgmentoAPI.Domain.Common.Abstractions.Services;
using OgmentoAPI.Domain.Common.Infrastructure.Repository;
using OgmentoAPI.Domain.Common.Services;

namespace OgmentoAPI.Domain.Catalog.Services
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddCatalog(this IServiceCollection services, string dbConnectionString)
		{
			return services.AddDbContext<CatalogDbContext>(opts => opts.UseSqlServer(dbConnectionString))
				.AddTransient<IAzureQueueService, AzureQueueService>()
				.AddTransient<ICategoryRepository, CategoryRepository>()
				.AddTransient<ICategoryServices, CategoryServices>()
				.AddTransient<IPictureRepository, PictureRepository>()
				.AddTransient<IPictureService, PictureServices>()
				.AddTransient<IProductRepository, ProductRepository>()
				.AddTransient<IProductServices, ProductServices>();
		}
	}
}
