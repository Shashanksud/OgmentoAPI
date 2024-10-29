using System.ComponentModel.DataAnnotations;

namespace OgmentoAPI.Domain.Client.Abstractions.DataContext
{
	public class Planogram
	{
		[Key]
		public int PlanogramId { get; set; }
		public int KioskId { get; set; }
		public int MachineId { get; set; }
		public int TrayId { get; set; }
		public int BeltId { get; set; }
		public int ProductId { get; set; }
		public int Quatity { get; set; }
		public int MaxQuantity { get; set; }
		public bool TrayIsActive { get; set; }
		public bool BeltIsActive { get; set; }
	}
}
