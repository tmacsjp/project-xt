using Microsoft.EntityFrameworkCore;
using OA.Core;
using OA.Domain.Inform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure
{
    public class BusinessDbContext : AdoDbContext
    {
        public DbSet<InformModel> InformModels { get; set; }

        public DbSet<InformType> InformTypes { get; set; }

        public BusinessDbContext(DbContextOptions<BusinessDbContext> options) : base(options)
        {

        }
    }

    
}
