using ClassLibTeam12.Business;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace WebApiTeam12.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginStatistiekController : ControllerBase
    {
        [HttpGet("daily-login-counts")]
        public IActionResult GetDailyLoginCounts()
        {
            try
            {
                var result = TijdslogHelper.SelectDailyLoginCounts();

                if (result.Succeeded && result.DataTable != null)
                {
                    var loginCounts = new List<object>();

                    foreach (DataRow row in result.DataTable.Rows)
                    {
                        loginCounts.Add(new
                        {
                            Datum = Convert.ToDateTime(row["Datum"]).ToString("yyyy-MM-dd"),
                            Aantal = Convert.ToInt32(row["AantalLogins"])
                        });
                    }

                    return Ok(loginCounts);
                }

                return StatusCode(500, "Fout bij ophalen van loginstatistieken.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
