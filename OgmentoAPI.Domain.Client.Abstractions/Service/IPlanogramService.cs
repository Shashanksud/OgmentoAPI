using OgmentoAPI.Domain.Client.Abstractions.Models.Planogram;
using OgmentoAPI.Domain.Common.Abstractions.Dto;

namespace OgmentoAPI.Domain.Client.Abstractions.Service
{
	public interface IPlanogramService
	{
		Task<ResponseDto> SaveOrUpdatePOG(AddPogModel addPogModel);
		Task<PlanogramModel> GetPOG(string kioskName);
		Task<ResponseDto> DeleteBelt(DeletePogModel pogModel);
		Task<ResponseDto> UpdateBeltTrayActiveStatus(StatusPogModel trayStatus);
		//Task<ResponseDto> DeleteTray(DeletePogModel pogModel);
		//Task<ResponseDto> DeleteMachine(DeletePogModel pogModel);
	}
}
