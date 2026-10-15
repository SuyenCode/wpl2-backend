using ClassLibTeam12.Business;
using ClassLibTeam12.Data.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiTeam12.ViewModel;

namespace WebApiTeam12.Controllers
{
    /// <summary>
    /// Deze controller beheert de acties met betrekking tot vacatures (Jobs).
    /// Alleen gebruikers met de rol 'Admin' kunnen vacatures verwijderen.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        [HttpGet("all-jobs")]
        public IActionResult GetAllJobs()
        {
            try
            {
                var result = Jobs.GetAllJobs();
                if (result.Succeeded)
                {
                    var allJobs = Jobs.ProcessAllJobs(result.DataTable);
                    return Ok(allJobs);
                }
                return BadRequest(result.AddError);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Interne serverfout: {ex.Message}");
            }
        }

        [HttpGet("filter")]
        public IActionResult GetFilteredJobs(
            [FromQuery] int? jobID,
            [FromQuery] string? naam,
            [FromQuery] string? beschrijving,
            [FromQuery] int? positie,
            [FromQuery] string? bedrijfNaam)
        {
            jobID = jobID.HasValue ? jobID : null;
            naam = string.IsNullOrWhiteSpace(naam) ? null : naam;
            beschrijving = string.IsNullOrWhiteSpace(beschrijving) ? null : beschrijving;
            positie = positie.HasValue ? positie : null;
            bedrijfNaam = string.IsNullOrWhiteSpace(bedrijfNaam) ? null : bedrijfNaam;

            var result = Jobs.GetFilteredJobs(jobID, naam, beschrijving, positie, bedrijfNaam);
            if (result.Succeeded && result.DataTable != null)
            {
                var filteredJobs = Jobs.ProcessAllJobs(result.DataTable);
                return Ok(filteredJobs);
            }
            return NotFound("Geen vacatures gevonden met de opgegeven filters.");
        }

        [HttpPost("add-jobs")]
        public IActionResult InsertJob([FromBody] JobModel newModel)
        {
            try
            {
                var result = Jobs.Add(newModel.Naam, newModel.Beschrijving, newModel.Positie, newModel.BedrijfID);
                if (result.Succeeded)
                    return Ok(newModel);
                return BadRequest(result.AddError);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fout bij het invoegen: {ex.Message}");
            }
        }

        [HttpPost("update-jobs")]
        public IActionResult UpdateJob([FromBody] JobUpdateModel updateModel)
        {
            try
            {
                var result = Jobs.UpdateJobs(
                    updateModel.JobID,
                    updateModel.Naam,
                    updateModel.Beschrijving,
                    updateModel.Positie,
                    updateModel.BedrijfID);
                if (result.Succeeded)
                    return Ok(updateModel);
                return BadRequest(result.AddError);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fout bij het bijwerken: {ex.Message}");
            }
        }

        // Alleen Admin mag vacatures verwijderen
        [Authorize(Roles = "Admin")]
        [HttpPost("delete-job")]
        public IActionResult DeleteJob([FromBody] JobDeleteModel deleteModel)
        {
            try
            {
                var result = Jobs.DeleteJob(deleteModel.JobId);
                if (result.Succeeded)
                    return Ok(deleteModel);
                return BadRequest(result.AddError);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fout bij het deleten: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("bulk-delete-jobs")]
        public IActionResult BulkDeleteJobs([FromQuery] List<int> JobIDs)
        {
            if (JobIDs == null || !JobIDs.Any())
                return BadRequest("Geen geldige JobIDs opgegeven.");

            var result = Jobs.DeleteJobs(JobIDs);
            if (result.Succeeded)
                return Ok("Jobs succesvol verwijderd");
            return BadRequest(result.AddError);
        }
    }
}
