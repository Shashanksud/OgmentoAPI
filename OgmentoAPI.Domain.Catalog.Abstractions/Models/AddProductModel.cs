﻿

using OgmentoAPI.Domain.Common.Abstractions.Dto;
using OgmentoAPI.Domain.Common.Abstractions.Models;

namespace OgmentoAPI.Domain.Catalog.Abstractions.Models
{
	public class AddProductModel: ProductBase
	{
		public int ProductId { get; set; }
		public List<PictureModel> Images { get; set; } = new List<PictureModel>();
		public List<Guid> Categories { get; set; } = new List<Guid>();
	}
}
