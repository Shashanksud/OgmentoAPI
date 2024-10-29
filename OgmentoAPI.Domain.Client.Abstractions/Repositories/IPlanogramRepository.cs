using OgmentoAPI.Domain.Client.Abstractions.DataContext;

namespace OgmentoAPI.Domain.Client.Abstractions.Repositories
{
	public interface IPlanogramRepository
	{
		Task<List<int>> GetMachineIds(int kioskId);
		Task<List<(int,bool)>> GetTrayIds(int kioskId, int machineId);
		Task<List<(int,bool)>> GetBeltIds(int machineId, int kioskId, int trayId);
		Task<Planogram> GetProduct(int kioskId, int machineId, int trayId, int beltId);
	}
}
