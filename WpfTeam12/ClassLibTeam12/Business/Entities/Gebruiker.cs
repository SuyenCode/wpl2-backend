namespace ClassLibTeam12.Business.Entities
{
    public class Gebruiker
    {
        public int GebruikerID { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public string Email { get; set; }
        public string Wachtwoord { get; set; }
        public string Telefoon { get; set; }
        public int RolID { get; set; }
        public string Locatie { get; set; }
        public string ProfileImagePath { get; set; }

    }
}
