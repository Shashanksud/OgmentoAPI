namespace OgmentoAPI.Domain.Client.Abstractions.Dto.Planogram
{
	public class BeltPogDto
	{
		public ProductPogDto Product { get; set; }
		public int BeltId { get; set; }
		public bool BeltIsActive { get; set; }
	}
}
