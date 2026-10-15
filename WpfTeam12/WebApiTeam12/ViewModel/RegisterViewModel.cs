using Microsoft.AspNetCore.Identity;

namespace WebApiTeam12.ViewModel
{
    public class RegisterViewModel
    {
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public string Telefoonnummer { get; set; }
        public string Email { get; set; }
        public string Wachtwoord { get; set; }
        public string Locatie { get; set; }
    }
}

