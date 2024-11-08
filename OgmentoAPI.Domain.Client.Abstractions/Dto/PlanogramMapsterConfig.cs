using Mapster;
using OgmentoAPI.Domain.Client.Abstractions.Dto.Planogram;
using OgmentoAPI.Domain.Client.Abstractions.Models.Planogram;

namespace OgmentoAPI.Domain.Client.Abstractions.Dto
{
	public static class PlanogramMapsterConfig
	{
		public static void RegisterPlanogramMappings()
		{
			TypeAdapterConfig<PlanogramModel, PlanogramDto>.NewConfig();
			TypeAdapterConfig<PlanogramDto, PlanogramModel>.NewConfig();
			TypeAdapterConfig<MachinePogModel, MachinePogDto>.NewConfig();
			TypeAdapterConfig<MachinePogDto, MachinePogModel>.NewConfig();
			TypeAdapterConfig<TrayPogModel, TrayPogDto>.NewConfig();
			TypeAdapterConfig<TrayPogDto, TrayPogModel>.NewConfig();
			TypeAdapterConfig<BeltPogModel, BeltPogDto>.NewConfig();
			TypeAdapterConfig<BeltPogDto, BeltPogModel>.NewConfig();
			TypeAdapterConfig<ProductPogModel, ProductPogDto>.NewConfig();
			TypeAdapterConfig<ProductPogDto, ProductPogModel>.NewConfig();
			TypeAdapterConfig<AddPogModel, AddPogDto>.NewConfig();
			TypeAdapterConfig<AddPogDto, AddPogModel>.NewConfig();
			TypeAdapterConfig<DeletePogModel, DeletePogDto>.NewConfig();
			TypeAdapterConfig<DeletePogDto, DeletePogModel>.NewConfig();
			TypeAdapterConfig<StatusDto, StatusModel>.NewConfig();
		}
		public static AddPogDto ToDto(this AddPogModel addPogModel)
		{
			return addPogModel.Adapt<AddPogDto>();
		}

		public static AddPogModel ToModel(this AddPogDto addPogDto)
		{
			return addPogDto.Adapt<AddPogModel>();
		}
		public static StatusModel ToModel(this StatusDto statusDto)
		{
			return statusDto.Adapt<StatusModel>();
		}
		public static DeletePogDto ToDto(this DeletePogModel deletePogModel)
		{
			return deletePogModel.Adapt<DeletePogDto>();
		}

		public static DeletePogModel ToModel(this DeletePogDto deletePogDto)
		{
			return deletePogDto.Adapt<DeletePogModel>();
		}
		public static PlanogramDto ToDto(this PlanogramModel planogramModel)
		{
			return planogramModel.Adapt<PlanogramDto>();
		}

		public static PlanogramModel ToModel(this PlanogramDto planogramDto)
		{
			return planogramDto.Adapt<PlanogramModel>();
		}
		public static MachinePogDto ToDto(this MachinePogModel machinePogModel)
		{
			return machinePogModel.Adapt<MachinePogDto>();
		}
		public static List<MachinePogDto> ToDto(this List<MachinePogModel> machinePogModels)
		{
			return machinePogModels.Adapt<List<MachinePogDto>>();
		}
		public static MachinePogModel ToModel(this MachinePogDto machinePogDto)
		{
			return machinePogDto.Adapt<MachinePogModel>();
		}
		public static List<MachinePogModel> ToModel(this List<MachinePogDto> machinePogDtos)
		{
			return machinePogDtos.Adapt<List<MachinePogModel>>();
		}
		public static TrayPogDto ToDto(this TrayPogModel trayPogModel)
		{
			return trayPogModel.Adapt<TrayPogDto>();
		}
		public static List<TrayPogDto> ToDto(this List<TrayPogModel> trayPogModels)
		{
			return trayPogModels.Adapt<List<TrayPogDto>>();
		}

		public static TrayPogModel ToModel(this TrayPogDto trayPogDto)
		{
			return trayPogDto.Adapt<TrayPogModel>();
		}
		public static List<TrayPogModel> ToModel(this List<TrayPogDto> trayPogDtos)
		{
			return trayPogDtos.Adapt<List<TrayPogModel>>();
		}
		public static BeltPogDto ToDto(this BeltPogModel beltPogModel)
		{
			return beltPogModel.Adapt<BeltPogDto>();
		}
		public static List<BeltPogDto> ToDto(this List<BeltPogModel> beltPogModels)
		{
			return beltPogModels.Adapt<List<BeltPogDto>>();
		}
		public static BeltPogModel ToModel(this BeltPogDto beltPogDto)
		{
			return beltPogDto.Adapt<BeltPogModel>();
		}
		public static List<BeltPogModel> ToModel(this List<BeltPogDto> beltPogDtos)
		{
			return beltPogDtos.Adapt<List<BeltPogModel>>();
		}

		public static ProductPogDto ToDto(this ProductPogModel productPogModel)
		{
			return productPogModel.Adapt<ProductPogDto>();
		}

		public static ProductPogModel ToModel(this ProductPogDto productPogDto)
		{
			return productPogDto.Adapt<ProductPogModel>();
		}
		
	}
}
