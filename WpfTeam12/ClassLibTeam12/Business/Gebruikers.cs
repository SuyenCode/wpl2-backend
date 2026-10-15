using ClassLibTeam12.Business.Entities;
using ClassLibTeam12.Business.Validation;
using ClassLibTeam12.Data;
using ClassLibTeam12.Data.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.Remoting.Contexts;

namespace ClassLibTeam12.Business
{
    /// <summary>
    ///   Deze static class biedt methoden voor het beheren van gebruikers.
    /// </summary>
    public static class Gebruikers
    {

        public static Gebruiker GetById(int gebruikerId)
        {
            GebruikerData gebruikerData = new GebruikerData();
            return gebruikerData.GetById(gebruikerId);
        }

        /// <summary>
        ///   Haalt gefilterde gebruikers op basis van voornaam, achternaam en email.
        /// </summary>
        /// <param name="voornaam">De voornaam van de gebruiker om op te filteren.</param>
        /// <param name="achternaam">De achternaam van de gebruiker om op te filteren.</param>
        /// <param name="email">Het emailadres van de gebruiker om op te filteren.</param>
        /// <returns>Een SelectResult-object met de gefilterde gebruikers.</returns>
        public static SelectResult GetFilteredUsers(int? gebruikerid, string voornaam, string achternaam, string rolNaam)
        {
            GebruikerData gebruikerData = new GebruikerData();
            return gebruikerData.GetFilteredUsers(gebruikerid, voornaam, achternaam, rolNaam);
        }
        /// <summary>
        ///   Haalt alle gebruikers op.
        /// </summary>
        /// <returns>Een SelectResult-object met alle gebruikers.</returns>
        public static SelectResult GetGebruikers()
        {
            GebruikerData gebruikerData = new GebruikerData();
            SelectResult result = gebruikerData.Select();
            return result;
        }

        /// <summary>
        ///   Haalt een gebruiker op basis van emailadres.
        /// </summary>
        /// <param name="email">Het emailadres van de gebruiker.</param>
        /// <returns>Een Gebruiker-object met de gegevens van de gevonden gebruiker.</returns>
        public static Gebruiker GetByEmail(string email)
        {
            GebruikerData gebruikerData = new GebruikerData();
            return gebruikerData.GetByEmail(email);

        }
        /// <summary>
        ///   Wijzigt het wachtwoord van een gebruiker.
        /// </summary>
        /// <param name="email">Het emailadres van de gebruiker.</param>
        /// <param name="newPassword">Het nieuwe wachtwoord.</param>
        /// <returns>Een ResetResult-object dat aangeeft of de operatie geslaagd is.</returns>
        public static ResetResult UpdatePassword(string email, string newPassword)
        {
            GebruikerData gebruikerData = new GebruikerData();
            return gebruikerData.UpdatePassword(email, newPassword);
        }




        /// <summary>
        ///   Voegt een nieuwe gebruiker toe aan het systeem.
        /// </summary>
        /// <param name="voornaam">De voornaam van de nieuwe gebruiker.</param>
        /// <param name="achternaam">De achternaam van de nieuwe gebruiker.</param>
        /// <param name="email">Het emailadres van de nieuwe gebruiker.</param>
        /// <param name="wachtwoord">Het wachtwoord van de nieuwe gebruiker.</param>
        /// <param name="telefoon">Het telefoonnummer van de nieuwe gebruiker.</param>
        /// <returns>Een InsertResult-object dat aangeeft of de operatie geslaagd is.</returns>
        public static InsertResult Add(string voornaam, string achternaam, string email, string wachtwoord, string telefoon, string locatie, string profileImagePath)
        {
            Gebruiker gebruiker = new Gebruiker
            {
                Voornaam = voornaam,
                Achternaam = achternaam,
                Email = email,
                Wachtwoord = wachtwoord,
                Telefoon = telefoon,
                RolID = email.Contains("@pxl.com") ? 1 : 2, // Admin of Gebruiker
                Locatie = locatie,
                ProfileImagePath = profileImagePath // Zorg dat dit correct wordt toegewezen
            };

            GebruikerData gebruikerData = new GebruikerData();
            InsertResult result = gebruikerData.Insert(gebruiker);
            return result;
        }

        /// Controleert of een combinatie van email en wachtwoord geldig is.
        ///
        public static SelectResult LoginCheck(string email, string password)
        {
            GebruikerData gebruikerData = new GebruikerData();
            SelectResult resutlt = gebruikerData.Login(email, password);
            return resutlt;
        }
        /// Wijzigt het wachtwoord en andere gegevens van een bestaande gebruiker.
        ///
        public static UpdateResult ChangePassword(int gebruikerId, string oldPassword, string newPassword)
        {
            GebruikerData gebruikerData = new GebruikerData();
            UpdateResult result = gebruikerData.UpdatePasswordGebruikerID(gebruikerId, oldPassword, newPassword);
            return result;
        }


        /// Haalt de details van een gebruiker op basis van het e-mailadres.
        /// 
        public static SelectResult GetUserDetails(int userID)
        {
            GebruikerData gebruikerData = new GebruikerData();
            SelectResult result = gebruikerData.SelectUserDetails(userID);
            return result;
        }
        ///  Haalt een lijst van alle gebruikers op met hun rolnaam
        /// 
        public static SelectResult GetAllUsers(bool isAdmin)
        {
            GebruikerData gebruikerData = new GebruikerData();
            return gebruikerData.SelectAllUsers(isAdmin);
        }
        /// Verwerkt de data van alle gebruikers.
        ///

        public static List<object> ProcessAllUsers(DataTable dataTable, bool isAdmin = false)
        {
            var allUsers = new List<object>();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    if (isAdmin)
                    {
                        // Admin ziet alle details, inclusief wachtwoord
                        allUsers.Add(new
                        {
                            GebruikerId = Convert.ToInt32(row["GebruikerID"]),
                            Voornaam = row["Voornaam"].ToString(),
                            Achternaam = row["Achternaam"].ToString(),
                            Email = row["Email"].ToString(),
                            Telefoon = row["Telefoon"].ToString(),
                            Wachtwoord = row["Wachtwoord"].ToString(),
                            RolNaam = row["RolNaam"].ToString()
                        });
                    }
                    else
                    {
                        // Niet-admin ziet gemaskeerde details
                        allUsers.Add(new
                        {
                            GebruikerId = Convert.ToInt32(row["GebruikerID"]),
                            Voornaam = row["Voornaam"].ToString(),
                            Achternaam = row["Achternaam"].ToString(),
                            RolNaam = row["RolNaam"].ToString()
                        });
                    }
                }
            }

            return allUsers;
        }


        /// Wijzigt het wachtwoord en andere gegevens van een bestaande gebruiker
        ///
        public static UpdateResult ChangeUserDetails(
    int userID,
    string voornaam,
    string achternaam,
    string email,
    string telefoon,
    string locatie
)
        {
            GebruikerData gebruikerData = new GebruikerData();
            UpdateResult result = gebruikerData.UpdateUserDetails(
                userID,
                voornaam,
                achternaam,
                email,
                telefoon,
                locatie
            );
            return result;
        }

        public static UpdateResult UpdateUserDetailsAdmin(int id, string voornaam, string achternaam, string wachtwoord, string telefoon, string locatie, string profielfotoPad)
        {
            GebruikerData gebruikerData = new GebruikerData();
            UpdateResult result = gebruikerData.AdminUpdateUserDetails(id, voornaam, achternaam, wachtwoord, telefoon, locatie, profielfotoPad);
            return result;
        }

        public static bool ProfilePicture(int userId, string fileUrl)
        {
            // Maak een instantie van de GebruikerData-klasse en voer de database-update uit.
            GebruikerData gebruikerData = new GebruikerData();
            bool result = gebruikerData.UpdateProfilePicture(userId, fileUrl);

            return result;
        }

    }
}
