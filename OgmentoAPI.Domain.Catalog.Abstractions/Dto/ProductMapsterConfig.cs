using OgmentoAPI.Domain.Catalog.Abstractions.Models;
using OgmentoAPI.Domain.Common.Abstractions.Dto;
using Mapster;
using OgmentoAPI.Domain.Common.Abstractions.Models;
namespace OgmentoAPI.Domain.Catalog.Abstractions.Dto
{
	public static class ProductMapsterConfig
	{
		public static void RegisterProductMappings()
		{
			TypeAdapterConfig<ProductModel, ProductDto>.NewConfig();
			TypeAdapterConfig<PictureDto, PictureModel>.NewConfig().Map(dest => dest.BinaryData, src=> Convert.FromBase64String(src.Base64Encoded));
			TypeAdapterConfig<CategoryModel,CategoryDto>.NewConfig();
            TypeAdapterConfig<ProductDto, ProductModel>.NewConfig();

            TypeAdapterConfig<AddProductDto, AddProductModel>.NewConfig();
            TypeAdapterConfig<PictureModel, PictureDto>.NewConfig().Map(dest => dest.Base64Encoded, src => Convert.ToBase64String(src.BinaryData));


        }

		public static List<ProductDto> ToDto(this List<ProductModel> productModels)
		{
			return productModels.Adapt<List<ProductDto>>();
		}

		public static List<ProductModel> ToModel(this List<ProductDto> productDtos)
		{
			return productDtos.Adapt<List<ProductModel>>();
		}

		public static ProductModel ToModel(this ProductDto productDto)
		{
			return productDto.Adapt<ProductModel>();
		}

		public static ProductDto ToDto(this ProductModel productModel)
		{
			return productModel.Adapt<ProductDto>();
		}

		public static AddProductModel ToModel(this AddProductDto addProductDto)
		{
			return addProductDto.Adapt<AddProductModel>();
		}
	}
}



