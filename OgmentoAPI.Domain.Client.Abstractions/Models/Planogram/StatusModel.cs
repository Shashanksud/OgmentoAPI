
namespace OgmentoAPI.Domain.Client.Abstractions.Models.Planogram
{
	public class StatusModel
	{
		public int? KioskId { get; set; }
		public string KioskName { get; set; }
		public int MachineId { get; set; }
		public int TrayId { get; set; }
		public int? BeltId { get; set; }
		public bool Status { get; set; }
	}
}
