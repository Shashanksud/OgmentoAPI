
namespace OgmentoAPI.Domain.Client.Abstractions.Models.Planogram
{
	public class DeletePogModel
	{
		public string KioskName { get; set; }
		public int? MachineId { get; set; }
		public int? TrayId { get; set; }
		public int? BeltId { get; set; }
		public string? ProductSku { get; set; }
	}
}
