using OA.Core;
using OA.Core.ApplicationToController;
using OA.Domain.Inform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Application
{
    [OpenServiceApi]
    public class InformTypeService
    {
        IInformRepository _informRepository;
        public InformTypeService(IInformRepository informRepository)
        {
            _informRepository = informRepository;
        }

        public async Task<List<InformType>> Get(string cname)
        {
            try
            {
                return await _informRepository.ListAsync<InformType>(x => x.PhId > 0, true);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message);
            }

            return new List<InformType>();
        }

    }
}
