namespace OgmentoAPI.Domain.Client.Abstractions.Models.Planogram
{
	public class AddPogModel
	{
		public string KioskName { get; set; }
		public int MachineId { get; set; }
		public int TrayId { get; set; }
		public int BeltId { get; set; }
		public string ProductSku { get; set; }
		public int Quatity { get; set; }
		public int MaxQuantity { get; set; }
		public bool TrayIsActive { get; set; }
		public bool BeltIsActive { get; set; }
	}
}
