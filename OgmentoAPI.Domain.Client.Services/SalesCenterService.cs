﻿using OgmentoAPI.Domain.Client.Abstractions.DataContext;
using OgmentoAPI.Domain.Client.Abstractions.Dto;
using OgmentoAPI.Domain.Client.Abstractions.Models;
using OgmentoAPI.Domain.Client.Abstractions.Repositories;
using OgmentoAPI.Domain.Client.Abstractions.Service;
using OgmentoAPI.Domain.Client.Infrastructure.Repository;
using OgmentoAPI.Domain.Common.Abstractions.CustomExceptions;
using System.Linq.Expressions;

namespace OgmentoAPI.Domain.Client.Services
{
	public class SalesCenterService : ISalesCenterService
	{
		private readonly ISalesCenterRepository _salesCenterRepository;
		private readonly IKioskRepository _kioskRepository;

		public SalesCenterService(ISalesCenterRepository salesCenterRepository, IKioskRepository kioskRepository)
		{
			_salesCenterRepository = salesCenterRepository;
			_kioskRepository = kioskRepository;
		}
		public IEnumerable<SalesCenter> GetSalesCenterForUser(int Id)
		{
			Expression<Func<SalesCenterUserMapping, bool>> predicate = (mapping => mapping.UserId == Id);
			return _salesCenterRepository.GetSalesCenter(predicate);
		}
		public List<SalesCenterModel> GetAllSalesCenters()
		{
			IEnumerable<SalesCenterModel> salesCenters = _salesCenterRepository.GetSalesCenterDetails();
			return salesCenters.ToList();

		}

		public int? UpdateSalesCenters(int userId, List<Guid> guids)
		{

			try
			{
				return _salesCenterRepository.UpdateSalesCentersForUser(userId, guids);

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public int? UpdateMainSalesCenter(SalesCenterModel salesCenterModel)
		{
			try
			{
				return _salesCenterRepository.UpdateMainSalesCenter(salesCenterModel);
			}
			catch (Exception ex)
			{
				throw ex;

			}
		}
		public int? AddSalesCenter(SalesCentersDto salesCenterDto)
		{
			try
			{
				return _salesCenterRepository.AddSalesCenter(salesCenterDto);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}


		public int? DeleteSalesCenter(Guid salesCenterUid)
		{

			int salesCenterUserCount = _salesCenterRepository.GetUserSalesCenterMappingId(salesCenterUid);
			if (salesCenterUserCount > 0)
			{
				throw new ValidationException("Cannot delete SalesCenter: it has associated User.");
			}
			int salesCenterId = _salesCenterRepository.GetSalesCenterDetail(salesCenterUid).ID;
			bool linkedKioskCount = _kioskRepository.GetKioskCountBySalesCenter(salesCenterId);
			if (linkedKioskCount)
			{
				throw new ValidationException("Cannot delete SalesCenter: it has associated kiosk.");
			}

			return _salesCenterRepository.DeleteSalesCenter(salesCenterUid);

		}



		public SalesCenter GetSalesCenterDetail(Guid salesCenterUid)
		{
			return _salesCenterRepository.GetSalesCenterDetail(salesCenterUid);
		}
		//public int GetUserSalesCenterMappingId(Guid salesCenterUid)
		//{
		//    {
		//        try
		//        {
		//         return   _salesCenterRepository.GetUserSalesCenterMappingId(salesCenterUid);
		//        }
		//        catch (Exception ex)
		//        {
		//            throw ex;
		//        }
		//    }
		//}
		public void DeleteSalesCenterUserMapping(int userId)
		{
		  _salesCenterRepository.DeleteSalesCenterUserMapping(userId);
		}

        //public int GetUserSalesCenterMappingId(Guid salesCenterUid)
        //{
        //    {
        //        try
        //        {
        //         return   _salesCenterRepository.GetUserSalesCenterMappingId(salesCenterUid);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }
        //}
		public async Task<SalesCenterModel> GetSalesCenter(int salesCenterId)
		{
			SalesCenter salesCenter = await _salesCenterRepository.GetSalesCenter(salesCenterId);
			return new SalesCenterModel
			{
				SalesCenterName = salesCenter.SalesCenterName,
				SalesCenterUid = salesCenter.SalesCenterUid,
				City = salesCenter.City,
				CountryId = salesCenter.CountryId,
			};
		}
    }
}