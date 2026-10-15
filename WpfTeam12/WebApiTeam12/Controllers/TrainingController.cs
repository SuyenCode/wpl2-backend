using ClassLibTeam12.Business;
using ClassLibTeam12.Data.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiTeam12.ViewModel;

namespace WebApiTeam12.Controllers
{
    /// <summary>
    /// Deze controller beheert de acties met betrekking tot trainingen.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        /// <summary>
        /// Haalt een lijst van alle trainingen op.
        /// </summary>
        /// <returns>Een IActionResult met een lijst van trainingen.</returns>
        [HttpGet]
        public ActionResult GetTrainingen()
        {
            // Roep de business logic aan om de trainingen op te halen.
            var trainingen = Trainingen.GetTrainingen();
            return Ok(trainingen);
        }

        [HttpGet("all-trainingen")]
        //[Route]
        public IActionResult GetAllTrainingen()
        {
            try
            {
                var result = Trainingen.GetAllTrainingen();
                if (result.Succeeded)
                {
                    var allTrainingen = Trainingen.ProcessAllTrainingen(result.DataTable);
                    return Ok(allTrainingen);
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

        [HttpGet("filter")]
        public IActionResult GetFilteredTrainingen([FromQuery] int? trainingID, [FromQuery] string? naam, [FromQuery] string? beschrijving, [FromQuery] string? categorie, [FromQuery] string? bedrijfNaam, [FromQuery] bool? status, [FromQuery] string? duration)
        {
            trainingID = trainingID.HasValue ? trainingID : null;
            naam = string.IsNullOrWhiteSpace(naam) ? null : naam;
            beschrijving = string.IsNullOrWhiteSpace(beschrijving) ? null : beschrijving;
            categorie = string.IsNullOrWhiteSpace(categorie) ? null : categorie;
            bedrijfNaam = string.IsNullOrWhiteSpace(bedrijfNaam) ? null : bedrijfNaam;
            status = status.HasValue ? status : null;
            duration = string.IsNullOrWhiteSpace(duration) ? null : duration;

            var result = Trainingen.GetFilteredTrainingen(trainingID, naam, beschrijving, categorie, bedrijfNaam, status, duration);

            if (result.Succeeded && result.DataTable != null)
            {
                var filteredTrainingen = Trainingen.ProcessAllTrainingen(result.DataTable);

                var maskedTrainingen = filteredTrainingen.Select(training => new
                {
                    TrainingId = training.GetType().GetProperty("TrainingId")?.GetValue(training),
                    Naam = training.GetType().GetProperty("Naam")?.GetValue(training),
                    Beschrijving = training.GetType().GetProperty("Beschrijving")?.GetValue(training),
                    Categorie = training.GetType().GetProperty("Categorie")?.GetValue(training),
                    BedrijfNaam = training.GetType().GetProperty("BedrijfNaam")?.GetValue(training),
                    Status = training.GetType().GetProperty("Status")?.GetValue(training),
                    Duration = training.GetType().GetProperty("Duration")?.GetValue(training)
                }).ToList();

                return Ok(maskedTrainingen);
            }
            else
            {
                return NotFound("Geen trainingen gevonden met de opgegeven filters.");
            }
        }

        [HttpPost("trainingen-add")]
        public IActionResult InsertIntoDB([FromBody] TrainingAddModel model)
        {
            try
            {
                InsertResult result = Trainingen.Add(model.Status, model.Naam, model.LearningMethod, model.Duration, model.Beschrijving, model.Categorie, model.BedrijfID);
                if (result.Succeeded)
                {
                    return Ok(model);
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
        [HttpPost("training-update")]
        public IActionResult UpdateIntoDB([FromBody] TrainingUpdateModel model)
        {
            try
            {
                UpdateResult result = Trainingen.UpdateTrainingen(model.TrainingID, model.Status, model.Naam, model.LearningMethod, model.Duration, model.Beschrijving, model.Categorie);
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
                return StatusCode(500, $"Fout bij het updaten {ex.Message}");
            }
        }
        [HttpPost("training-delete")]
        public IActionResult DeleteFromDB([FromBody] TrainingDeleteModel model)
        {
            try
            {
                UpdateResult result = Trainingen.DeleteTrainingen(model.TrainingID);

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
        [HttpDelete("bulk-delete-trainingen")]
        public IActionResult BulkDeleteTrainingen([FromQuery] List<int> TrainingIDs)
        {
            if (TrainingIDs == null || !TrainingIDs.Any())
                return BadRequest("Geen geldige TrainingIDs opgegeven.");

            var result = Trainingen.DeleteBulkTrainingen(TrainingIDs);
            if (result.Succeeded)
                return Ok("Trainingen succesvol verwijderd");
            return BadRequest(result.AddError);
        }
    }
}