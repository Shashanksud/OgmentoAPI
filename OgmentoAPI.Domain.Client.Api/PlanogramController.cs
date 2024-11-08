using Microsoft.AspNetCore.Mvc;
using OgmentoAPI.Domain.Client.Abstractions.Dto;
using OgmentoAPI.Domain.Client.Abstractions.Dto.Planogram;
using OgmentoAPI.Domain.Client.Abstractions.Service;
using OgmentoAPI.Domain.Common.Abstractions.Dto;

namespace OgmentoAPI.Domain.Client.Api
{
	[ApiController]
	[Route("api/[controller]")]
	public class PlanogramController: ControllerBase
	{
		private readonly IPlanogramService _planogramService;
		public PlanogramController(IPlanogramService planogramService)
		{
			_planogramService = planogramService;

		}
		[HttpPut]
		public async Task<IActionResult> SaveOrUpdatePOG(AddPogDto addPogDto)
		{
			ResponseDto response = await _planogramService.SaveOrUpdatePOG(addPogDto.ToModel());
			if (response.IsSuccess)
			{
				return Ok(response);
			}
			else
			{
				return BadRequest(response);
			}
		}
		[HttpGet]
		[Route("{kioskName}")]
		public async Task<IActionResult> GetPOG(string kioskName)
		{
			return Ok((await _planogramService.GetPOG(kioskName)).ToDto());
		}
		[HttpDelete]
		[Route("belt")]
		public async Task<IActionResult> DeleteBelt(DeletePogDto pogDto)
		{
			ResponseDto response = await _planogramService.DeleteBelt(pogDto.ToModel());
			if (response.IsSuccess)
			{
				return Ok(response);
			}
			else
			{
				return BadRequest(response);
			}
		}
		[HttpPut]
		[Route("tray/status")]
		public async Task<IActionResult> UpdateTrayStatus(StatusPogDto trayStatus)
		{
			ResponseDto response = await _planogramService.UpdateBeltTrayActiveStatus(trayStatus.ToModel());
			if (response.IsSuccess)
			{
				return Ok(response);
			}
			else
			{
				return BadRequest(response);
			}
		}
		[HttpPut]
		[Route("belt/status")]
		public async Task<IActionResult> UpdateBeltStatus(StatusPogDto beltStatus)
		{
			if(beltStatus.BeltId == null)
			{
				throw new InvalidOperationException("beltId cannot be null while deleting a belt");
			}
			ResponseDto response = await _planogramService.UpdateBeltTrayActiveStatus(beltStatus.ToModel());
			if (response.IsSuccess)
			{
				return Ok(response);
			}
			else
			{
				return BadRequest(response);
			}
		}
		//[HttpDelete]
		//[Route("tray")]
		//public async Task<IActionResult> DeleteTray(DeletePogDto pogDto)
		//{
		//	ResponseDto response = await _planogramService.DeleteTray(pogDto.ToModel());
		//	if (response.IsSuccess)
		//	{
		//		return Ok(response);
		//	}
		//	else
		//	{
		//		return BadRequest(response);
		//	}
		//}
		//[HttpDelete]
		//[Route("machine")]
		//public async Task<IActionResult> DeleteMachine(DeletePogDto pogDto)
		//{
		//	ResponseDto response = await _planogramService.DeleteMachine(pogDto.ToModel());
		//	if (response.IsSuccess)
		//	{
		//		return Ok(response);
		//	}
		//	else
		//	{
		//		return BadRequest(response);
		//	}
		//}
	}
}
