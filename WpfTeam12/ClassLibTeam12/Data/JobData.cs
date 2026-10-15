using ClassLibTeam12.Business;
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
    /// Deze interne klasse biedt methoden voor het beheren van jobgegevens in de database.
    /// Alle SQL-commando's worden uitgevoerd via de <see cref="SqlServer"/> klasse.
    /// </summary>
    internal class JobData : SqlServer
    {
        /// <summary>
        /// Constructor voor de klasse <see cref="JobData"/>.
        /// Stelt de naam van de tabel in op "Jobs".
        /// </summary>
        public JobData()
        {
            TableName = "Jobs";
        }

        /// <summary>
        /// Eigenschap die de naam van de tabel specificeert.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Selecteert alle records uit de tabel "Jobs".
        /// Dit maakt gebruik van de generieke <see cref="SqlServer.Select(string)"/> methode.
        /// </summary>
        /// <returns>
        /// Een <see cref="SelectResult"/> object dat alle gegevens bevat die zijn opgehaald uit de tabel.
        /// </returns>
        public SelectResult Select()
        {
            return base.Select(TableName);
        }

        /// <summary>
        /// Voegt een nieuwe job toe aan de tabel "Jobs" in de database.
        /// Dit maakt gebruik van de generieke <see cref="SqlServer.InsertRecord(SqlCommand)"/> methode.
        /// </summary>
        /// <param name="job">
        /// Een <see cref="Job"/> object dat de gegevens van de job bevat, zoals naam, beschrijving, locatie, positie en bedrijf-ID.
        /// </param>
        /// <returns>
        /// Een <see cref="InsertResult"/> object dat aangeeft of het toevoegen succesvol was en het ID van het nieuwe record bevat.
        /// </returns>
        public InsertResult Insert(Job job)
        {
            var result = new InsertResult();
            try
            {
                // Stel het SQL-invoegcommando samen
                string insertQuery = $"INSERT INTO {TableName} (Naam, Beschrijving, Positie, BedrijfID) VALUES (@Naam, @Beschrijving, @Positie, @BedrijfID);";

                // Maak een SqlCommand aan en voeg parameters toe
                using (SqlCommand insertCommand = new SqlCommand(insertQuery))
                {
                    insertCommand.Parameters.Add("@Naam", SqlDbType.VarChar).Value = job.Naam;
                    insertCommand.Parameters.Add("@Beschrijving", SqlDbType.VarChar).Value = job.Beschrijving;
                    insertCommand.Parameters.Add("@Positie", SqlDbType.Int).Value = job.Positie;
                    insertCommand.Parameters.Add("@BedrijfID", SqlDbType.Int).Value = job.BedrijfID;

                    // Roep de InsertRecord-methode aan om het commando uit te voeren
                    result = InsertRecord(insertCommand);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"SQL Fout bij het toevoegen van een nieuwe job: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Algemene fout bij het toevoegen van een nieuwe job: {ex.Message}", ex);
            }
            return result;
        }

        /// <summary>
        /// Filtert jobs op basis van opgegeven criteria.
        /// Maakt gebruik van de generieke <see cref="SqlServer.ExecuteSelect(string, Dictionary{string, object})"/> methode.
        /// </summary>
        /// <param name="naam">De naam om op te filteren.</param>
        /// <param name="beschrijving">De beschrijving om op te filteren.</param>
        /// <param name="locatie">De locatie om op te filteren.</param>
        /// <param name="positie">De positie om op te filteren.</param>
        /// <returns>Een lijst van gefilterde jobs.</returns>
        public SelectResult FilteredJobs(int? jobID, string naam, string beschrijving, int? positie, string bedrijfNaam)
        {
            StringBuilder query = new StringBuilder(@"
        SELECT J.JobID, J.Naam, J.Beschrijving, J.Positie, B.Naam AS BedrijfNaam FROM Jobs J
        
        LEFT JOIN Bedrijven B ON J.BedrijfID = B.BedrijfID
        WHERE 1=1");  // Initial WHERE clause to make the first condition simple

            List<string> conditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            // ID-filter (exacte match)
            if (jobID.HasValue)
            {
                conditions.Add("J.JobID LIKE @JobID");
                parameters.Add("@JobID", $"{jobID}%");
            }

            if (!string.IsNullOrWhiteSpace(naam))
            {
                conditions.Add("J.Naam LIKE @Naam");
                parameters.Add("@Naam", $"{naam}%");
            }

            if (!string.IsNullOrWhiteSpace(beschrijving))
            {
                conditions.Add("J.Beschrijving LIKE @Beschrijving");
                parameters.Add("@Beschrijving", $"{beschrijving}%");
            }

            if (positie.HasValue)
            {
                conditions.Add("J.Positie LIKE @Positie");
                parameters.Add("@Positie", $"%{positie}%");
            }

            if (!string.IsNullOrWhiteSpace(bedrijfNaam))
            {
                conditions.Add("B.Naam LIKE @BedrijfNaam");
                parameters.Add("@BedrijfNaam", $"{bedrijfNaam}%");
            }

            
            // If there are any conditions, append them to the query
            if (conditions.Any())
            {
                query.Append(" AND " + string.Join(" AND ", conditions));
            }

            return ExecuteSelect(query.ToString(), parameters);
        }

        /// <summary>
        /// Haalt gedetailleerde informatie over een specifieke job op.
        /// </summary>
        /// <param name="jobID">Het ID van de job.</param>
        /// <returns>Een <see cref="SelectResult"/> object met gedetailleerde informatie over de job.</returns>
        internal SelectResult SelectJobDetails(int? jobID)
        {
            SelectResult result = new SelectResult();
            result.DataTable = new DataTable();

            try
            {
                string sql = @"SELECT J.*, B.Naam AS BedrijfNaam, B.Locatie AS BedrijfLocatie
                               FROM Jobs J
                               LEFT JOIN Bedrijven B ON J.BedrijfID = B.BedrijfID
                               WHERE J.JobID = @JobID";

                var parameters = new Dictionary<string, object>
                {
                    { "@JobID", jobID }
                };

                return ExecuteSelect(sql, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.Succeeded = false;
                return result; // Return the result object even in case of an exception
            }
        }
        public UpdateResult UpdateJobs(int JobId, string naam, string beschrijving, int positie, int bedrijfID)
        {
            UpdateResult result = new UpdateResult();
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();
                    //SQL Command
                    StringBuilder updateQuery = new StringBuilder();
                    updateQuery.Append($"UPDATE Jobs SET Naam = @Naam, Beschrijving = @Beschrijving, Positie = @positie, BedrijfID = @BedrijfID");
                    updateQuery.Append($"WHERE JobID = @JobID");
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery.ToString(), conn))
                    {
                        updateCommand.Parameters.AddWithValue("@JobId", JobId);
                        updateCommand.Parameters.AddWithValue("@Naam", naam);
                        updateCommand.Parameters.AddWithValue("@Beschrijving", beschrijving);
                        updateCommand.Parameters.AddWithValue("@Positie", positie);
                        updateCommand.Parameters.AddWithValue("@BedrijfID", bedrijfID);

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
        public UpdateResult DeleteJobs(int jobId)
        {
            UpdateResult updateResult = new UpdateResult();

            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Verwijder sollicitaties die deze job gebruiken
                            string deleteSollicitatiesQuery = "DELETE FROM Sollicitaties WHERE JobID = @JobID";
                            using (SqlCommand cmd = new SqlCommand(deleteSollicitatiesQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@JobID", jobId);
                                cmd.ExecuteNonQuery();
                            }

                            // Verwijder job
                            string deleteJobQuery = "DELETE FROM Jobs WHERE JobID = @JobID";
                            using (SqlCommand cmd = new SqlCommand(deleteJobQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@JobID", jobId);
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected == 0)
                                {
                                    throw new Exception("Job niet gevonden of kon niet worden verwijderd.");
                                }
                            }

                            transaction.Commit();
                            updateResult.Succeeded = true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            updateResult.Succeeded = false;
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
                using (SqlCommand deleteCommand = new SqlCommand())
                {
                    // 1. Eerst inschrijvingen verwijderen met deze jobId's als foreign key
                    StringBuilder deleteInschrijvingenQuery = new StringBuilder();
                    deleteInschrijvingenQuery.Append("DELETE FROM Sollicitaties WHERE JobID IN (");
                    for (int i = 0; i < idsToDelete.Count; i++)
                    {
                        deleteInschrijvingenQuery.Append($"@Id{i}");
                        if (i < idsToDelete.Count - 1)
                            deleteInschrijvingenQuery.Append(", ");
                    }
                    deleteInschrijvingenQuery.Append(");");

                    // 2. Daarna jobs zelf verwijderen
                    StringBuilder deleteJobsQuery = new StringBuilder();
                    deleteJobsQuery.Append("DELETE FROM Jobs WHERE JobID IN (");
                    for (int i = 0; i < idsToDelete.Count; i++)
                    {
                        deleteJobsQuery.Append($"@Id{i}");
                        if (i < idsToDelete.Count - 1)
                            deleteJobsQuery.Append(", ");
                    }
                    deleteJobsQuery.Append(");");

                    // Combineer beide queries
                    deleteCommand.CommandText = deleteInschrijvingenQuery.ToString() + " " + deleteJobsQuery.ToString();

                    // Voeg parameters toe
                    for (int i = 0; i < idsToDelete.Count; i++)
                    {
                        deleteCommand.Parameters.Add($"@Id{i}", SqlDbType.Int).Value = idsToDelete[i];
                    }

                    result = DeleteRecords(deleteCommand);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return result;
        }


        public SelectResult SelectAllJobs()
        {
            SelectResult result = new SelectResult();
            result.DataTable = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();
                    string sql = "SELECT J.JobID, J.Naam, J.Beschrijving, J.Positie, B.Naam AS BedrijfNaam FROM Jobs J LEFT JOIN Bedrijven B ON J.BedrijfID = B.BedrijfID";
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
    }
}
