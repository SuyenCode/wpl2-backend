using ClassLibTeam12.Business;
using ClassLibTeam12.Business.Entities;
using ClassLibTeam12.Data.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClassLibTeam12.Data
{
    /// <summary>
    /// Deze interne klasse biedt methoden voor het beheren van inschrijvingsgegevens in de database.
    /// Alle SQL-commando's worden uitgevoerd via de <see cref="SqlServer"/> klasse.
    /// </summary>
    internal class InschrijvingData : SqlServer
    {
        /// <summary>
        /// Constructor voor de klasse <see cref="InschrijvingData"/>.
        /// Stelt de naam van de tabel in op "Inschrijvingen".
        /// </summary>
        public InschrijvingData()
        {
            TableName = "Inschrijvingen";
        }

        /// <summary>
        /// Eigenschap die de naam van de tabel specificeert.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Selecteert alle records uit de tabel "Inschrijvingen".
        /// Dit maakt gebruik van de generieke <see cref="SqlServer.Select(string)"/> methode.
        /// </summary>
        /// <returns>
        /// Een <see cref="SelectResult"/> object dat alle gegevens bevat die zijn opgehaald uit de tabel.
        /// </returns>
        public SelectResult Select()
        {
            return base.Select(TableName);
        }
        public InsertResult InsertMail(Inschrijving inschrijving)
        {
            var result = new InsertResult();
            try
            {
                // Stel het SQL-invoegcommando samen
                StringBuilder insertQuery = new StringBuilder();
                insertQuery.Append($"INSERT INTO {TableName} ");
                insertQuery.Append($"(Datum, GebruikerID, TrainingId, Status) VALUES ");
                insertQuery.Append($"(@Inschrijvingsdatum, @GebruikerID, @TrainingId, 1);");

                // Maak een SqlCommand aan en voeg parameters toe
                using (SqlCommand insertCommand = new SqlCommand(insertQuery.ToString()))
                {
                    insertCommand.Parameters.Add("@Inschrijvingsdatum", SqlDbType.DateTime).Value = inschrijving.Datum;
                    insertCommand.Parameters.Add("@GebruikerID", SqlDbType.Int).Value = inschrijving.GebruikerID;
                    insertCommand.Parameters.Add("@TrainingID", SqlDbType.Int).Value = inschrijving.TrainingID;

                    // Roep de InsertRecord-methode aan om het commando uit te voeren
                    result = InsertRecord(insertCommand);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fout bij het toevoegen van een nieuwe inschrijving: {ex.Message}", ex);
            }
            return result;
        }
        /// <summary>
        /// Voegt een nieuwe inschrijving toe aan de tabel "Inschrijvingen" in de database.
        /// Dit maakt gebruik van de generieke <see cref="SqlServer.InsertRecord(SqlCommand)"/> methode.
        /// </summary>
        /// <param name="inschrijving">
        /// Een <see cref="Inschrijving"/> object dat de gegevens van de inschrijving bevat, zoals inschrijvingsdatum, gebruiker-ID en inschrijving-ID.
        /// </param>
        /// <returns>
        /// Een <see cref="InsertResult"/> object dat aangeeft of het toevoegen succesvol was en het ID van het nieuwe record bevat.
        /// </returns>
        public InsertResult Insert(Inschrijving inschrijving)
        {
            var result = new InsertResult();
            try
            {
                // Stel het SQL-invoegcommando samen
                StringBuilder insertQuery = new StringBuilder();
                insertQuery.Append($"INSERT INTO {TableName} ");
                insertQuery.Append($"(GebruikerID, TrainingID, Datum, Status) VALUES ");
                insertQuery.Append($"(@GebruikerID, @TrainingID, @Datum, @Status);");

                // Maak een SqlCommand aan en voeg parameters toe
                using (SqlCommand insertCommand = new SqlCommand(insertQuery.ToString()))
                {
                    //insertCommand.Parameters.Add("@InschrijvingID", SqlDbType.Int).Value = inschrijving.InschrijvingID;
                    insertCommand.Parameters.Add("@GebruikerID", SqlDbType.Int).Value = inschrijving.GebruikerID;
                    insertCommand.Parameters.Add("@TrainingID", SqlDbType.Int).Value = inschrijving.TrainingID;
                    insertCommand.Parameters.Add("@Datum", SqlDbType.DateTime).Value = inschrijving.Datum;
                    insertCommand.Parameters.Add("@Status", SqlDbType.Bit).Value = inschrijving.Status;

                    // Roep de InsertRecord-methode aan om het commando uit te voeren
                    result = InsertRecord(insertCommand);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fout bij het toevoegen van een nieuwe inschrijving: {ex.Message}", ex);
            }
            return result;
        }

        public SelectResult SelectAllInschrijvingen()
        {
            SelectResult result = new SelectResult();
            result.DataTable = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();
                    string sql = "SELECT i.InschrijvingID, i.Datum, i.Status, CONCAT(g.Voornaam, ' ', g.Achternaam) AS VolledigeNaam, t.Naam AS TrainingNaam FROM Inschrijvingen i JOIN Gebruikers g ON i.GebruikerID = g.GebruikerID JOIN Trainingen t ON i.TrainingID = t.TrainingID";
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
            catch (Exception)
            {
                result.Succeeded = false;
            }
            return result;
        }

        public UpdateResult CancelInschrijvingData(int inschrijvingid, int gebruikerID)
        {
            UpdateResult result = new UpdateResult();
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();
                    //SQL Command
                    StringBuilder updateQuery = new StringBuilder();
                    updateQuery.Append($"UPDATE Inschrijvingen SET Status = 0 ");
                    updateQuery.Append($"WHERE InschrijvingID = @InschrijvingID AND GebruikerID = @GebruikerID");
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery.ToString(), conn))
                    {
                        updateCommand.Parameters.AddWithValue("@InschrijvingID", inschrijvingid);
                        updateCommand.Parameters.AddWithValue("@GebruikerID", gebruikerID);

                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            result.Succeeded = true;
                        }
                        else
                        {
                            result.Succeeded = false;
                            throw new Exception("Something went wrong.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return result;
        }

        public SelectResult SearchTrainingNaam(int trainingId, int gebruikerID)
        {
            SelectResult result = new SelectResult();
            result.DataTable = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();
                    string sql = "select top 1 T.Naam from inschrijvingen I left join Trainingen T on I.TrainingID = T.TrainingID where I.TrainingID = @TrainingID and I.GebruikerID = @GebruikerID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@TrainingID", trainingId);
                        cmd.Parameters.AddWithValue("@GebruikerID", gebruikerID); 
                        
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
            catch (Exception)
            {
                result.Succeeded = false;
            }
            return result;
        }

        public UpdateResult AdminUpdateInschrijving(int? inschrijvingId, int? gebruikerId, int? trainingId, bool? status)
        {
            UpdateResult result = new UpdateResult();

            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();

                    string sql = @"UPDATE Inschrijvingen
                        SET GebruikerID = @GebruikerID, 
                        TrainingID = @TrainingID, 
                        Status = @Status

                        WHERE InschrijvingID = @InschrijvingID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@InschrijvingID", inschrijvingId);
                        cmd.Parameters.AddWithValue("@GebruikerID", gebruikerId);
                        cmd.Parameters.AddWithValue("@TrainingID", trainingId);
                        cmd.Parameters.AddWithValue("@Status", status);

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
        public SelectResult SelectFilteredInscrijvingen(
      int? inschrijvingId,
      string volledigeNaam,
      string trainingNaam,
      bool? status,
      DateTime? vanafDatum,
      DateTime? totDatum)
        {
            StringBuilder query = new StringBuilder(@"
        SELECT
            i.InschrijvingID,
            CONCAT(g.Voornaam, ' ', g.Achternaam) AS VolledigeNaam,
            t.Naam AS TrainingNaam,
            i.Datum,
            i.Status
        FROM Inschrijvingen i
        LEFT JOIN Trainingen t ON i.TrainingID = t.TrainingID
        LEFT JOIN Gebruikers g ON g.GebruikerID = i.GebruikerID
        WHERE 1=1
    ");

            List<string> filters = new List<string>();
            var parameters = new Dictionary<string, object>();

            if (inschrijvingId.HasValue)
            {
                filters.Add("i.InschrijvingID LIKE @InschrijvingID");
                parameters.Add("@InschrijvingID", $"{inschrijvingId.Value}%");
            }

            if (!string.IsNullOrWhiteSpace(volledigeNaam))
            {
                filters.Add("CONCAT(g.Voornaam, ' ', g.Achternaam) LIKE @VolledigeNaam");
                parameters.Add("@VolledigeNaam", $"{volledigeNaam}%");
            }

            if (!string.IsNullOrWhiteSpace(trainingNaam))
            {
                filters.Add("t.Naam LIKE @TrainingNaam");
                parameters.Add("@TrainingNaam", $"{trainingNaam}%");
            }

            if (status.HasValue)
            {
                filters.Add("i.Status = @Status");
                parameters.Add("@Status", status.Value);
            }

            if (vanafDatum.HasValue)
            {
                filters.Add("i.Datum >= @VanafDatum");
                parameters.Add("@VanafDatum", vanafDatum.Value);
            }

            if (totDatum.HasValue)
            {
                filters.Add("i.Datum <= @TotDatum");
                parameters.Add("@TotDatum", totDatum.Value);
            }

            
            if (filters.Any())
            {
                query.Append(" AND " + string.Join(" AND ", filters));
            }

            return ExecuteSelect(query.ToString(), parameters);
        }

        public SelectResult SelectById(int inschrijvingId)
        {
            SelectResult result = new SelectResult();
            result.DataTable = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();

                    string sql = @"SELECT i.InschrijvingID, i.GebruikerID, i.TrainingID, i.Datum, i.Status, CONCAT(g.Voornaam, ' ', g.Achternaam) AS VolledigeNaam, t.Naam AS TrainingTitel
                        FROM Inschrijvingen i
                        JOIN Gebruikers g ON i.GebruikerID = g.GebruikerID
                        JOIN Trainingen t ON i.TrainingID = t.TrainingID
                        WHERE i.InschrijvingID = @InschrijvingID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@InschrijvingID", inschrijvingId);
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
            catch (Exception)
            {
                result.Succeeded = false;
            }
            return result;
        }

        public UpdateResult DeleteSingleInschrijvingen(int inschrijvingeId)
        {
            UpdateResult updateResult = new UpdateResult();
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();
                    //SQL Command
                    string deleteQuery = $"DELETE FROM Inschrijvingen WHERE InschrijvingID = @InschrijvingID";
                    using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, conn))
                    {
                        deleteCommand.Parameters.AddWithValue("@InschrijvingID", inschrijvingeId);

                        int rowsAffected = deleteCommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            updateResult.Succeeded = true;
                        }
                        else
                        {
                            updateResult.Succeeded = false;
                            throw new Exception("Something went wrong.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                updateResult.Succeeded = false;
            }
            return updateResult;
        }

        public DeleteResult BulkDelete(List<int> idsToDelete)
        {
            var result = new DeleteResult();
            try
            {
                // 1. Inschrijvingen verwijderen op basis van InschrijvingId
                StringBuilder deleteInschrijvingenQuery = new StringBuilder();
                deleteInschrijvingenQuery.Append("DELETE FROM Inschrijvingen WHERE InschrijvingId IN (");
                for (int i = 0; i < idsToDelete.Count; i++)
                {
                    deleteInschrijvingenQuery.Append($"@Id{i}");
                    if (i < idsToDelete.Count - 1)
                        deleteInschrijvingenQuery.Append(", ");
                }
                deleteInschrijvingenQuery.Append(");");

                using (SqlCommand deleteInschrijvingenCommand = new SqlCommand(deleteInschrijvingenQuery.ToString()))
                {
                    for (int i = 0; i < idsToDelete.Count; i++)
                    {
                        deleteInschrijvingenCommand.Parameters.Add($"@Id{i}", SqlDbType.Int).Value = idsToDelete[i];
                    }
                    result = DeleteRecords(deleteInschrijvingenCommand);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return result;
        }
    }
}
