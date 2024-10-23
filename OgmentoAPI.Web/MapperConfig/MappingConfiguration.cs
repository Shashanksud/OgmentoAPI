using Mapster;
using OgmentoAPI.Domain.Authorization.Abstractions.Dto;
using OgmentoAPI.Domain.Catalog.Abstractions.Dto;
using OgmentoAPI.Domain.Client.Abstractions.DataContext;
using OgmentoAPI.Domain.Client.Abstractions.Dto;

namespace OgmentoAPI.MapperConfig
{
	public class MappingConfiguration
	{
		public static void ConfigureMappings()
		{
			UserMapsterConfig.RegisterUserMappings();
			KioskMapsterConfig.RegisterKioskMappings();
			SalesCentersMapsterConfig.RegisterSalesCentersMappings();
			ProductMapsterConfig.RegisterProductMappings();
			CategoryMapsterConfig.RegisterCategoryMappings();
		}
	}

}
