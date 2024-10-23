using Mapster;
using OgmentoAPI.Domain.Client.Abstractions.Dto;
using OgmentoAPI.Domain.Client.Abstractions.Models;

public static class SalesCentersMapsterConfig
{
	public static void RegisterSalesCentersMappings()
	{
		TypeAdapterConfig<SalesCenterModel, SalesCentersDto>.NewConfig();
		TypeAdapterConfig<SalesCentersDto, SalesCenterModel>.NewConfig();
	}

	public static List<SalesCentersDto> ToDto(this List<SalesCenterModel> models)
	{
		return models.Adapt<List<SalesCentersDto>>();
	}

	public static SalesCentersDto ToDto(this SalesCenterModel model)
	{
		return model.Adapt<SalesCentersDto>();
	}

	public static SalesCenterModel ToModel(this SalesCentersDto dto)
	{
		return dto.Adapt<SalesCenterModel>();
	}

	public static List<SalesCenterModel> ToModel(this List<SalesCentersDto> dtos)
	{
		return dtos.Adapt<List<SalesCenterModel>>();
	}
}
