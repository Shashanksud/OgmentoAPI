

using OgmentoAPI.Domain.Client.Abstractions.DataContext;
using OgmentoAPI.Domain.Client.Abstractions.Models;
using OgmentoAPI.Domain.Client.Abstractions.Repositories;
using OgmentoAPI.Domain.Client.Abstractions.Service;

namespace OgmentoAPI.Domain.Client.Services
{
	public class PlanogramService: IPlanogramService
	{
		private readonly IPlanogramRepository _planogramRepository;
		public PlanogramService(IPlanogramRepository planogramRepository)
		{
			_planogramRepository = planogramRepository;
		}
		//public async Task<int> AddPOG(KioskPogModel kioskPog)
		//{

		//}
	}
}
