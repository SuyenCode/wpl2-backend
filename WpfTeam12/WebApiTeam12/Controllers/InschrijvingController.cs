using ClassLibTeam12.Business;
using ClassLibTeam12.Data.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiTeam12.ViewModel;
using ClassLibTeam12.Business.Validation;
using Microsoft.EntityFrameworkCore;
using ClassLibTeam12.Business.Entities;
using Microsoft.AspNetCore.Mvc.Infrastructure;


namespace WebApiTeam12.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InschrijvingController : ControllerBase
    {
        private readonly EmailService _emailService;

        public InschrijvingController()
        {
            _emailService = new EmailService(); 
        }

        [HttpGet("filter-inschrijvingen")]
        public IActionResult FilterInschrijvingen(
    [FromQuery] int? inschrijvingId = null,
    [FromQuery] string? volledigeNaam = null,
    [FromQuery] string? trainingNaam = null,
    [FromQuery] bool? status = null,
    [FromQuery] DateTime? vanafDatum = null,
    [FromQuery] DateTime? totDatum = null)
        {
            try
            {
                var result = Inschrijvingen.GetFilteredInschrijvingen(inschrijvingId, volledigeNaam, trainingNaam, status, vanafDatum, totDatum);

                if (!result.Succeeded)
                    return BadRequest("Inschrijvingen konden niet gefilterd worden.");

                var lijst = Inschrijvingen.ProcessAllInschrijvingen(result.DataTable);
                return Ok(lijst);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fout bij filteren: {ex.Message}");
            }
        }


        [HttpPost("nieuwe-inschrijving")]
        [Authorize]
        public async Task<IActionResult> NieuweInschrijving([FromBody] InschrijvingViewModel model)
        {
            var gebruikerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(gebruikerId, out int gebruikerID))
                return Unauthorized("Gebruiker niet geldig.");

            
            var gebruiker = Gebruikers.GetById(gebruikerID); 

            if (gebruiker == null || string.IsNullOrEmpty(gebruiker.Email))
                return BadRequest("Gebruiker niet gevonden of geen geldig e-mailadres.");

            
            var result = Inschrijvingen.AddEmail(DateTime.Now, gebruikerID, model.TrainingId);

            if (!result.Succeeded)
                return BadRequest("Inschrijving mislukt.");

            var result2 = Inschrijvingen.getTrainingNaam(model.TrainingId, gebruikerID);

            var row = result2.DataTable.Rows[0];
            string naam = row["Naam"]?.ToString() ?? ""; ;

            await _emailService.SendEmailAsync(
                gebruiker.Email,
                "Bevestiging Inschrijving",
                $"<h3>Uw inschrijving is succesvol!</h3><p>Voor training: {naam}</p>"
            );

            return Ok("Inschrijving succesvol en bevestiging verzonden.");
        }


        [HttpGet("all-inschrijvingen")]
        //[Route]
        public IActionResult GetAllInschrijvingen()
        {
            try
            {
                var result = Inschrijvingen.GetAllInschrijvingen();
                if (result.Succeeded)
                {
                    var inschrijving = Inschrijvingen.ProcessAllInschrijvingen(result.DataTable);
                    return Ok(inschrijving);
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
        [HttpPost("Inschrijvingen-Add")]
        [Authorize]
        public IActionResult AddInschrijvingen(InschrijvingAddModel newModel)
        {
            try
            {
                InsertResult result = Inschrijvingen.Add(newModel.GebruikerID, newModel.TrainingID, DateTime.Now, newModel.Status);
                if (result.Succeeded)
                {
                    return Ok(newModel);
                }
                else
                {
                    return BadRequest(result.AddError);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }


        [HttpPost("annuleer-inschrijving")]
        [Authorize]
        public IActionResult AnnuleerInschrijving([FromBody] int inschrijvingID)
        {
            try
            {
                // Haal gebruikerID uit JWT claims
                var gebruiker = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(gebruiker))
                    return Unauthorized("Gebruiker niet herkend.");

                if (!int.TryParse(gebruiker, out int gebruikerID))
                    return BadRequest("Ongeldige gebruikerID in token.");

                UpdateResult result = Inschrijvingen.CancelInschrijving(inschrijvingID, gebruikerID);
                if (result.Succeeded)
                {
                    return Ok($"Inschrijving {inschrijvingID} geannuleerd.");
                }
                else
                {
                    return BadRequest("Annuleren mislukt: inschrijving bestaat niet of is al compleet.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Interne serverfout: {ex.Message}");
            }
        }

        [HttpPut("update-inschrijving")]
        [Authorize(Roles = "Admin")] // Alleen Admins mogen inschrijvingen wijzigen
        [Authorize]
        public IActionResult UpdateInschrijving([FromBody] UpdateInschrijvingViewModel model)
        {
            // Controleer het model
            if (model == null || model.InschrijvingID == null || model.GebruikerID == null || model.TrainingID == null || model.Status == null)
            {
                return BadRequest("Alle velden vereist.");
            }

            // Voer de update uit
            var result = Inschrijvingen.UpdateInschrijvingAdmin(model.InschrijvingID, model.GebruikerID, model.TrainingID, model.Status);

            return result.Succeeded ? Ok("Inschrijving succesvol bijgewerkt.") : BadRequest("Error tijdens het bewerken van inschrijving.");
        }

        [HttpGet("by-id")]
        public IActionResult GetById(int inschrijvingId)
        {
            try
            {
                var result = Inschrijvingen.GetInschrijvingById(inschrijvingId);
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

        

        [HttpPost("Inschrijving-delete")]
        [Authorize]
        public IActionResult AnnuleerInschrijving([FromBody] InschrijvingDeleteModel model)
        {
            try
            {
                UpdateResult result = Inschrijvingen.DeleteInschrijvingen(model.inschrijvingID);

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
        [HttpDelete("bulk-delete-Inschrijvingen")]
        public IActionResult BulkDeleteTrainingen([FromQuery] List<int> inschrijvingIDs)
        {
            if (inschrijvingIDs == null || !inschrijvingIDs.Any())
                return BadRequest("Geen geldige inschrijvingIDs opgegeven.");

            var result = Inschrijvingen.DeleteBulkInschrijving(inschrijvingIDs);
            if (result.Succeeded)
                return Ok("Inschrijvingen succesvol verwijderd");
            return BadRequest(result.AddError);
        }
    }
}
