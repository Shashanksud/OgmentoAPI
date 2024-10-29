
using OgmentoAPI.Domain.Catalog.Abstractions.Models;

namespace OgmentoAPI.Domain.Client.Abstractions.Dto
{
	public class ProductPogDto: ProductBase
	{
		public int Quantity { get; set; }
		public int MaxQuantity { get; set; }
		public bool Scannable { get; set; }
	}
}
