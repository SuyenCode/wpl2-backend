using ClassLibTeam12.Business;
using ClassLibTeam12.Business.Entities;
using ClassLibTeam12.Data.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApiTeam12.ViewModel;

namespace WebApiTeam12.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SollicitatieController : ControllerBase
    {
        private readonly EmailService _emailService;

        public SollicitatieController()
        {
            _emailService = new EmailService();
        }

        [HttpGet("filter")]
        public IActionResult FilterSollicitaties(
            [FromQuery] int? sollicitatieId = null,
    [FromQuery] string? volledigeNaam = null,
    [FromQuery] string? jobNaam = null,
    [FromQuery] bool? status = null,
    [FromQuery] DateTime? vanafDatum = null,
    [FromQuery] DateTime? totDatum = null)
        {
            try
            {

                var result = Sollicitaties.GetFilteredSollicitaties(sollicitatieId, volledigeNaam, jobNaam, status, vanafDatum, totDatum);

                if (!result.Succeeded)
                    return BadRequest("Sollicitaties konden niet gefilterd worden.");

                var lijst = Sollicitaties.ProcessAllSollicitaties(result.DataTable);
                return Ok(lijst);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fout bij filteren: {ex.Message}");
            }
        }
        [HttpPost("nieuwe-sollicitatie")]
        [Authorize]
        public async Task<IActionResult> NieuweSollicitatie([FromBody] SollicitatieViewModel model)
        {
            var gebruikerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(gebruikerId, out int gebruikerID))
                return Unauthorized("Gebruiker niet geldig.");

            var gebruiker = Gebruikers.GetById(gebruikerID);

            if (gebruiker == null || string.IsNullOrEmpty(gebruiker.Email))
                return BadRequest("Gebruiker niet gevonden of geen geldig e-mailadres.");

            var result = Sollicitaties.AddMail(DateTime.Now, gebruikerID, model.JobId);

            if (!result.Succeeded)
                return BadRequest("Sollicitatie mislukt.");

            await _emailService.SendEmailAsync(
                gebruiker.Email,
                "Bevestiging Sollicitatie",
                $"<h3>Uw sollicitatie is succesvol!</h3><p>Voor vacature: {model.JobNaam}</p>"
            );

            return Ok("Sollicitatie succesvol en bevestiging verzonden.");
        }


        [HttpGet("all-sollicitaties")]
        //[Route]
        public IActionResult GetAllSollicitaties()
        {
            try
            {
                var result = Sollicitaties.GetAllSollicitaties();
                if (result.Succeeded)
                {
                    var allSollicitaties = Sollicitaties.ProcessAllSollicitaties(result.DataTable);
                    return Ok(allSollicitaties);
                }
                else
                {
                    return BadRequest(result.AddError);
                }
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Interne serverfout: {ex.Message}");
            }
        }

        [HttpPost("sollicitatie-delete")]
        [Authorize]
        public IActionResult AnnuleerSollicitatie([FromBody] SollicitatieDeleteModel model)
        {
            try
            {
                UpdateResult result = Sollicitaties.DeleteSollicitaties(model.sollicitatieID);

                if (result.Succeeded)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest(result.Succeeded);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fout bij het deleten {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("bulk-delete-sollicitaties")]
        public IActionResult BulkDeleteTrainingen([FromQuery] List<int> sollicitatiesIDs)
        {
            if (sollicitatiesIDs == null || !sollicitatiesIDs.Any())
                return BadRequest("Geen geldige sollicitatiesIDs opgegeven.");

            var result = Sollicitaties.DeleteBulkSollicitaties(sollicitatiesIDs);
            if (result.Succeeded)
                return Ok("Sollicitaties succesvol verwijderd");
            return BadRequest(result.AddError);
        }


        [HttpPost("sollicitatie-add")]
        [Authorize]
        public IActionResult AddSollicitatie(SollicatieViewModel addNewModel)
        {
            try
            {
                InsertResult result = Sollicitaties.Add(addNewModel.Datum, addNewModel.GebruikerID, addNewModel.JobID, addNewModel.Status);
                if (result.Succeeded)
                {
                    return Ok(addNewModel);
                }
                else
                {
                    return BadRequest(result.AddError);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fout bij het invoegen {ex.Message}");
            }
        }

        [HttpPut("update-sollicitatie")]
        [Authorize(Roles = "Admin")] // Alleen Admins mogen inschrijvingen wijzigen
        [Authorize]
        public IActionResult UpdateSollicitatie([FromBody] UpdateSollicitatieViewModel model)
        {
            // Controleer het model
            if (model == null || model.SollicitatieID == null || model.GebruikerID == null || model.JobID == null || model.Status == null)
            {
                return BadRequest("Alle velden vereist.");
            }

            // Voer de update uit
            var result = Sollicitaties.UpdateSollicitatieAdmin(model.SollicitatieID, model.GebruikerID, model.JobID, model.Status);

            return result.Succeeded ? Ok("Sollicitatie succesvol bijgewerkt.") : BadRequest("Error tijdens het bewerken van sollicitatie.");
        }

        [HttpGet("by-id")]
        public IActionResult GetById(int sollicitatieId)
        {
            try
            {
                var result = Sollicitaties.GetSollicitatieById(sollicitatieId);
                if (result.Succeeded)
                {
                    return Ok(Inschrijvingen.ConvertDataTableToJson(result.DataTable));
                }
                else
                {
                    return BadRequest(result.AddError);
                }
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Interne serverfout: {ex.Message}");
            }
        }
    }
}
