using ClassLibTeam12.Business.Entities;
using ClassLibTeam12.Data.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;



namespace ClassLibTeam12.Data
{
    /// <summary>
    ///   Deze interne klasse biedt methoden voor het beheren van gebruikersgegevens in de database.
    ///   Alle SQL-commando's worden uitgevoerd via de <see cref="SqlServer"/> klasse.
    /// </summary>
    internal class GebruikerData : SqlServer
    {

        public Gebruiker GetById(int gebruikerId)
        {
            string query = "SELECT * FROM Gebruikers WHERE GebruikerID = @GebruikerID";
            var parameters = new Dictionary<string, object>
    {
        { "@GebruikerID", gebruikerId }
    };

            var result = ExecuteSelect(query, parameters); // Bu SqlServer sınıfından geliyor

            if (!result.Succeeded || result.DataTable.Rows.Count == 0)
                return null;

            DataRow row = result.DataTable.Rows[0];

            return new Gebruiker
            {
                GebruikerID = gebruikerId,
                Voornaam = row["Voornaam"].ToString(),
                Achternaam = row["Achternaam"].ToString(),
                Email = row["Email"].ToString(),
                Wachtwoord = row["Wachtwoord"].ToString(),
                Telefoon = row["Telefoon"].ToString(),
                RolID = Convert.ToInt32(row["RolID"]),
                Locatie = row["Locatie"].ToString(),
                ProfileImagePath = row["ProfileImagePath"].ToString()
            };
        }


        /// <summary>
        ///   Initialiseert een nieuw exemplaar van de <see cref="GebruikerData"/> klasse.
        /// </summary>
        public GebruikerData()
        {
            TableName = "Gebruikers";
        }

        /// <summary>
        ///   Eigenschap die de naam van de tabel in de database specificeert.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        ///   Selecteert alle records uit de tabel "Gebruikers".
        ///   Dit maakt gebruik van de generieke <see cref="SqlServer.Select(string)"/> methode.
        /// </summary>
        /// <returns>
        ///   Een <see cref="SelectResult"/> object dat alle gegevens bevat die zijn opgehaald uit de tabel.
        /// </returns>
        public SelectResult Select()
        {
            return base.Select(TableName);
        }

        /// <summary>
        ///   Haalt een gebruiker op basis van het e-mailadres.
        /// </summary>
        /// <param name="email">Het e-mailadres van de gebruiker.</param>
        /// <returns>
        ///   Een <see cref="Gebruiker"/> object indien gevonden, anders null.
        /// </returns>
        public Gebruiker GetByEmail(string email)
        {
            var query = "SELECT Voornaam, Achternaam, Email, Wachtwoord FROM Gebruikers WHERE Email = @Email";
            // Parameters voorbereiden
            var parameters = new Dictionary<string, object> { { "@Email", email } };

            // SQL-commando uitvoeren via de basisklasse SqlServer
            SelectResult result = ExecuteSelect(query, parameters);

            if (result.Succeeded && result.DataTable.Rows.Count > 0)
            {
                DataRow row = result.DataTable.Rows[0];
                return new Gebruiker
                {
                    Voornaam = row["Voornaam"].ToString(),
                    Achternaam = row["Achternaam"].ToString(),
                    Email = row["Email"].ToString(),
                    Wachtwoord = row["Wachtwoord"].ToString()
                };
            }
            return null;
        }

        /// <summary>
        ///  Reset het wachtwoord van de gebruiker
        /// </summary>
        /// <param name="email">Het e-mailadres van de gebruiker.</param>
        /// <param name="newPassword">Het nieuwe wachtwoord van de gebruiker.</param>
        /// <returns>
        ///   Een <see cref="ResetResult"/> object dat aangeeft of het wachtwoord succesvol is gewijzigd.
        /// </returns>
        public ResetResult ResetPassword(string email, string newPassword)
        {
            ResetResult result = new ResetResult(); // ✅ Gebruik ResetResult in plaats van UpdateResult
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();

                    // Wachtwoord hashen (optioneel maar aanbevolen!)
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                    string sql = "UPDATE Gebruikers SET Wachtwoord = @NewPassword WHERE Email = @Email";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@NewPassword", hashedPassword);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            result.Succeeded = true;
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Errors = new List<string> { "Gebruiker niet gevonden." };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.Succeeded = false;
                result.Errors = new List<string> { ex.Message };
            }

            return result;
        }



        /// <summary>
        ///   Voegt een nieuwe gebruiker toe aan de database.
        /// </summary>
        /// <param name="gebruiker">Het <see cref="Gebruiker"/> object dat moet worden toegevoegd.</param>
        /// <returns>
        ///   Een <see cref="InsertResult"/> object dat aangeeft of de operatie succesvol was.
        /// </returns>
        public InsertResult Insert(Gebruiker gebruiker)
        {
            var result = new InsertResult();
            try
            {
                // Stel de SQL-query samen met ProfielfotoPad
                string insertQuery = $"INSERT INTO {TableName} (Voornaam, Achternaam, Email, Wachtwoord, Telefoon, RolID, Locatie, ProfileImagePath) " +
                                     "VALUES (@Voornaam, @Achternaam, @Email, @Wachtwoord, @Telefoon, @RolID, @Locatie, @ProfielfotoUrl);";

                // Parameters voorbereiden
                using (SqlCommand insertCommand = new SqlCommand(insertQuery))
                {
                    insertCommand.Parameters.Add("@Voornaam", SqlDbType.VarChar).Value = gebruiker.Voornaam;
                    insertCommand.Parameters.Add("@Achternaam", SqlDbType.VarChar).Value = gebruiker.Achternaam;
                    insertCommand.Parameters.Add("@Email", SqlDbType.VarChar).Value = gebruiker.Email;
                    insertCommand.Parameters.Add("@Wachtwoord", SqlDbType.VarChar).Value = gebruiker.Wachtwoord; // Vergeet niet het wachtwoord te hashen
                    insertCommand.Parameters.Add("@Telefoon", SqlDbType.VarChar).Value = gebruiker.Telefoon;
                    int roleId = gebruiker.Email.Contains("@pxl.com") ? 1 : 2; // 1 = Admin, 2 = Gebruiker
                    insertCommand.Parameters.Add("@RolID", SqlDbType.Int).Value = roleId;
                    insertCommand.Parameters.Add("@Locatie", SqlDbType.VarChar).Value = gebruiker.Locatie;
                    insertCommand.Parameters.Add("@ProfielfotoUrl", SqlDbType.VarChar).Value = gebruiker.ProfileImagePath; // Voeg de profielfoto toe

                    // SQL-commando uitvoeren via de basisklasse SqlServer
                    result = InsertRecord(insertCommand);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fout bij het toevoegen van de gebruiker: {ex.Message}");
            }
            return result;
        }

        /// <summary>
        ///   Controleert de inloggegevens van een gebruiker.
        /// </summary>
        /// <param name="email">Het e-mailadres van de gebruiker.</param>
        /// <param name="password">Het wachtwoord van de gebruiker.</param>
        /// <returns>
        ///   Een <see cref="SelectResult"/> object dat aangeeft of de inloggegevens correct zijn.
        /// </returns>
        public SelectResult Login(string email, string password)
        {
            string query = "SELECT * FROM Gebruikers WHERE Wachtwoord = @Wachtwoord and Email = @Email;";
            var parameters = new Dictionary<string, object>
            {
                { "@Wachtwoord", password },
                { "@Email", email }
            };
            return ExecuteSelect(query, parameters);
        }

        /// <summary>
        ///   Wijzigt het wachtwoord van een gebruiker.
        /// </summary>
        /// <param name="email">Het e-mailadres van de gebruiker.</param>
        /// <param name="oldPassword">Het oude wachtwoord van de gebruiker.</param>
        /// <param name="newPassword">Het nieuwe wachtwoord van de gebruiker.</param>
        /// <returns>
        ///   Een <see cref="UpdateResult"/> object dat aangeeft of het wachtwoord succesvol is gewijzigd.
        /// </returns>
        public UpdateResult UpdatePasswordGebruikerID(int gebruikerID, string oldPassword, string newPassword)
        {
            UpdateResult result = new UpdateResult();
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();
                    string sql = "UPDATE Gebruikers SET Wachtwoord = @NewPassword WHERE GebruikerID = @GebruikerID AND Wachtwoord = @OldPassword";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@GebruikerID", gebruikerID);
                        cmd.Parameters.AddWithValue("@OldPassword", oldPassword);
                        cmd.Parameters.AddWithValue("@NewPassword", newPassword);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            result.Succeeded = true;
                        }
                        else
                        {
                            result.Succeeded = false;
                            throw new Exception("Invalid email or password.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.Succeeded = false;
            }
            return result;
        }



        /// <summary>
        /// Selecteert de details van een gebruiker op basis van het e-mailadres.
        /// </summary>
        /// <param name="email">Het e-mailadres van de gebruiker.</param>
        /// <returns>Een <see cref="SelectResult"/> object met de gebruikersdetails.</returns>
        public SelectResult SelectUserDetails(int userID)
        {
            SelectResult result = new SelectResult();
            result.DataTable = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();
                    string sql = @"SELECT G.*, R.Naam AS RolNaam 
                           FROM Gebruikers G
                           LEFT JOIN Rollen R ON G.RolID = R.RolID
                           WHERE G.GebruikerID = @GebruikerID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@GebruikerID", userID);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(result.DataTable);
                        }
                    }

                    if (result.DataTable.Rows.Count > 0)
                    {
                        result.Succeeded = true;
                    }
                    else
                    {
                        result.Succeeded = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.Succeeded = false;
            }

            return result;
        }

        /// <summary>
        /// Selecteert alle gebruikers.
        /// </summary>
        /// <returns>Een <see cref="SelectResult"/> object met de lijst van alle gebruikers.</returns>
        public SelectResult SelectAllUsers(bool isAdmin)
        {
            SelectResult result = new SelectResult();
            result.DataTable = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();
                    string sql;

                    // Verschillende query voor admins en niet-admins
                    if (isAdmin)
                    {
                        sql = @"SELECT G.GebruikerID, G.Voornaam, G.Achternaam, G.Email, G.Telefoon, 
                       G.Wachtwoord, R.Naam AS RolNaam 
                       FROM Gebruikers G
                       LEFT JOIN Rollen R ON G.RolID = R.RolID"; // Inclusief wachtwoord
                    }
                    else
                    {
                        sql = @"SELECT G.GebruikerID, G.Voornaam, G.Achternaam, R.Naam AS RolNaam FROM Gebruikers G LEFT JOIN Rollen R ON G.RolID = R.RolID";
                        // Zonder wachtwoord
                    }

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(result.DataTable);
                        }
                    }
                    if (result.DataTable.Rows.Count > 0)
                    {
                        result.Succeeded = true;
                    }
                    else
                    {
                        result.Succeeded = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij ophalen van gebruikers: {ex.Message}");
                result.Succeeded = false;
            }

            return result;
        }

        /// <summary>
        /// Wijzigt de gegevens van een gebruiker.
        /// </summary>
        /// <param name="password">Het wachtwoord van de gebruiker.</param>
        /// <param name="firstName">De voornaam van de gebruiker.</param>
        /// <param name="lastName">De achternaam van de gebruiker.</param>
        /// <param name="email">Het e-mailadres van de gebruiker.</param>
        /// <param name="phoneNumber">Het telefoonnummer van de gebruiker.</param>
        /// <returns>Een <see cref="UpdateResult"/> object dat aangeeft of de update succesvol is uitgevoerd.</returns>
        public UpdateResult UpdateUserDetails(
     int userId,
     string voornaam,
     string achternaam,
     string email,
     string telefoon,
     string locatie
 )
        {
            UpdateResult result = new UpdateResult();
            string oldProfileImagePath = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();

                    // 1. Haal oude profielfoto op
                    string selectSql = "SELECT ProfileImagePath FROM Gebruikers WHERE GebruikerID = @GebruikerID";
                    using (SqlCommand selectCmd = new SqlCommand(selectSql, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@GebruikerID", userId);
                        object selectResult = selectCmd.ExecuteScalar();
                        oldProfileImagePath = (selectResult != DBNull.Value && selectResult != null)
                            ? selectResult.ToString()
                            : null;
                    }

                    // 2. Dynamische update-query
                    string sql = @"UPDATE Gebruikers 
                           SET Voornaam = @Voornaam, 
                               Achternaam = @Achternaam, 
                               Email = @Email, 
                               Telefoon = @Telefoon, 
                               Locatie = @Locatie

                  WHERE GebruikerID = @GebruikerID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@GebruikerID", userId);
                        cmd.Parameters.AddWithValue("@Voornaam", voornaam);
                        cmd.Parameters.AddWithValue("@Achternaam", achternaam);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Telefoon", telefoon);
                        cmd.Parameters.AddWithValue("@Locatie", locatie);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        result.Succeeded = rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
            }
            return result;
        }


        public UpdateResult AdminUpdateUserDetails(int id, string voornaam, string achternaam, string wachtwoord, string telefoon, string locatie, string profielfotoPad)
        {
            UpdateResult result = new UpdateResult();

            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();

                    string sql = @"
                UPDATE Gebruikers 
                SET 
                    Voornaam = @Voornaam, 
                    Achternaam = @Achternaam, 
                    Wachtwoord = @Wachtwoord, 
                    Telefoon = @Telefoon, 
                    Locatie = @Locatie, 
                    ProfileImagePath = @ProfielfotoPad
                WHERE GebruikerID = @GebruikerID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        // Voeg parameters toe aan de SQL-query
                        cmd.Parameters.AddWithValue("@Voornaam", voornaam ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Achternaam", achternaam ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Wachtwoord", wachtwoord ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Telefoon", telefoon ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Locatie", locatie ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProfielfotoPad", profielfotoPad ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@GebruikerID", id);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        result.Succeeded = rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.Succeeded = false;
            }

            return result;
        }


        public SelectResult GetFilteredUsers(int? gebruikerId, string voornaam, string achternaam, string rol)
        {
            StringBuilder query = new StringBuilder(@"SELECT G.GebruikerID, G.Voornaam, G.Achternaam, R.Naam as Rolnaam
                                              FROM Gebruikers G 
                                              LEFT JOIN Rollen R ON G.RolID = R.RolID 
                                              WHERE 1=1");  // Initial WHERE clause to make the first condition simple

            List<string> conditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            // ID-filter (exacte match)
            if (gebruikerId.HasValue)
            {
                conditions.Add("G.GebruikerID LIKE @GebruikerID");
                parameters.Add("@GebruikerID", $"{gebruikerId.Value}%");

                //conditions.Add("G.GebruikerID = @GebruikerID");
                //parameters.Add("@GebruikerID", gebruikerId.Value);

            }

            if (!string.IsNullOrWhiteSpace(voornaam))
            {
                conditions.Add("G.Voornaam LIKE @voornaam");
                parameters.Add("@voornaam", $"{voornaam}%");
            }

            if (!string.IsNullOrWhiteSpace(achternaam))
            {
                conditions.Add("G.Achternaam LIKE @achternaam");
                parameters.Add("@achternaam", $"{achternaam}%");
            }

            if (!string.IsNullOrWhiteSpace(rol))
            {
                conditions.Add("R.Naam LIKE @Rol");
                parameters.Add("@Rol", $"%{rol}%");
            }

            // If there are any conditions, append them to the query
            if (conditions.Any())
            {
                query.Append(" AND " + string.Join(" AND ", conditions));
            }

            return ExecuteSelect(query.ToString(), parameters);
        }

        public bool UpdateProfilePicture(int userId, string fileUrl)
        {
            bool updateSuccess = false;

            string query = "UPDATE Gebruikers SET ProfileImagePath = @FileUrl WHERE GebruikerID = @UserId";

            using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@FileUrl", fileUrl);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        updateSuccess = rowsAffected > 0; // Als er minstens 1 rij is bijgewerkt, was de update succesvol.
                    }
                }
                catch (Exception ex)
                {
                    // Log of verwerk de uitzondering indien nodig
                    Console.WriteLine(ex.Message);
                    updateSuccess = false;
                }
            }

            return updateSuccess;
        }
        public ResetResult UpdatePassword(string email, string newPassword)
        {
            var query = "UPDATE Gebruikers SET Wachtwoord = @Wachtwoord WHERE Email = @Email";
            // Parameters voorbereiden
            var parameters = new Dictionary<string, object>
            {
                { "@Wachtwoord", newPassword },
                { "@Email", email }
            };

            try
            {
                // SqlCommand aanmaken en parameters toevoegen
                using (SqlCommand updateCommand = new SqlCommand(query))
                {
                    foreach (var param in parameters)
                    {
                        updateCommand.Parameters.AddWithValue(param.Key, param.Value);
                    }
                    // SQL-commando uitvoeren via de basisklasse SqlServer
                    return UpdateRecord(updateCommand);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ResetResult { Succeeded = false, Errors = new List<string> { ex.Message } };
            }
        }
    }
}
