namespace OgmentoAPI.Domain.Client.Abstractions.Dto
{
	public class BeltPogDto
	{
		public ProductPogDto Product { get; set; }
		public int BeltId { get; set; }
		public bool BeltIsActive {  get; set; }
	}
}
