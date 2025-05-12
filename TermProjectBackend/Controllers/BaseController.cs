using Microsoft.AspNetCore.Mvc;
using TermProjectBackend.Extensions;


namespace TermProjectBackend.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected int? CurrentUserId => User.GetUserId();
    }

}
