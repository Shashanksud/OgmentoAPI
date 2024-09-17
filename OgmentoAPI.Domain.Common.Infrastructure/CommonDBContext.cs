﻿using Microsoft.EntityFrameworkCore;
using OgmentoAPI.Domain.Client.Abstractions.DataContext;
using OgmentoAPI.Domain.Common.Abstractions.DataContext;

namespace OgmentoAPI.Domain.Common.Infrastructure
{
    public class CommonDBContext:DbContext
    {
        public CommonDBContext(DbContextOptions<CommonDBContext> options):base(options) { }
        public DbSet<Country> Countries { get; set; }

    }
}
