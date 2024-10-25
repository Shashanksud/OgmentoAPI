using Mapster;
using OgmentoAPI.Domain.Catalog.Abstractions.Models;
using System.Collections.Generic;

namespace OgmentoAPI.Domain.Catalog.Abstractions.Dto
{
	public static class CategoryMapsterConfig
	{
		public static void RegisterCategoryMappings()
		{
			TypeAdapterConfig<CategoryModel, CategoryDto>.NewConfig();
			TypeAdapterConfig<CategoryDto, CategoryModel>.NewConfig();		
		}

		public static List<CategoryDto> ToDto(this List<CategoryModel> categoryModels)
		{
			return categoryModels.Adapt<List<CategoryDto>>();
		}

		public static List<CategoryModel> ToModel(this List<CategoryDto> categoryDtos)
		{
			return categoryDtos.Adapt<List<CategoryModel>>();
		}

		public static CategoryModel ToModel(this CategoryDto categoryDto)
		{
			return categoryDto.Adapt<CategoryModel>();
		}

		public static CategoryDto ToDto(this CategoryModel categoryModel)
		{
			return categoryModel.Adapt<CategoryDto>();
		}
	}
}