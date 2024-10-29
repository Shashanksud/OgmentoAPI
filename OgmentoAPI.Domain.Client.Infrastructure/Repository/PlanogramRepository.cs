using OgmentoAPI.Domain.Client.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using OgmentoAPI.Domain.Client.Abstractions.DataContext;
using OgmentoAPI.Domain.Common.Abstractions.CustomExceptions;
namespace OgmentoAPI.Domain.Client.Infrastructure.Repository
{
	public class PlanogramRepository : IPlanogramRepository
	{
		private readonly ClientDBContext _dbContext;
		public PlanogramRepository(ClientDBContext dbContext)
		{
			_dbContext = dbContext;
		}
		public List<int> GetMachineIds(int kioskId)
		{
			return  _dbContext.Planogram
				.AsNoTracking()
				.Where(x => x.KioskId == kioskId)
				.Select(x => x.MachineId)
				.Distinct()
				.ToList();
		}
		public List<(int,bool)> GetTrayIds(int kioskId,int machineId)
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
		public List<(int,bool)> GetBeltIds(int kioskId,int machineId, int trayId) {
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
		public async Task<Planogram?> GetPlanogram(int kioskId, int machineId, int trayId, int beltId)
		{
			return await _dbContext.Planogram.SingleOrDefaultAsync(x => x.KioskId == kioskId && x.MachineId == machineId && x.TrayId == trayId && x.BeltId == beltId);
		}

		public async Task<int> AddPOG(Planogram planogram)
		{
			try
			{
				_dbContext.Planogram.Add(planogram);
				return await _dbContext.SaveChangesAsync();
			}catch(DbUpdateException ex)
			{
				throw new DatabaseOperationException($"error occurred while adding planogram: {ex}");
			}
		}
		public async Task<int> UpdatePOG(Planogram planogram)
		{
			try
			{
				_dbContext.Planogram.Update(planogram);
				return await _dbContext.SaveChangesAsync();
			}
			catch (DbUpdateException ex)
			{
				throw new DatabaseOperationException($"error occurred while updating planogram: {ex}");
			}
		}

		public bool IsPlanogramExists(int kioskId, int machineId, int trayId, int beltId)
		{
			return _dbContext.Planogram.Any(x => x.KioskId == kioskId && x.MachineId == machineId && x.TrayId == trayId && x.BeltId == beltId);
		}
		public async Task<int> DeleteBelt(Planogram planogram)
		{
			try
			{
				_dbContext.Planogram.Remove(planogram);
				return await _dbContext.SaveChangesAsync();
			}
			catch (DbUpdateException ex)
			{
				throw new DatabaseOperationException($"error occurred while deleting planogram: {ex}");
			}
		}
		public async Task<int> DeletePlanograms(List<int> planogramIds)
		{
			try
			{
				return await _dbContext.Planogram.Where(x => planogramIds.Contains(x.PlanogramId)).ExecuteDeleteAsync();
			}
			catch (DbUpdateException ex)
			{
				throw new DatabaseOperationException($"error occurred while deleting planogram: {ex}");
			}
		}

		public async Task<List<Planogram>> GetPlanogram(int kioskId, int machineId, int trayId)
		{
			return await _dbContext.Planogram.Where(x => x.KioskId == kioskId && x.MachineId == machineId && x.TrayId == trayId).ToListAsync();
		}
		public async Task<List<Planogram>> GetPlanogram(int kioskId, int machineId)
		{
			return await _dbContext.Planogram.Where(x => x.KioskId == kioskId && x.MachineId == machineId).ToListAsync();
		}
	}
}
