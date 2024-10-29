
namespace OgmentoAPI.Domain.Client.Abstractions.Models
{
	public class MachinePogModel
	{
		public int MachineId { get; set; }
		public List<TrayPogModel> Trays { get; set; }
	}
}
