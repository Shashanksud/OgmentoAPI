using OgmentoAPI.Domain.Catalog.Abstractions.Models;
using OgmentoAPI.Domain.Catalog.Abstractions.Services;
using OgmentoAPI.Domain.Client.Abstractions.DataContext;
using OgmentoAPI.Domain.Client.Abstractions.Models;
using OgmentoAPI.Domain.Client.Abstractions.Repositories;
using OgmentoAPI.Domain.Client.Abstractions.Service;
using OgmentoAPI.Domain.Common.Abstractions.CustomExceptions;
using System.Diagnostics;

namespace OgmentoAPI.Domain.Client.Services
{
	public class PlanogramService: IPlanogramService
	{
		private readonly IPlanogramRepository _planogramRepository;
		private readonly IKioskService _kioskService;
		private readonly ISalesCenterService _salesCenterService;
		private readonly IProductServices _productServices;
		public PlanogramService(IPlanogramRepository planogramRepository, IKioskService kioskService, ISalesCenterService salesCenterService, IProductServices productServices)
		{
			_planogramRepository = planogramRepository;
			_kioskService = kioskService;
			_salesCenterService = salesCenterService;
			_productServices = productServices;
		}
		public async Task<List<BeltPogModel>> GetBelts(int kioskId, int machineId, int trayId)
		{
			List<BeltPogModel> beltPogModels = new List<BeltPogModel>();
			List<(int beltId, bool BeltIsActive)> beltIds = await _planogramRepository.GetBeltIds(kioskId,machineId, trayId);
			for(int i = 0; i < beltIds.Count; i++)
			{
				beltPogModels.Add(new BeltPogModel
				{
					BeltId = beltIds[i].beltId,
					BeltIsActive = beltIds[i].BeltIsActive,
					Product = await GetProduct( kioskId,machineId, trayId, beltIds[i].beltId),
				});
			}
			return beltPogModels;
		}
		public async Task<ProductPogModel> GetProduct(int kioskId, int machineId, int trayId, int beltId)
		{
			Planogram planogram = await _planogramRepository.GetProduct(kioskId, machineId, trayId, beltId);
			ProductBase product = await _productServices.GetProduct(planogram.ProductId);
			return new ProductPogModel
			{
				Price = product.Price,
				ExpiryDate = product.ExpiryDate,
				ProductDescription = product.ProductDescription,
				ProductName = product.ProductName,
				Weight = product.Weight,
				LoyaltyPoints = product.LoyaltyPoints,
				SkuCode = product.SkuCode,
				ProductId = planogram.ProductId,
				MaxQuantity = planogram.MaxQuantity,
				Quantity = planogram.Quatity,
			};
		}
		public async Task<List<TrayPogModel>> GetTrays(int machineId, int kioskId)
		{
			List<TrayPogModel> trayPogModels = new List<TrayPogModel>();
			List<(int TrayId,bool TrayIsActive)> TrayIds = await _planogramRepository.GetTrayIds(kioskId, machineId);
			for (int i = 0; i < TrayIds.Count; i++) {

				trayPogModels.Add(new TrayPogModel
				{
					TrayId = TrayIds[i].TrayId,
					TrayIsActive = TrayIds[i].TrayIsActive,
					Belt = await GetBelts(kioskId, machineId, TrayIds[i].TrayId)
				});
			}
			return trayPogModels;
		}

		public async Task<PlanogramModel> GetPOG(string kioskName)
		{
			PlanogramModel planogram = new PlanogramModel();
			planogram.Machines = new List<MachinePogModel>();
			int? kioskId = await _kioskService.GetKioskId(kioskName);
			if (!kioskId.HasValue)
			{
				throw new DatabaseOperationException($"{kioskName} not found in database.");
			}
			planogram.Location = await _kioskService.GetKiosk(kioskId.Value);
			List<int> machineIds = await _planogramRepository.GetMachineIds(kioskId.Value);
			for (int i = 0; i < machineIds.Count; i++)
			{
				planogram.Machines.Add(new MachinePogModel()
				{
					MachineId = machineIds[i],
					Trays = await GetTrays(machineIds[i], kioskId.Value),
				});
			}
			return planogram;
		}
		//public async Task<int> AddPOG(KioskPogModel kioskPog)
		//{

		//}
	}
}
