﻿using OgmentoAPI.Domain.Client.Abstractions.DataContext;
using OgmentoAPI.Domain.Client.Abstractions.Models;
using OgmentoAPI.Domain.Client.Abstractions.Repositories;
using OgmentoAPI.Domain.Client.Abstractions.Service;
using OgmentoAPI.Domain.Common.Abstractions.Enums;
using System.Linq.Expressions;

namespace OgmentoAPI.Domain.Client.Services
{
    public class SalesCenterService : ISalesCenterService
    {
        private readonly ISalesCenterRepository _salesCenterRepository;
        public SalesCenterService(ISalesCenterRepository salesCenterRepository)
        {
            _salesCenterRepository = salesCenterRepository;
        }
        public IEnumerable<SalesCenter> GetSalesCenter(int Id)
        {
            Expression<Func<SalesCenterUserMapping, bool>> predicate = (mapping => mapping.UserId == Id);
            return _salesCenterRepository.GetSalesCenter(predicate);
        }
        public List<SalesCenterModel> GetAllCenters()
        {
            var salesCenters = _salesCenterRepository.GetSalesCenterDetails();
            List<SalesCenterModel> salesCenterModel= new List<SalesCenterModel>();
            foreach(var centers in salesCenters)
            {
                salesCenterModel.Add(
                    new SalesCenterModel
                    {
                        CenterName = centers.SalesCenterName,
                        CenterUid = centers.SalesCenterUid.ToString(),
                        Country = Enum.GetName(typeof(Country),centers.CountryId),
                        City = centers.City,
                        CenterId = centers.ID
                    });
            }
            return salesCenterModel;
        }
    }
}
