using Microsoft.EntityFrameworkCore;
using OA.Domain.Inform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure
{
    public class InformRepository : BaseRepository<BusinessDbContext, InformModel>, IInformRepository
    {
        BusinessDbContext _dbContext;
        public InformRepository(BusinessDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<InformModel>> GetList(bool tracking = false)
        {
            var query = _dbContext.InformModels.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();
            return await query.ToListAsync();
        }

    }
}
