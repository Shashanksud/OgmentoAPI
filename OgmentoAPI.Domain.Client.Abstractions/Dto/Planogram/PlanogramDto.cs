using OgmentoAPI.Domain.Client.Abstractions.Models;

namespace OgmentoAPI.Domain.Client.Abstractions.Dto.Planogram
{
	public class PlanogramDto
	{
		public KioskDto? Location { get; set; }
		public List<MachinePogDto> Machines { get; set; }
	}
}
