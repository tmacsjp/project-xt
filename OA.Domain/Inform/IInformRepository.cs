using OA.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Domain.Inform
{
    public interface IInformRepository : IBaseRepository<InformModel>
    {
        Task<List<InformModel>> GetList(bool tracking = false);
    }
}
