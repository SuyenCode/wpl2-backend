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
using ClassLibTeam12.Business.Entities;
using Microsoft.AspNetCore.Mvc.Infrastructure;





namespace WebApiTeam12.Controllers
{
    /// <summary>
    /// Deze controller beheert gebruikersacties.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GebruikerController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public GebruikerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet("temp")]
        [Authorize]
        public IActionResult TempSecureEndpoint()
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"];
            string tokenString = authorizationHeader?.Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            // Extract the expiration time
            DateTime expiration = token.ValidTo;

            // Optioneel: Bereken de resterende tijd
            TimeSpan timeLeft = expiration - DateTime.UtcNow;

            // Optioneel: zet alle claims om in een leesbare lijst
            var claimsList = User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            });

            return Ok(new
            {
                ExpirationTime = expiration,
                TimeLeft = timeLeft,
                Message = "Je bent succesvol geauthenticeerd!",
                Claims = claimsList
            });
        }

        /// <summary>
        /// Haalt gefilterde gebruikers op basis van voornaam, achternaam en/of e-mail.
        /// </summary>
        /// <param name="voornaam">De voornaam om op te filteren (optioneel).</param>
        /// <param name="achternaam">De achternaam om op te filteren (optioneel).</param>
        /// <param name="email">Het e-mailadres om op te filteren (optioneel).</param>
        /// <returns>Een lijst van gefilterde gebruikers of een NotFound resultaat als er geen gebruikers gevonden worden.</returns>
        [HttpGet("filter")]
        [AllowAnonymous]
        public IActionResult GetFilteredUsers([FromQuery] int? gebruikerID, [FromQuery] string? voornaam, [FromQuery] string? achternaam, [FromQuery] string? rolNaam)
        {

            // Zet de zoekparameters naar null indien leeg
            gebruikerID = gebruikerID.HasValue ? gebruikerID : null;
            voornaam = string.IsNullOrWhiteSpace(voornaam) ? null : voornaam;
            achternaam = string.IsNullOrWhiteSpace(achternaam) ? null : achternaam;
            rolNaam = string.IsNullOrWhiteSpace(rolNaam) ? null : rolNaam;


            // Haal de gefilterde gebruikers op
            var result = Gebruikers.GetFilteredUsers(gebruikerID, voornaam, achternaam, rolNaam);

            if (result.Succeeded && result.DataTable != null)
            {
                var filteredUsers = Gebruikers.ProcessAllUsers(result.DataTable);


                var maskedUsers = filteredUsers.Select(user => new
                {
                    GebruikerId = user.GetType().GetProperty("GebruikerId")?.GetValue(user),
                    Voornaam = user.GetType().GetProperty("Voornaam")?.GetValue(user),
                    Achternaam = user.GetType().GetProperty("Achternaam")?.GetValue(user),
                    Email = ValidationRules.MaskEmail(user.GetType().GetProperty("Email")?.GetValue(user)?.ToString()),
                    Telefoon = user.GetType().GetProperty("Telefoon")?.GetValue(user)?.ToString(),
                    RolNaam = user.GetType().GetProperty("RolNaam")?.GetValue(user)?.ToString()
                }).ToList();

                return Ok(maskedUsers);

            }
            else
            {
                return NotFound("Geen gebruikers gevonden met de opgegeven filters.");
            }
        }

        /// <summary>
        /// Verifieert de login van een gebruiker.
        /// </summary>
        /// <param name="loginModel">Het login model met het e-mailadres en wachtwoord.</param>
        /// <returns>Het ID van de gebruiker indien de login succesvol is, anders een foutmelding.</returns>
        /// <summary>
        /// Login-endpoint: valideert de gebruiker en geeft een JWT-token terug.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginViewModel loginModel)
        {
            string email = loginModel.Email;
            string password = loginModel.Password;

            var result = Gebruikers.LoginCheck(email, password);
          


            if (result.DataTable.Rows.Count > 0)
            {
                var row = result.DataTable.Rows[0];

                int id = row["GebruikerID"] != DBNull.Value ? Convert.ToInt32(row["GebruikerID"]) : 0;
                int rolId = row["RolID"] != DBNull.Value ? Convert.ToInt32(row["RolID"]) : 0;
                string rolNaam = rolId == 1 ? "Admin" : "User";
                string emailString = row["Email"]?.ToString() ?? "";

                TijdslogHelper.InsertLoginTime(id);


                // Genereer JWT-token
                var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, rolNaam), // Voeg de dynamisch bepaalde RolNaam toe
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    userId = id,
                    role = rolNaam, // Geef de rolnaam terug in de response
                    email = emailString,
                    token = tokenString,
                    expiration = token.ValidTo
                });
            }
            else
            {
                return Unauthorized("Email or password incorrect");
            }
        }

        /// <summary>
        /// Wijzigt het wachtwoord van een gebruiker.
        /// </summary>
        /// <param name="model">Het model met het e-mailadres, oude wachtwoord en nieuwe wachtwoord.</param>
        /// <returns>Een succesmelding als het wachtwoord gewijzigd is, anders een foutmelding.</returns>
        [HttpPost("change-password")]
        [Authorize]
        public IActionResult ChangePassword([FromBody] ChangePasswordModel model)
        {
            // Valideer het model
            if (model == null || string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrEmpty(model.NewPassword))
            {
                return BadRequest("Invalid client request.");
            }

            // Haal de claims van de ingelogde gebruiker op
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Roep de Gebruikers business logic aan om het wachtwoord te wijzigen
            var result = Gebruikers.ChangePassword(int.Parse(userId), model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok("Password changed successfully.");
            }
            else
            {
                return BadRequest("Failed to change the password.");
            }
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUserWithProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = Gebruikers.GetUserDetails(int.Parse(userId));
            if (result.Succeeded && result.DataTable.Rows.Count > 0)
            {
                var user = result.DataTable.Rows[0];

                return Ok(new
                {
                    GebruikerId = user["GebruikerID"],
                    Voornaam = user["Voornaam"],
                    Achternaam = user["Achternaam"],
                    Email = user["Email"],
                    Telefoon = user["Telefoon"],
                    RolNaam = user["RolNaam"],
                    ProfielfotoUrl = user["ProfileImagePath"],
                    Locatie = user["Locatie"]
                });
            }

            return NotFound();
        }


        /// <summary>
        /// Haalt alle gebruikers op (gemaskeerd voor niet-admins).
        /// </summary>
        /// <returns>Een lijst van gemaskeerde gebruikers.</returns>
        [HttpGet("all-users")]
        [AllowAnonymous]
        public IActionResult GetAllUsers()
        {
            bool isAdmin = false;
            var result = Gebruikers.GetAllUsers(isAdmin);
            if (result.Succeeded && result.DataTable != null)
            {
                var allUsers = Gebruikers.ProcessAllUsers(result.DataTable, isAdmin);
                if (allUsers.Count > 0)
                {
                    // Gebruikers zien gemaskeerde gegevens
                    var maskedUsers = allUsers.Select(user => new
                    {
                        GebruikerId = user.GetType().GetProperty("GebruikerId")?.GetValue(user),
                        Voornaam = user.GetType().GetProperty("Voornaam")?.GetValue(user)?.ToString(),
                        Achternaam = user.GetType().GetProperty("Achternaam")?.GetValue(user)?.ToString(),
                        RolNaam = user.GetType().GetProperty("RolNaam")?.GetValue(user)?.ToString()
                    }).ToList();

                    return Ok(maskedUsers);
                }
                else
                {
                    return NotFound("Geen gebruikers gevonden");
                }
            }
            else
            {
                return StatusCode(500, "Er is een fout opgetreden bij het ophalen van de gebruikersgegevens");
            }
        }
        /// <summary>
        /// Haalt alle gebruikers op (inclusief wachtwoorden, alleen voor admins).
        /// </summary>
        /// <returns>Een lijst van alle gebruikers met volledige details.</returns>
        [HttpGet("all-users-admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllUsersAdmin()
        {
            bool isAdmin = true;
            var result = Gebruikers.GetAllUsers(isAdmin);

            if (result.Succeeded && result.DataTable != null)
            {
                var allUsers = Gebruikers.ProcessAllUsers(result.DataTable, isAdmin);
                return Ok(allUsers.Select(user => new
                {
                    GebruikerId = user.GetType().GetProperty("GebruikerId")?.GetValue(user),
                    Voornaam = user.GetType().GetProperty("Voornaam")?.GetValue(user),
                    Achternaam = user.GetType().GetProperty("Achternaam")?.GetValue(user),
                    Email = user.GetType().GetProperty("Email")?.GetValue(user),
                    Telefoon = user.GetType().GetProperty("Telefoon")?.GetValue(user),
                    Wachtwoord = user.GetType().GetProperty("Wachtwoord")?.GetValue(user),
                    RolNaam = user.GetType().GetProperty("RolNaam")?.GetValue(user)
                }).ToList());
            }
            return StatusCode(500, "Fout bij ophalen gebruikers");
        }



        /// <summary>
        /// Registreert een nieuwe gebruiker.
        /// </summary>
        /// <param name="model">Het model met de registratiegegevens.</param>
        /// <returns>Een succesmelding als de registratie succesvol is, anders een foutmelding.</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegisterViewModel model)
        {
            // Model validatie
            if (model == null ||
                string.IsNullOrWhiteSpace(model.Voornaam) ||
                string.IsNullOrWhiteSpace(model.Achternaam) ||
                string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Wachtwoord) ||
                string.IsNullOrWhiteSpace(model.Telefoonnummer) ||
                string.IsNullOrWhiteSpace(model.Locatie))
            {
                return BadRequest(new { message = "Alle velden zijn verplicht, inclusief Locatie." });
            }

            //Extra validaties
            if (!ValidationRules.IsValidEmail(model.Email))
                return BadRequest(new { message = "Invalid email format." });
            if (!ValidationRules.IsValidPhoneNumber(model.Telefoonnummer))
                return BadRequest(new { message = "Invalid phone number." });
            if (!ValidationRules.IsValidName(model.Voornaam) || !ValidationRules.IsValidName(model.Achternaam))
                return BadRequest(new { message = "Name can only contain letters." });
            if (model.Wachtwoord.Length < 6 || !ValidationRules.IsValidPassword(model.Wachtwoord))
                return BadRequest(new { message = "Password must be at least 6 characters long and contain both letters and numbers." });

            // Gebruik default profielfoto als gebruiker niets opgeeft
            string defaultProfileImage = "0d8256f6-33f9-4fbc-84d1-d7b387ee7d9d_Portrait_Placeholder.png";
            var profielfotoPad = $"{Request.Scheme}://{Request.Host}/uploads/profile_pictures/{defaultProfileImage}";


            // Gebruiker toevoegen aan database (inclusief profielfotoPad)
            InsertResult result = Gebruikers.Add(
                model.Voornaam,
                model.Achternaam,
                model.Email,
                model.Wachtwoord,
                model.Telefoonnummer,
                model.Locatie,
                profielfotoPad
            );

            string role = model.Email.EndsWith("@pxl.com", StringComparison.OrdinalIgnoreCase) ? "Admin" : "User";

            if (result.Succeeded)
            {
                var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, result.NewId.ToString()),
            new Claim(ClaimTypes.Email, model.Email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    message = "User registered and logged in successfully.",
                    token = tokenString,
                    expiration = token.ValidTo
                });
            }
            else
            {
                return StatusCode(500, new { message = "Failed to register user." });
            }

        }
        /// <summary>
        /// Werkt de details van een bestaande gebruiker bij.
        /// </summary>
        /// <param name="detailsModel">Het model met de bijgewerkte gebruikersdetails.</param>
        /// <returns>Een succesmelding als de update succesvol is, anders een foutmelding.</returns>
        [HttpPut("updateUser-details")]
        [Authorize]
        public IActionResult UpdateUserDetails([FromBody] UpdateUserDetailsViewModel detailsModel)
        {
            if (detailsModel == null ||
                string.IsNullOrWhiteSpace(detailsModel.Voornaam) ||
                string.IsNullOrWhiteSpace(detailsModel.Achternaam) ||
                string.IsNullOrWhiteSpace(detailsModel.Email) ||
                string.IsNullOrWhiteSpace(detailsModel.Telefoon) ||
                string.IsNullOrWhiteSpace(detailsModel.Locatie))
            {
                return BadRequest(new { message = "Alle velden zijn verplicht." });
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = Gebruikers.ChangeUserDetails(
                int.Parse(userId),
                detailsModel.Voornaam,
                detailsModel.Achternaam,
                detailsModel.Email,
                detailsModel.Telefoon,
                detailsModel.Locatie
            );

            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Bijwerken gebruiker mislukt." });
            }
            return Ok(new { message = "Gebruiker succesvol bijgewerkt." });
        }



        [HttpPost("update-user/{id}")]
        [Authorize(Roles = "Admin")] // Alleen Admins mogen andere gebruikers wijzigen
        public IActionResult UpdateUsersDetails(int id, [FromBody] UpdateUserDetailsAdminViewModel model)
        {
            // Controleer het model
            if (model == null || string.IsNullOrEmpty(model.Voornaam) || string.IsNullOrEmpty(model.Achternaam) || string.IsNullOrEmpty(model.Wachtwoord) || string.IsNullOrEmpty(model.Telefoon) || string.IsNullOrEmpty(model.Locatie))
            {
                return BadRequest("Invalid client request.");
            }

            // Voer de update uit
            var result = Gebruikers.UpdateUserDetailsAdmin(id, model.Voornaam, model.Achternaam, model.Wachtwoord, model.Telefoon, model.Locatie, model.ProfielfotoPad);

            return result.Succeeded ? Ok("User updated successfully.") : BadRequest("User update failed.");
        }

        [HttpPost("upload-profielfoto")]
        [Authorize]
        public IActionResult UploadProfilePicture([FromForm] ProfileViewModel model)
        {
            var file = model.File;
            if (file == null || file.Length == 0)
                return BadRequest("Geen bestand geüpload");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("Alleen JPG/PNG toegestaan");

            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                var uploadsFolder = Path.Combine("wwwroot", "uploads", "profile_pictures");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var photoUrl = $"/uploads/profile_pictures/{uniqueFileName}";
                var updateResult = Gebruikers.ProfilePicture(int.Parse(userId), photoUrl);

                return Ok(new { photoUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Interne fout: {ex.Message}");
            }
        }


    }
}
