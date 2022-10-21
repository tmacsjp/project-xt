using Microsoft.AspNetCore.Mvc;
using OA.API.Filter;
using OA.Core;
using OA.Domain.Inform;

namespace OA.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class InformController: ControllerBase
    {
        IInformRepository _informRepository;
        public InformController(IInformRepository informRepository)
        { 
            _informRepository = informRepository;
        }

        [HttpPost]
        public async Task<List<InformModel>> Get(string cname)
        {
            try
            {
                //throw new ApiException("wawa");

                return await _informRepository.GetList();
            }
            catch (Exception ex)
            {
               throw new ApiException(ex.Message);
            }

            return new List<InformModel>();
        }
    }
}
