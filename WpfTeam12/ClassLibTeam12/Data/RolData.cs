using ClassLibTeam12.Business.Entities;
using ClassLibTeam12.Data.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ClassLibTeam12.Data
{
    /// <summary>
    ///   Deze interne klasse biedt methoden voor het beheren van rolgegevens in de database.
    ///   Alle SQL-commando's worden uitgevoerd via de <see cref="SqlServer"/> klasse.
    /// </summary>
    internal class RolData : SqlServer
    {
        /// <summary>
        ///   Initialiseert een nieuw exemplaar van de <see cref="RolData"/> klasse.
        ///   Stelt de naam van de tabel in op "Rollen".
        /// </summary>
        public RolData()
        {
            TableName = "Rollen";
        }

        /// <summary>
        ///   Eigenschap die de naam van de tabel specificeert.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        ///   Selecteert alle records uit de tabel "Rollen".
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
        ///   Voegt een nieuwe rol toe aan de tabel "Rollen" in de database.
        ///   Dit maakt gebruik van de generieke <see cref="SqlServer.InsertRecord(SqlCommand)"/> methode.
        /// </summary>
        /// <param name="rol">
        ///   Een <see cref="Rol"/> object dat de gegevens van de rol bevat, zoals naam.
        /// </param>
        /// <returns>
        ///   Een <see cref="InsertResult"/> object dat aangeeft of het toevoegen succesvol was en het ID van het nieuwe record bevat.
        /// </returns>
        public InsertResult Insert(Rol rol)
        {
            var result = new InsertResult();
            try
            {
                // Stel het SQL-invoegcommando samen
                string insertQuery = $"INSERT INTO {TableName} (Naam) VALUES (@Naam);";

                // Maak een SqlCommand aan en voeg parameters toe
                using (SqlCommand insertCommand = new SqlCommand(insertQuery))
                {
                    insertCommand.Parameters.Add("@Naam", SqlDbType.VarChar).Value = rol.Naam;

                    // Roep de InsertRecord-methode aan om het commando uit te voeren
                    result = InsertRecord(insertCommand);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"SQL Fout bij het toevoegen van een nieuwe rol: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Algemene fout bij het toevoegen van een nieuwe rol: {ex.Message}", ex);
            }
            return result;
        }

        /// <summary>
        ///   Haalt een specifieke rol op basis van het ID.
        ///   Dit maakt gebruik van de generieke <see cref="SqlServer.ExecuteSelect(string, Dictionary{string, object})"/> methode.
        /// </summary>
        /// <param name="rolId">Het ID van de rol.</param>
        /// <returns>
        ///   Een <see cref="SelectResult"/> object dat gegevens bevat over de geselecteerde rol.
        /// </returns>
        public SelectResult GetById(int rolId)
        {
            try
            {
                string query = "SELECT * FROM Rollen WHERE RolID = @RolID";
                var parameters = new Dictionary<string, object> { { "@RolID", rolId } };

                // Roep ExecuteSelect aan om het commando uit te voeren
                return ExecuteSelect(query, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Fout bij het ophalen van een rol met ID {rolId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        ///   Wijzigt een bestaande rol in de tabel "Rollen".
        ///   Dit maakt gebruik van de generieke <see cref="SqlServer.UpdateRecord(SqlCommand)"/> methode.
        /// </summary>
        /// <param name="rol">
        ///   Een <see cref="Rol"/> object dat de bijgewerkte gegevens van de rol bevat, zoals naam en ID.
        /// </param>
        /// <returns>
        ///   Een <see cref="ResetResult"/> object dat aangeeft of de update succesvol was.
        /// </returns>
        public ResetResult Update(Rol rol)
        {
            try
            {
                string updateQuery = "UPDATE Rollen SET Naam = @Naam WHERE RolID = @RolID";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery))
                {
                    updateCommand.Parameters.Add("@Naam", SqlDbType.VarChar).Value = rol.Naam;
                    updateCommand.Parameters.Add("@RolID", SqlDbType.Int).Value = rol.RolID;

                    // Roep UpdateRecord aan om het commando uit te voeren
                    return UpdateRecord(updateCommand);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fout bij het bijwerken van een rol met ID {rol.RolID}: {ex.Message}", ex);
            }
        }
    }
}
