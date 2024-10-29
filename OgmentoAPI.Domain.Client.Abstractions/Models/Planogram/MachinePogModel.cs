namespace OgmentoAPI.Domain.Client.Abstractions.Models.Planogram
{
	public class MachinePogModel
	{
		public int MachineId { get; set; }
		public List<TrayPogModel> Trays { get; set; }
	}
}
