using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Friends_Date_API.Controllers
{
    /*
        After Publishing our app api Server only know about api/controller it does not know about 
        client page to solve this we need this class

        whenever API do not found any route it will fall back index.html file from wwwroot which file type is text/html
        then angular know how to control root
    */
    public class FallbackController : Controller
    {
        public ActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), 
                "wwwroot", "index.html"), "text/HTML");
        }
    }
}
