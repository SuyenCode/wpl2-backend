using ClassLibTeam12.Business.Entities;
using ClassLibTeam12.Data.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Linq;
using System.Collections;
using ClassLibTeam12.Business;

namespace ClassLibTeam12.Data
{
    /// <summary>
    ///   Deze interne klasse biedt methoden voor het beheren van sollicitatiegegevens in de database.
    ///   Alle SQL-commando's worden uitgevoerd via de <see cref="SqlServer"/> klasse.
    /// </summary>
    internal class SollicitatieData : SqlServer
    {
        /// <summary>
        ///   Initialiseert een nieuw exemplaar van de <see cref="SollicitatieData"/> klasse.
        ///   Stelt de naam van de tabel in op "Sollicitaties".
        /// </summary>
        public SollicitatieData()
        {
            TableName = "Sollicitaties";
        }

        /// <summary>
        ///   Eigenschap die de naam van de tabel specificeert.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        ///   Selecteert alle records uit de tabel "Sollicitaties".
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
        ///   Voegt een nieuwe sollicitatie toe aan de tabel "Sollicitaties" in de database.
        ///   Dit maakt gebruik van de generieke <see cref="SqlServer.InsertRecord(SqlCommand)"/> methode.
        /// </summary>
        /// <param name="sollicitatie">
        ///   Een <see cref="Sollicitatie"/> object dat de gegevens van de sollicitatie bevat, zoals datum, job-ID en gebruiker-ID.
        /// </param>
        /// <returns>
        ///   Een <see cref="InsertResult"/> object dat aangeeft of het toevoegen succesvol was en het ID van het nieuwe record bevat.
        /// </returns>
        public InsertResult Insert(Sollicitatie sollicitatie)
        {
            var result = new InsertResult();
            try
            {
                // Stel het SQL-invoegcommando samen
                string insertQuery = $"INSERT INTO {TableName} (Datum, JobID, GebruikerID, Status) VALUES (@Datum, @JobID, @GebruikerID, @Status);";

                using (SqlCommand insertCommand = new SqlCommand(insertQuery))
                {
                    insertCommand.Parameters.Add("@Datum", SqlDbType.DateTime).Value = sollicitatie.Datum;
                    insertCommand.Parameters.Add("@JobID", SqlDbType.Int).Value = sollicitatie.JobID;
                    insertCommand.Parameters.Add("@GebruikerID", SqlDbType.Int).Value = sollicitatie.GebruikerID;
                    insertCommand.Parameters.Add("@Status", SqlDbType.Bit).Value = sollicitatie.Status;

                    result = InsertRecord(insertCommand);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"SQL Fout bij het toevoegen van een nieuwe sollicitatie: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Algemene fout bij het toevoegen van een nieuwe sollicitatie: {ex.Message}", ex);
            }
            return result;
        }


        /// <summary>
        ///   Haalt gedetailleerde informatie over een specifieke sollicitatie op basis van ID.
        ///   Dit maakt gebruik van de generieke <see cref="SqlServer.ExecuteSelect(string, Dictionary{string, object})"/> methode.
        /// </summary>
        /// <param name="sollicitatieId">Het ID van de sollicitatie.</param>
        /// <returns>
        ///   Een <see cref="SelectResult"/> object dat gegevens bevat over de geselecteerde sollicitatie.
        /// </returns>
        public SelectResult GetById(int sollicitatieId)
        {
            try
            {
                string query = "SELECT * FROM Sollicitaties WHERE SollicitatieID = @SollicitatieID";
                var parameters = new Dictionary<string, object> { { "@SollicitatieID", sollicitatieId } };

                // Roep ExecuteSelect aan om het commando uit te voeren
                return ExecuteSelect(query, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Fout bij het ophalen van een sollicitatie met ID {sollicitatieId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        ///   Wijzigt een bestaande sollicitatie in de tabel "Sollicitaties".
        ///   Dit maakt gebruik van de generieke <see cref="SqlServer.UpdateRecord(SqlCommand)"/> methode.
        /// </summary>
        /// <param name="sollicitatie">
        ///   Een <see cref="Sollicitatie"/> object dat de bijgewerkte gegevens van de sollicitatie bevat, zoals datum, job-ID en gebruiker-ID.
        /// </param>
        /// <returns>
        ///   Een <see cref="ResetResult"/> object dat aangeeft of de update succesvol was.
        /// </returns>
        public ResetResult Update(Sollicitatie sollicitatie)
        {
            try
            {
                string updateQuery = "UPDATE Sollicitaties SET Datum = @Datum, JobID = @JobID, GebruikerID = @GebruikerID WHERE SollicitatieID = @SollicitatieID";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery))
                {
                    updateCommand.Parameters.Add("@Datum", SqlDbType.DateTime).Value = sollicitatie.Datum;
                    updateCommand.Parameters.Add("@JobID", SqlDbType.Int).Value = sollicitatie.JobID;
                    updateCommand.Parameters.Add("@GebruikerID", SqlDbType.Int).Value = sollicitatie.GebruikerID;
                    updateCommand.Parameters.Add("@SollicitatieID", SqlDbType.Int).Value = sollicitatie.SollicitatieID;

                    // Roep UpdateRecord aan om het commando uit te voeren
                    return UpdateRecord(updateCommand);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fout bij het bijwerken van een sollicitatie met ID {sollicitatie.SollicitatieID}: {ex.Message}", ex);
            }
        }

        public SelectResult SelectAllSollicitaties()
        {
            SelectResult result = new SelectResult();
            result.DataTable = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();
                    string sql = "SELECT SollicitatieID, CONCAT(G.Voornaam, ' ', G.Achternaam) AS VolledigeNaam, J.Naam, S.Datum, S.Status FROM Sollicitaties S LEFT JOIN Gebruikers G ON S.GebruikerID = G.GebruikerID LEFT JOIN Jobs J ON S.JobID = J.JobID";
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
        public UpdateResult CancelSollicitatieData(int sollicitatieId, int gebruikerId)
        {
            UpdateResult result = new UpdateResult();
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();
                    StringBuilder updateQuery = new StringBuilder();
                    updateQuery.Append("UPDATE Sollicitaties SET Status = 0 ");
                    updateQuery.Append("WHERE SollicitatieID = @SollicitatieID AND GebruikerID = @GebruikerID");

                    using (SqlCommand cmd = new SqlCommand(updateQuery.ToString(), conn))
                    {
                        cmd.Parameters.AddWithValue("@SollicitatieID", sollicitatieId);
                        cmd.Parameters.AddWithValue("@GebruikerID", gebruikerId);

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
        public SelectResult SelectFilteredSollicitaties(
            int? sollicitatieId,
    string volledigeNaam = null,
    string jobNaam = null,
    bool? status = null,
    DateTime? vanafDatum = null,
    DateTime? totDatum = null)
        {
            StringBuilder baseQuery = new StringBuilder (@"
SELECT s.SollicitatieID, CONCAT(g.Voornaam, ' ', g.Achternaam) AS VolledigeNaam, s.JobID, j.Naam AS Naam, s.Datum, s.Status
FROM Sollicitaties s
LEFT JOIN Jobs j ON s.JobID = j.JobID
LEFT JOIN Gebruikers g on g.GebruikerID = s.GebruikerID");
            List<string> filters = new List<string>();
            var parameters = new Dictionary<string, object>();

            if (sollicitatieId.HasValue)
            {
                filters.Add("CAST(s.SollicitatieID AS VARCHAR) LIKE @SollicitatieID");
                parameters.Add("@SollicitatieID", $"{sollicitatieId.Value}%");
            }

            if (!string.IsNullOrEmpty(volledigeNaam))
            {
                // Let op: filter op CONCAT van voornaam en achternaam
                filters.Add("CONCAT(g.Voornaam, ' ', g.Achternaam) LIKE @VolledigeNaam");
                parameters.Add("@VolledigeNaam", $"{volledigeNaam}%");
            }

            if (!string.IsNullOrEmpty(jobNaam))
            {
                filters.Add("j.Naam LIKE @Naam");
                parameters.Add("@Naam", $"{jobNaam}%");
            }

            if (status.HasValue)
            {
                filters.Add("s.Status = @Status");
                parameters.Add("@Status", status.Value);
            }

            if (vanafDatum.HasValue)
            {
                filters.Add("s.Datum >= @VanafDatum");
                parameters.Add("@VanafDatum", vanafDatum.Value);
            }

            if (totDatum.HasValue)
            {
                filters.Add("s.Datum <= @TotDatum");
                parameters.Add("@TotDatum", totDatum.Value);
            }

            if (filters.Any())
            {
                baseQuery.Append(" WHERE " + string.Join(" AND ", filters));
            }

            return ExecuteSelect(baseQuery.ToString(), parameters);
        }




        public UpdateResult AdminUpdateSollicitatie(int? sollicitatieId, int? gebruikerId, int? jobId, bool? status)
        {
            UpdateResult result = new UpdateResult();

            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();

                    string sql = @"UPDATE Sollicitaties
                        SET GebruikerID = @GebruikerID, 
                        JobID = @JobID, 
                        Status = @Status
                        WHERE SollicitatieID = @SollicitatieID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@SollicitatieID", sollicitatieId);
                        cmd.Parameters.AddWithValue("@GebruikerID", gebruikerId);
                        cmd.Parameters.AddWithValue("@JobID", jobId);
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

        public SelectResult SelectById(int sollicitatieId)
        {
            SelectResult result = new SelectResult();
            result.DataTable = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();

                    string sql = @"SELECT s.SollicitatieID, s.GebruikerID, s.JobID, s.Datum, s.Status, CONCAT(g.Voornaam, ' ', g.Achternaam) AS VolledigeNaam, j.Naam AS JobNaam
                        FROM Sollicitaties s
                        JOIN Gebruikers g ON s.GebruikerID = g.GebruikerID 
                        JOIN Jobs j ON s.JobID = j.JobID
                        WHERE s.SollicitatieID = @SollicitatieID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@SollicitatieID", sollicitatieId);
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

        public DeleteResult BulkDelete(List<int> idsToDelete)
        {
            var result = new DeleteResult();
            try
            {
                // 1. Sollicitaties verwijderen op basis van SollicitatieId
                StringBuilder deleteSollicitatiesQuery = new StringBuilder();
                deleteSollicitatiesQuery.Append("DELETE FROM Sollicitaties WHERE SollicitatieId IN (");
                for (int i = 0; i < idsToDelete.Count; i++)
                {
                    deleteSollicitatiesQuery.Append($"@Id{i}");
                    if (i < idsToDelete.Count - 1)
                        deleteSollicitatiesQuery.Append(", ");
                }
                deleteSollicitatiesQuery.Append(");");

                using (SqlCommand deleteSollicitatiesCommand = new SqlCommand(deleteSollicitatiesQuery.ToString()))
                {
                    for (int i = 0; i < idsToDelete.Count; i++)
                    {
                        deleteSollicitatiesCommand.Parameters.Add($"@Id{i}", SqlDbType.Int).Value = idsToDelete[i];
                    }
                    result = DeleteRecords(deleteSollicitatiesCommand);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return result;
        }

        public UpdateResult DeleteSingleSollicitaties(int sollicitatieId)
        {
            UpdateResult updateResult = new UpdateResult();
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();
                    //SQL Command
                    string deleteQuery = $"DELETE FROM SOLLICITATIES WHERE SOLLICITATIEID = @SollicitatieID";
                    using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, conn))
                    {
                        deleteCommand.Parameters.AddWithValue("@SollicitatieID", sollicitatieId);

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
    }
}
