using ClassLibTeam12.Business;
using ClassLibTeam12.Business.Services;
using ClassLibTeam12.Data.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace WebApiTeam12.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BestellingenController : ControllerBase
    {

        private readonly BestellingenService _bestellingenService;

        public BestellingenController()
        {
            _bestellingenService = new BestellingenService();
        }

        // Controllers/BestellingenController.cs
        [HttpGet("me")]
        public IActionResult MijnBestellingen()
        {
            var gebruiker = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(gebruiker))
                return Unauthorized("Gebruiker niet herkend.");

            if (!int.TryParse(gebruiker, out int gebruikerId))
                return BadRequest("Ongeldige gebruikerID in token.");

            // Gebruik het juiste return type (List<BestellingDTO>)
            var bestellingen = _bestellingenService.GetBestellingenVoorGebruiker(gebruikerId);
            return Ok(bestellingen);
        }

        [HttpGet("filter")]
        [Authorize]
        public IActionResult FilterBestellingen(
     [FromQuery] string? type,
     [FromQuery] string? naam,              
     [FromQuery] bool? status,
     [FromQuery] DateTime? vanafDatum,
     [FromQuery] DateTime? totDatum)
        {
            var gebruiker = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(gebruiker, out int gebruikerId))
                return BadRequest("Ongeldige gebruikerID.");

            var result = _bestellingenService.GetFilteredBestellingen(gebruikerId, type, naam, status, vanafDatum, totDatum);
            return Ok(result);
        }
    }
}
