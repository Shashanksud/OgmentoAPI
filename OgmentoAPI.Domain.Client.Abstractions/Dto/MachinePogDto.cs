
namespace OgmentoAPI.Domain.Client.Abstractions.Dto
{
	public class MachinePogDto
	{
		public int MachineId { get; set; }
		public List<TrayPogDto> Trays { get; set; }
	}
}
