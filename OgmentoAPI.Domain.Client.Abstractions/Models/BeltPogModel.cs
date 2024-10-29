namespace OgmentoAPI.Domain.Client.Abstractions.Models
{
	public class BeltPogModel
	{
		public ProductPogModel Product { get; set; }
		public int BeltId { get; set; }
		public bool BeltIsActive {  get; set; }
	}
}
