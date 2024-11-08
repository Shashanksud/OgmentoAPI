namespace OgmentoAPI.Domain.Client.Abstractions.Dto.Planogram
{
	public class TrayPogDto
	{
		public int TrayId { get; set; }
		public bool TrayIsActive { get; set; }
		public List<BeltPogDto> Belt { get; set; }
	}
}
