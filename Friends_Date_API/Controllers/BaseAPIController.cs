using Friends_Date_API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Friends_Date_API.Controllers
{
    // specify action filter class here because every request will hit this class
    [ServiceFilter(typeof(LogUserActivity))] 
    [ApiController]
    [Route("api/[Controller]")]
    public class BaseAPIController : ControllerBase
    {
       
    }
}
