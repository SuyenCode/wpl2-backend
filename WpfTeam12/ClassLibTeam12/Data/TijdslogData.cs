using ClassLibTeam12.Business.Entities;
using ClassLibTeam12.Data.Framework;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ClassLibTeam12.Data
{
    /// <summary>
    ///   Deze interne klasse biedt methoden voor het beheren van timeloggegevens in de database.
    ///   Alle SQL-commando's worden uitgevoerd via de <see cref="SqlServer"/> klasse.
    /// </summary>
    internal class TijdslogData : SqlServer
    {
        /// <summary>
        ///   Initialiseert een nieuw exemplaar van de <see cref="TijdslogData"/> klasse.
        ///   Stelt de naam van de tabel in op "Tijdslogs".
        /// </summary>
        public TijdslogData()
        {
            TableName = "Tijdslogs";
        }

        /// <summary>
        ///   Eigenschap die de naam van de tabel specificeert.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        ///   Selecteert alle records uit de tabel "Tijdslogs".
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
        ///   Voegt een nieuw timelogrecord toe aan de tabel "Tijdslogs" in de database.
        ///   Dit maakt gebruik van de generieke <see cref="SqlServer.InsertRecord(SqlCommand)"/> methode.
        /// </summary>
        /// <param name="tijdslog">
        ///   Een <see cref="Tijdslog"/> object dat de gegevens van het timelog bevat, zoals inlogtijd en gebruiker-ID.
        /// </param>
        /// <returns>
        ///   Een <see cref="InsertResult"/> object dat aangeeft of het toevoegen succesvol was en het ID van het nieuwe record bevat.
        /// </returns>
        public InsertResult Insert(Tijdslog tijdslog)
        {
            var result = new InsertResult();
            try
            {
                // Stel het SQL-invoegcommando samen
                string insertQuery = $"INSERT INTO {TableName} (InlogTime, GebruikerID) VALUES (@InlogTime, @GebruikerID);";

                // Maak een SqlCommand aan en voeg parameters toe
                using (SqlCommand insertCommand = new SqlCommand(insertQuery))
                {
                    insertCommand.Parameters.Add("@InlogTime", SqlDbType.DateTime).Value = tijdslog.InlogTime;
                    insertCommand.Parameters.Add("@GebruikerID", SqlDbType.Int).Value = tijdslog.GebruikerID;

                    // Roep de InsertRecord-methode aan om het commando uit te voeren
                    result = InsertRecord(insertCommand);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"SQL Fout bij het toevoegen van een timelog: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Algemene fout bij het toevoegen van een timelog: {ex.Message}", ex);
            }
            return result;
        }


        /// <summary>
        ///   Selecteert de laatste inlogtijden van alle gebruikers.
        ///   Dit maakt gebruik van de generieke <see cref="SqlServer.ExecuteSelect(string, Dictionary{string, object})"/> methode.
        /// </summary>
        /// <returns>
        ///   Een <see cref="SelectResult"/> object met de laatste inlogtijden per gebruiker.
        /// </returns>
        public SelectResult SelectAllLastLoginTimes()
        {
            string query = @"
                SELECT 
                    GebruikerID, 
                    MAX(InlogTime) AS LastLoginTime
                FROM 
                    Tijdslogs
                GROUP BY 
                    GebruikerID";

            return ExecuteSelect(query, null);
        }

        public SelectResult SelectDailyLoginCounts()
        {
            string query = @"
        SELECT 
            CAST(InlogTime AS DATE) AS Datum,
            COUNT(*) AS AantalLogins
        FROM 
            Tijdslogs
        GROUP BY 
            CAST(InlogTime AS DATE)
        ORDER BY 
            Datum ASC";

            return ExecuteSelect(query, null);
        }

        
    }
}
