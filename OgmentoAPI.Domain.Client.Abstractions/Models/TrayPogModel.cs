﻿
namespace OgmentoAPI.Domain.Client.Abstractions.Models
{
	public class TrayPogModel
	{
		public int TrayId { get; set; }
		public bool TrayIsActive { get; set; }
		public List<BeltPogModel> Belt { get; set; }
	}
}
