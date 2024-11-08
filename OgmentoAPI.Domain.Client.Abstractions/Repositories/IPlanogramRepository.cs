using OgmentoAPI.Domain.Client.Abstractions.DataContext;
using OgmentoAPI.Domain.Client.Abstractions.Models.Planogram;

namespace OgmentoAPI.Domain.Client.Abstractions.Repositories
{
	public interface IPlanogramRepository
	{
		List<int> GetMachineIds(int kioskId);
		List<(int,bool)> GetTrayIds(int kioskId, int machineId);
		List<(int,bool)> GetBeltIds(int machineId, int kioskId, int trayId);
		Task<Planogram?> GetPlanogram(int kioskId, int machineId, int trayId, int beltId);
		Task<List<Planogram>> GetPlanogram(int kioskId, int machineId, int trayId);
		Task<List<Planogram>> GetPlanogram(int kioskId, int machineId);
		Task<int> AddPOG(Planogram planogram);
		Task<int> UpdatePOG(Planogram planogram);
		bool IsPlanogramExists(int kioskId, int machineId, int trayId, int beltId);
		Task<int> DeleteBelt(Planogram planogram);
		Task<int> DeletePlanograms(List<int> planogramIds);
		Task<int> UpdateStatus(StatusModel status);
	}
}
