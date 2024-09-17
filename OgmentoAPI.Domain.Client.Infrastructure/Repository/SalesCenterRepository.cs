﻿using Microsoft.EntityFrameworkCore;
using OgmentoAPI.Domain.Client.Abstractions.DataContext;
using OgmentoAPI.Domain.Client.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OgmentoAPI.Domain.Client.Infrastructure.Repository
{
    public class SalesCenterRepository : ISalesCenterRepository
    {
        private readonly ClientDBContext _Context;
        public SalesCenterRepository(ClientDBContext Context)
        {
            _Context = Context;
        }


        public IEnumerable<int> GetSalesCenterIds(Expression<Func<SalesCenterUserMapping, bool>> predicate)
        {
            var salesCenterIds = _Context.SalesCenterUserMapping
                .AsNoTracking()
                .Where(predicate)
                .Select(mapping => mapping.SalesCenterId)
                .ToList();

            return salesCenterIds;
        }

        public IEnumerable<SalesCenter> GetSalesCenterDetails(Expression<Func<SalesCenterUserMapping, bool>> predicate)
        {
            var salesCenterIds = GetSalesCenterIds(predicate);
            return _Context.SalesCenter
                .AsNoTracking()
                .Where(x => salesCenterIds.Contains(x.ID))
                .ToList();

        }
    }
}
