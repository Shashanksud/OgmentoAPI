using OgmentoAPI.Domain.Catalog.Abstractions.Models;

namespace OgmentoAPI.Domain.Client.Abstractions.Models.Planogram
{
	public class ProductPogModel : ProductBase
	{
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public int MaxQuantity { get; set; }
		public bool Scannable { get; set; }
	}
}
