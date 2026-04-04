using Microsoft.AspNetCore.Mvc;

namespace ProductService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new string[] { "Product1", "Product2" });
    }
}