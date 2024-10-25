using Microsoft.AspNetCore.Mvc;
using OgmentoAPI.Domain.Client.Abstractions.DataContext;
using OgmentoAPI.Domain.Client.Abstractions.Dto;
using OgmentoAPI.Domain.Client.Abstractions.Service;

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
		//public async Task<IActionResult> AddPOG(KioskPogDto kioskPogDto)
		//{
		//	int response = await _planogramService.AddPOG(kioskPogDto.ToModel());
		//	return Ok(response);
		//}
	}
}
