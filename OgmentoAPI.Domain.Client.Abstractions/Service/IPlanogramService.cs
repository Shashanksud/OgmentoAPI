using OgmentoAPI.Domain.Client.Abstractions.Models;

namespace OgmentoAPI.Domain.Client.Abstractions.Service
{
	public interface IPlanogramService
	{
		//Task<int> AddPOG(KioskPogModel kioskPog);
		Task<PlanogramModel> GetPOG(string kioskName);
	}
}
