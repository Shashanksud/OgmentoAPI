namespace OgmentoAPI.Domain.Client.Abstractions.Models.Planogram
{
	public class PlanogramModel
	{
		public KioskModel? Location { get; set; }
		public List<MachinePogModel> Machines { get; set; }
	}
}
