using Microsoft.AspNetCore.Mvc;

namespace NetCore.Host.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemoController : ControllerBase
{
    [HttpGet("demo")]
    public IActionResult GetDemo()
    {
        return new OkResult();
    }

    [HttpGet("throw")]
    public IActionResult Throw()
    {
        throw new NullReferenceException();
    }
}