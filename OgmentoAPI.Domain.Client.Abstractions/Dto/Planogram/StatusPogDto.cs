
namespace OgmentoAPI.Domain.Client.Abstractions.Dto.Planogram
{
	public class StatusPogDto
	{
		public string KioskName {  get; set; }
		public int MachineId {  get; set; }
		public int TrayId { get; set; }
		public int? BeltId { get; set; }
		public bool Status { get; set; }
	}
}
