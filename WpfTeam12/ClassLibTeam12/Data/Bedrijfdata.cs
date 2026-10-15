using ClassLibTeam12.Business.Entities;
using ClassLibTeam12.Data.Framework;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ClassLibTeam12.Data
{
    /// <summary>
    ///   Deze interne klasse biedt methoden voor het beheren van bedrijfsgegevens in de database.
    ///   Alle SQL-commando's worden uitgevoerd via de <see cref="SqlServer"/> klasse.
    /// </summary>
    internal class BedrijfData : SqlServer
    {
        /// <summary>
        ///   Initialiseert een nieuw exemplaar van de <see cref="BedrijfData"/> klasse.
        /// </summary>
        public BedrijfData()
        {
            TableName = "Bedrijven";
        }

        /// <summary>
        ///   Eigenschap die de naam van de tabel in de database specificeert.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        ///   Selecteert alle records uit de tabel "Bedrijven".
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
        ///   Voegt een nieuw bedrijf toe aan de tabel "Bedrijven" in de database.
        ///   Dit maakt gebruik van de generieke <see cref="SqlServer.InsertRecord(SqlCommand)"/> methode.
        /// </summary>
        /// <param name="bedrijf">
        ///   Een <see cref="Bedrijf"/> object dat de gegevens van het bedrijf bevat, zoals naam en locatie.
        /// </param>
        /// <returns>
        ///   Een <see cref="InsertResult"/> object dat aangeeft of het toevoegen succesvol was en het ID van het nieuwe record bevat.
        /// </returns>
        public InsertResult Insert(Bedrijf bedrijf)
        {
            var result = new InsertResult();
            try
            {
                // Stel het SQL-invoegcommando samen
                StringBuilder insertQuery = new StringBuilder();
                insertQuery.Append($"INSERT INTO {TableName} ");
                insertQuery.Append($"(Naam, Locatie) VALUES ");
                insertQuery.Append($"(@Naam, @Locatie);");

                // Maak een SqlCommand aan en voeg parameters toe
                using (SqlCommand insertCommand = new SqlCommand(insertQuery.ToString()))
                {
                    insertCommand.Parameters.Add("@Naam", SqlDbType.VarChar).Value = bedrijf.Naam;
                    insertCommand.Parameters.Add("@Locatie", SqlDbType.VarChar).Value = bedrijf.Locatie;

                    // Roep de InsertRecord-methode aan om het commando uit te voeren
                    result = InsertRecord(insertCommand);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fout bij het invoegen van een nieuw bedrijf: {ex.Message}");
            }
            return result;
        }
    }
}
