using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

[ApiController]
[Route("api/[controller]")]
public class HubsController : ControllerBase
{
    private readonly ILogger<HubsController> _logger;
    private readonly APS _aps;

    public HubsController(ILogger<HubsController> logger, APS aps)
    {
        _logger = logger;
        _aps = aps;
    }

    [HttpGet()]
    public async Task<ActionResult<string>> ListHubs()
    {
        var tokens = await AuthController.PrepareTokens(Request, Response, _aps);
        if (tokens == null)
        {
            return Unauthorized();
        }
        var hubs = await _aps.GetHubs(tokens);
        return JsonConvert.SerializeObject(hubs);
    }

    [HttpGet("{hub}/projects")]
    public async Task<ActionResult<string>> ListProjects(string hub)
    {
        var tokens = await AuthController.PrepareTokens(Request, Response, _aps);
        if (tokens == null)
        {
            return Unauthorized();
        }
        var projects = await _aps.GetProjects(hub, tokens);
        return JsonConvert.SerializeObject(projects);
    }
}