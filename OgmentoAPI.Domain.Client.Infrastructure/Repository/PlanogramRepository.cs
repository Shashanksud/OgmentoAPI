using OgmentoAPI.Domain.Client.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using OgmentoAPI.Domain.Client.Abstractions.DataContext;
using OgmentoAPI.Domain.Client.Abstractions.Models;
namespace OgmentoAPI.Domain.Client.Infrastructure.Repository
{
	public class PlanogramRepository : IPlanogramRepository
	{
		private readonly ClientDBContext _dbContext;
		public PlanogramRepository(ClientDBContext dbContext)
		{
			_dbContext = dbContext;
		}
		public async Task<List<int>> GetMachineIds(int kioskId)
		{
			return  _dbContext.Planogram
				.AsNoTracking()
				.Where(x => x.KioskId == kioskId)
				.Select(x => x.MachineId)
				.Distinct()
				.ToList();
		}
		public async Task<List<(int,bool)>> GetTrayIds(int kioskId,int machineId)
		{
			List<Planogram> planograms = _dbContext.Planogram
					.AsNoTracking()
					.Where(x => x.KioskId == kioskId && x.MachineId == machineId)
					.ToList();
			List<int> trayIds = planograms
					.Select(x => x.TrayId)
					.Distinct()
					.ToList();
			List<(int, bool)> trayData = new List<(int, bool)>();
			for (int i = 0; i < trayIds.Count; i++) {
				bool trayIdStatus = planograms.First(y => y.MachineId == machineId && y.KioskId == kioskId && y.TrayId == trayIds[i]).TrayIsActive;
				trayData.Add((trayIds[i], trayIdStatus));
			}
			return trayData;
		}
		public async Task<List<(int,bool)>> GetBeltIds(int kioskId,int machineId, int trayId) {
			List<Planogram> planograms = _dbContext.Planogram
				.AsNoTracking()
				.Where(x => x.KioskId == kioskId && x.MachineId == machineId && x.TrayId == trayId)
				.ToList();
			List<int> beltIds = planograms
				.Select(x => x.BeltId)
				.Distinct()
				.ToList();
			List<(int, bool)> beltData = new List<(int, bool)>();
			for (int i = 0; i < beltIds.Count; i++)
			{
				bool beltIdStatus = planograms.First(y => y.MachineId == machineId && y.KioskId == kioskId && y.TrayId ==trayId && y.BeltId == beltIds[i] ).BeltIsActive;
				beltData.Add((beltIds[i], beltIdStatus));
			}
			return beltData;
		}
		public async Task<Planogram> GetProduct(int kioskId, int machineId, int trayId, int beltId)
		{
			return await _dbContext.Planogram.SingleAsync(x => x.KioskId == kioskId && x.MachineId == machineId && x.TrayId == trayId && x.BeltId == beltId);
		}

	}
}
