using OgmentoAPI.Domain.Catalog.Abstractions.Models;
using OgmentoAPI.Domain.Common.Abstractions.Dto;

namespace OgmentoAPI.Domain.Catalog.Abstractions.Dto
{
	public class AddProductDto: ProductBase
	{
	
		public List<PictureDto> Images { get; set; } = new List<PictureDto>();
		public List<Guid> Categories { get; set; } = new List<Guid>();
	}
}
