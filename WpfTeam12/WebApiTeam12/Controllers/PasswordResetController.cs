using ClassLibTeam12.Business;
using ClassLibTeam12.Business.Entities;
using Microsoft.AspNetCore.Mvc;
using WebApiTeam12.ViewModel;




namespace WebApiTeam12.Controllers
{
    /// <summary>
    /// Deze controller beheert het resetten van wachtwoorden.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordResetController : ControllerBase
    {
        /// <summary>
        /// De EmailService om e-mails te versturen.
        /// </summary>
        private readonly EmailService _emailService;

        /// <summary>
        /// Constructor voor de PasswordResetController.
        /// </summary>
        /// <param name="emailService">De EmailService om te injecteren.</param>
        public PasswordResetController(EmailService emailService)
        {
            _emailService = emailService;
        }

        /// <summary>
        /// Verwerkt het verzoek om een wachtwoord te resetten.
        /// </summary>
        /// <param name="request">Het PasswordResetRequest object met het e-mailadres van de gebruiker.</param>
        /// <returns>Een IActionResult met een succes- of foutmelding.</returns>
        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetRequest request)
        {
            Gebruiker user = null;

            try
            {
                // Haal de gebruiker op basis van het e-mailadres.
                user = Gebruikers.GetByEmail(request.Email);

                // Controleer of de gebruiker bestaat.
                if (user == null)
                {
                    return NotFound(new { message = "Dit e-mailadres is niet geregistreerd in het systeem!" });
                }

                // Genereer een nieuw willekeurig wachtwoord.
                string newPassword = GenerateRandomPassword();

                // Update het wachtwoord in de database.
                var updateResult = Gebruikers.UpdatePassword(request.Email, newPassword);

                // Controleer of de update succesvol was.
                if (updateResult == null || !updateResult.Succeeded)
                {
                    return StatusCode(500, new { message = "Wachtwoord kon niet worden bijgewerkt." });
                }

                // Het verzenden van e-mail is dynamisch gemaakt.
                string subject = "Your password is updated!";

                string htmlPart = $"<h3>Dear {user.Voornaam} {user.Achternaam}, <br/> Your password is updated as {newPassword}";

                // Verstuur het nieuwe wachtwoord naar de gebruiker.
                bool emailSent = await _emailService.SendEmailAsync(user.Email, subject, htmlPart);

                // Controleer of het verzenden van de e-mail succesvol was.
                if (emailSent)
                {
                    return Ok(new { message = "Je nieuwe wachtwoord is naar je e-mailadres gestuurd." });
                }
                else
                {
                    return StatusCode(500, new { message = "Fout bij verzenden wachtwoord." });
                }
            }
            catch (Exception ex)
            {
                // Log de fout en retourneer een server error.
                Console.WriteLine($"[ERROR] {DateTime.UtcNow}: {ex.Message} - {ex.StackTrace}");
                return StatusCode(500, new { message = "Er is een fout opgetreden. Controleer de server logs voor details." });
            }
        }

        /// <summary>
        /// Genereert een willekeurig wachtwoord van 8 tekens lang.
        /// </summary>
        /// <returns>Een willekeurig wachtwoord.</returns>
        private string GenerateRandomPassword()
        {
            // Definieer de karakters die gebruikt worden voor het genereren van het wachtwoord.
            string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            string digits = "0123456789";
            string specialChars = "!@#$%^&*()";
            string allChars = upperCase + lowerCase + digits + specialChars;

            // Maak een random number generator.
            var random = new Random();

            // Maak een lijst van karakters voor het wachtwoord.
            List<char> password = new List<char>
            {
                upperCase[random.Next(upperCase.Length)],
                lowerCase[random.Next(lowerCase.Length)],
                digits[random.Next(digits.Length)],
                specialChars[random.Next(specialChars.Length)]
            };

            // Vul de rest van het wachtwoord met willekeurige karakters.
            for (int i = 0; i < 4; i++)
            {
                password.Add(allChars[random.Next(allChars.Length)]);
            }

            // Schud de karakters door elkaar.
            password = password.OrderBy(x => random.Next()).ToList();

            // Converteer de lijst van karakters naar een string en retourneer deze.
            return new string(password.ToArray());
        }
    }
}