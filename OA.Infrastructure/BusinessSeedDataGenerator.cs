using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure
{
    public class BusinessSeedDataGenerator: ISeedDataGenerator
    {
        BusinessDbContext _dbContext;
        public BusinessSeedDataGenerator(BusinessDbContext businessDbContext)
        {
            _dbContext = businessDbContext;
        }
        public async Task Execute()
        {
            var sql = _dbContext.Database.GenerateCreateScript();
            int k = 0;



            await _dbContext.SaveChangesAsync();
        }
    }

    public interface ISeedDataGenerator
    {
        Task Execute();
    }

}
