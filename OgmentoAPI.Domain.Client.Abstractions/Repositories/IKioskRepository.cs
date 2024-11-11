using OgmentoAPI.Domain.Client.Abstractions.DataContext;
using OgmentoAPI.Domain.Client.Abstractions.Models;

namespace OgmentoAPI.Domain.Client.Abstractions.Repositories
{
	public interface IKioskRepository
    {
        List<KioskModel> GetKioskDetails();
        public Task UpdateKioskDetails(string kioskName, int salesCenterId);
		public Task DeleteKioskByName(string kioskName);
		List<KioskModel> GetKioskDetails(List<int> salesCenterIds);
		public Task AddKiosk(KioskModel kioskModel);
		Task<int?> GetKioskId(string kioskName);
		Task<Kiosk> GetKiosk(int kioskId);
		bool GetKioskCountBySalesCenter(int salesCenterId);
	}
}
