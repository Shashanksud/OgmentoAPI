using OgmentoAPI.Domain.Catalog.Abstractions.Models;
using OgmentoAPI.Domain.Catalog.Abstractions.Services;
using OgmentoAPI.Domain.Client.Abstractions.DataContext;
using OgmentoAPI.Domain.Client.Abstractions.Models.Planogram;
using OgmentoAPI.Domain.Client.Abstractions.Repositories;
using OgmentoAPI.Domain.Client.Abstractions.Service;
using OgmentoAPI.Domain.Common.Abstractions.CustomExceptions;
using OgmentoAPI.Domain.Common.Abstractions.Dto;

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
			List<(int beltId, bool BeltIsActive)> beltIds = _planogramRepository.GetBeltIds(kioskId,machineId, trayId);
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
			Planogram? planogram = await _planogramRepository.GetPlanogram(kioskId, machineId, trayId, beltId);
			if (planogram == null)
			{
				throw new EntityNotFoundException("Unable to find the entry in database");
			}
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
				Quantity = planogram.Quantity,
			};
		}
		public async Task<List<TrayPogModel>> GetTrays(int machineId, int kioskId)
		{
			List<TrayPogModel> trayPogModels = new List<TrayPogModel>();
			List<(int TrayId,bool TrayIsActive)> TrayIds = _planogramRepository.GetTrayIds(kioskId, machineId);
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
			List<int> machineIds = _planogramRepository.GetMachineIds(kioskId.Value);
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
		public async Task<ResponseDto> SaveOrUpdatePOG(AddPogModel addPogModel)
		{
			int? kioskId = await _kioskService.GetKioskId(addPogModel.KioskName);
			if (!kioskId.HasValue)
			{
				throw new EntityNotFoundException($"{addPogModel.KioskName} not found in database.");
			}
			int productId = await _productServices.GetProductId(addPogModel.ProductSku);
			Planogram planogram = new Planogram
			{
				KioskId = kioskId.Value,
				MachineId = addPogModel.MachineId,
				TrayId = addPogModel.TrayId,
				BeltId = addPogModel.BeltId,
				ProductId = productId,
				BeltIsActive = addPogModel.BeltIsActive,
				TrayIsActive = addPogModel.BeltIsActive,
				Quantity = addPogModel.Quantity,
				MaxQuantity = addPogModel.MaxQuantity,
			};
			int rowsAffected = 0;
			if (_planogramRepository.IsPlanogramExists(kioskId.Value, addPogModel.MachineId, addPogModel.TrayId, addPogModel.BeltId))
			{
				rowsAffected = await _planogramRepository.UpdatePOG(planogram);
			}
			else
			{
				rowsAffected = await _planogramRepository.AddPOG(planogram);
			}
			return new ResponseDto
			{
				IsSuccess = rowsAffected > 0,
				ErrorMessage = (rowsAffected > 0) ? "No error" : "Unexpected error occured"
			};
		}

		public async Task<ResponseDto> DeleteBelt(DeletePogModel pogModel)
		{
			int? kioskId = await _kioskService.GetKioskId(pogModel.KioskName);
			if (!kioskId.HasValue)
			{
				throw new EntityNotFoundException($"{pogModel.KioskName} not found in database.");
			}
			if(pogModel.ProductSku == null || pogModel.MachineId == null || pogModel.TrayId == null || pogModel.BeltId==null)
			{
				throw new InvalidOperationException("ProductSku, MachineId, TrayId or beltId can't be null while deleting belt.");
			}
			int productId = await _productServices.GetProductId(pogModel.ProductSku);
			Planogram? planogram = await _planogramRepository.GetPlanogram(kioskId.Value, pogModel.MachineId.Value, pogModel.TrayId.Value, pogModel.BeltId.Value);
			if(planogram == null || planogram.ProductId != productId)
			{
				throw new EntityNotFoundException("Unable to find the entry in database");
			}
			int rowsAffected = await _planogramRepository.DeleteBelt(planogram);
			return new ResponseDto
			{
				IsSuccess = rowsAffected > 0,
				ErrorMessage = (rowsAffected > 0) ? "No error" : "Unexpected error occured"
			};
		}

		public async Task<ResponseDto> DeleteTray(DeletePogModel pogModel)
		{
			int? kioskId = await _kioskService.GetKioskId(pogModel.KioskName);
			if (!kioskId.HasValue)
			{
				throw new EntityNotFoundException($"{pogModel.KioskName} not found in database.");
			}
			if (pogModel.MachineId == null || pogModel.TrayId == null) {
				throw new InvalidOperationException("MachineId or TrayId can't be null while deleting Tray.");
			}
			List<int> planograms = (await _planogramRepository.GetPlanogram(kioskId.Value, pogModel.MachineId.Value,pogModel.TrayId.Value)).Select(x=>x.PlanogramId).ToList();
			int rowsAffected = await _planogramRepository.DeletePlanograms(planograms);
			return new ResponseDto
			{
				IsSuccess = rowsAffected > 0,
				ErrorMessage = (rowsAffected > 0) ? "No error" : "Unexpected error occured"
			};
		}
		public async Task<ResponseDto> DeleteMachine(DeletePogModel pogModel)
		{
			int? kioskId = await _kioskService.GetKioskId(pogModel.KioskName);
			if (!kioskId.HasValue)
			{
				throw new EntityNotFoundException($"{pogModel.KioskName} not found in database.");
			}
			if (pogModel.MachineId == null)
			{
				throw new InvalidOperationException("MachineId can't be null while deleting Tray.");
			}
			List<int> planograms = (await _planogramRepository.GetPlanogram(kioskId.Value, pogModel.MachineId.Value)).Select(x => x.PlanogramId).ToList();
			int rowsAffected = await _planogramRepository.DeletePlanograms(planograms);
			return new ResponseDto
			{
				IsSuccess = rowsAffected > 0,
				ErrorMessage = (rowsAffected > 0) ? "No error" : "Unexpected error occured"
			};
		}

	}
}
