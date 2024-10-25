using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OgmentoAPI.Domain.Client.Abstractions.Dto;
using OgmentoAPI.Domain.Client.Abstractions.Models;
using OgmentoAPI.Domain.Client.Abstractions.Service;

namespace OgmentoAPI.Domain.Client.Api
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]
	public class SalesCenterController : ControllerBase
	{
		private readonly ISalesCenterService _salesCenterService;
		public SalesCenterController(ISalesCenterService salesCenterService)
		{
			_salesCenterService = salesCenterService;
		}

		[HttpGet]
		public IActionResult GetAllSalesCenters()
		{
			var result = _salesCenterService.GetAllSalesCenters();
			return Ok(result); 
		}
		[Route("UpdateMainSalesCenter")]
		[HttpPost]
		public IActionResult UpdateMainSalesCenter(SalesCentersDto salesCentersDto)
		{
			var model = salesCentersDto.ToModel(); 
			var response = _salesCenterService.UpdateMainSalesCenter(model);
			return Ok(response);
		}

		[HttpPost]
		[Route("AddSalesCenter")]
		public IActionResult AddSalesCenter(SalesCentersDto salesCenterDto)
		{
			int? result = _salesCenterService.AddSalesCenter(salesCenterDto);
			if (result.HasValue)
			{
				return Ok(result);
			}
			return BadRequest("Sales center already exists");
		}

		[Route("delete/{salesCenterUid}")]
		[HttpDelete]
		public IActionResult DeleteSalesCenter(Guid salesCenterUid)
		{
			int? response = _salesCenterService.DeleteSalesCenter(salesCenterUid);
			return Ok(response);
		}
	}

}
