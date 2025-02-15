﻿using OgmentoAPI.Domain.Client.Abstractions.Models;
using Mapster;

namespace OgmentoAPI.Domain.Client.Abstractions.Dto
{
	public static class KioskMapsterConfig
	{
		
		public static List<KioskDto> ToDto(this List<KioskModel> kioskModels)
		{
			return kioskModels.Adapt<List<KioskDto>>();
		}

		public static KioskDto ToDto(this KioskModel kioskModel)
		{
			return kioskModel.Adapt<KioskDto>();
		}

		public static KioskModel ToModel(this KioskDto kioskDto)
		{
			return kioskDto.Adapt<KioskModel>();
		}

		public static List<KioskModel> ToModel(this List<KioskDto> kioskDtos)
		{
			return kioskDtos.Adapt<List<KioskModel>>();
		}
		

		public static void RegisterKioskMappings()
		{
			TypeAdapterConfig<KioskModel, KioskDto>.NewConfig();
			TypeAdapterConfig<KioskDto, KioskModel>.NewConfig();
			
		}
	    
	
	}
}