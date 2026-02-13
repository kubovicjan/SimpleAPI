using Microsoft.AspNetCore.Mvc;
using SimpleAPI.Core.Attributes;
using SimpleAPI.Services;

namespace SimpleAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiKey]
public class CardsController : ControllerBase
{
    private ICardsService CardsService { get; }

    public CardsController(ICardsService cardsService)
    {
        CardsService = cardsService;
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetCardState(string id)
    {
        var state = await CardsService.GetCardInfo(id);
        return Ok(state);
    }
}
