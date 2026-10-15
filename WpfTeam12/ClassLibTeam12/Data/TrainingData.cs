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
    /// Interne klasse die methoden biedt voor het beheren van trainingsgegevens in de database.
    /// </summary>

    internal class TrainingData : SqlServer
    {
        /// <summary>
        /// Constructor voor de klasse `TrainingData`.
        /// Stelt de naam van de tabel in op "Trainingen".
        /// </summary>

        public TrainingData()
        {
            TableName = "Trainingen";
        }

        /// <summary>
        /// Eigenschap die de naam van de tabel specificeert.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Selecteert alle records uit de tabel "Training".
        /// </summary>
        /// <returns>Een <see cref="SelectResult"/> object met alle trainingsgegevens.</returns>
        public SelectResult Select()
        {
            return base.Select(TableName);
        }

        /// <summary>
        /// Voegt een nieuwe training toe aan de database.
        /// </summary>
        /// <param name="training">Een <see cref="Training"/> object met gegevens van de training, inclusief de naam, beschrijving, categorie en BedrijfID.</param>
        /// <returns>Een <see cref="InsertResult"/> object dat aangeeft of de operatie succesvol was.</returns>
        public InsertResult Insert(Training training)
        {
            var result = new InsertResult();
            try
            {
                // SQL Command
                StringBuilder insertQuery = new StringBuilder();
                insertQuery.Append($"Insert INTO {TableName} ");
                insertQuery.Append($"(Naam, Beschrijving, Categorie, BedrijfID, LearningMethod, Duration, Status) VALUES ");
                insertQuery.Append($"(@Naam, @Beschrijving, @Categorie, @BedrijfID, @LearningMethod, @Duration, @Status); ");

                using (SqlCommand insertCommand = new SqlCommand(insertQuery.ToString()))
                {
                    // Voeg parameters toe om SQL-injectie te voorkomen
                    insertCommand.Parameters.AddWithValue("@Naam", SqlDbType.VarChar).Value =
                    training.Naam;
                    insertCommand.Parameters.AddWithValue("@Beschrijving", SqlDbType.VarChar).Value =
                    training.Beschrijving;
                    insertCommand.Parameters.AddWithValue("@Categorie", SqlDbType.VarChar).Value =
                    training.Categorie;
                    insertCommand.Parameters.AddWithValue("@LearningMethod", SqlDbType.VarChar).Value =
                    training.LearningMethod;
                    insertCommand.Parameters.AddWithValue("@Duration", SqlDbType.VarChar).Value =
                    training.Duration;
                    insertCommand.Parameters.AddWithValue("@Status", SqlDbType.Bit).Value =
                    training.Status;
                    insertCommand.Parameters.AddWithValue("@BedrijfID", SqlDbType.Int).Value =
                    training.BedrijfID;

                    // Voer het SQL-commando uit en sla het resultaat op
                    result = InsertRecord(insertCommand);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
        public SelectResult SelectAllTrainingen()
        {
            SelectResult result = new SelectResult();
            result.DataTable = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();
                    string sql = "SELECT T.TrainingID, T.Naam, T.Beschrijving, T.Categorie, B.Naam AS BedrijfNaam, T.Status, T.Duration FROM Trainingen T LEFT JOIN Bedrijven B ON T.BedrijfID = B.BedrijfID";
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

        public SelectResult FilteredTrainingen(
    int? trainingId,
    string naam,
    string beschrijving,
    string categorie,
    string bedrijfNaam,
    bool? status,
    string duration)
        {
            StringBuilder query = new StringBuilder(@"
        SELECT T.TrainingID, T.Naam, T.Beschrijving, T.Categorie, B.Naam AS BedrijfNaam, T.Status, T.Duration FROM Trainingen T
        
        LEFT JOIN Bedrijven B ON T.BedrijfID = B.BedrijfID
        WHERE 1=1");  // Initial WHERE clause to make the first condition simple

            List<string> conditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            // ID-filter (exacte match)
            if (trainingId.HasValue)
            {
                conditions.Add("T.TrainingID LIKE @TrainingID");
                parameters.Add("@TrainingID", $"{trainingId}%");
            }

            if (!string.IsNullOrWhiteSpace(naam))
            {
                conditions.Add("T.Naam LIKE @Naam");
                parameters.Add("@Naam", $"{naam}%");
            }

            if (!string.IsNullOrWhiteSpace(beschrijving))
            {
                conditions.Add("T.Beschrijving LIKE @Beschrijving");
                parameters.Add("@Beschrijving", $"{beschrijving}%");
            }

            if (!string.IsNullOrWhiteSpace(categorie))
            {
                conditions.Add("T.Categorie LIKE @Categorie");
                parameters.Add("@Categorie", $"{categorie}%");
            }

            if (!string.IsNullOrWhiteSpace(bedrijfNaam))
            {
                conditions.Add("B.Naam LIKE @BedrijfNaam");
                parameters.Add("@BedrijfNaam", $"%{bedrijfNaam}%");
            }

            if (status.HasValue)
            {
                conditions.Add("T.Status = @Status");
                parameters.Add("@Status", status.Value);
            }

            if (!string.IsNullOrWhiteSpace(duration))
            {
                conditions.Add("T.Duration LIKE @Duration");
                parameters.Add("@Duration", $"%{duration}%");
            }

            // If there are any conditions, append them to the query
            if (conditions.Any())
            {
                query.Append(" AND " + string.Join(" AND ", conditions));
            }

            return ExecuteSelect(query.ToString(), parameters);
        }



        /// <summary>
        /// Haalt gedetailleerde informatie over een specifieke training op.
        /// </summary>
        /// <param name="trainingID">Het ID van de training.</param>
        /// <returns>Een <see cref="SelectResult"/> object met gedetailleerde informatie over de training.</returns>
        public SelectResult SelectTrainingDetails(int? trainingID)
        {
            SelectResult result = new SelectResult();
            result.DataTable = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();
                    string sql = @"SELECT T.*, B.Naam AS BedrijfNaam, B.Locatie AS BedrijfLocatie
                                   FROM Training T
                                   LEFT JOIN Bedrijven B ON T.BedrijfID = B.BedrijfID
                                   WHERE T.TrainingID = @TrainingID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@TrainingID", trainingID);
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

        public UpdateResult UpdateTrainingen(int trainingId, bool status, string naam, string beschrijving, string categorie, string learningMethod, string duration)
        {
            UpdateResult result = new UpdateResult();
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();
                    //SQL Command
                    StringBuilder updateQuery = new StringBuilder();
                    updateQuery.Append($"UPDATE Trainingen SET Naam = @Naam, Beschrijving = @Beschrijving, Categorie = @Categorie, LearningMethod = @LearningMethod, Duration = @Duration, Status = @Status ");
                    updateQuery.Append($"WHERE TrainingID = @TrainingID");
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery.ToString(), conn))
                    {
                        updateCommand.Parameters.AddWithValue("@TrainingID", trainingId);
                        updateCommand.Parameters.AddWithValue("@Naam", naam);
                        updateCommand.Parameters.AddWithValue("@Beschrijving", beschrijving);
                        updateCommand.Parameters.AddWithValue("@Categorie", categorie);
                        updateCommand.Parameters.AddWithValue("@LearningMethod", learningMethod);
                        updateCommand.Parameters.AddWithValue("@Duration", duration);
                        updateCommand.Parameters.AddWithValue("@Status", status);

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
        public UpdateResult DeleteTrainingen(int trainingId)
        {
            UpdateResult updateResult = new UpdateResult();
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    conn.Open();
                    //SQL Command
                    string deleteQuery = $"DELETE FROM TRAININGEN WHERE TRAININGID = @TRAININGID";
                    using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, conn))
                    {
                        deleteCommand.Parameters.AddWithValue("@TrainingID", trainingId);

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
                using (SqlCommand deleteCommand = new SqlCommand())
                {
                    // 1. Eerst inschrijvingen verwijderen met deze trainingId's als foreign key
                    StringBuilder deleteInschrijvingenQuery = new StringBuilder();
                    deleteInschrijvingenQuery.Append("DELETE FROM Inschrijvingen WHERE TrainingID IN (");
                    for (int i = 0; i < idsToDelete.Count; i++)
                    {
                        deleteInschrijvingenQuery.Append($"@Id{i}");
                        if (i < idsToDelete.Count - 1)
                            deleteInschrijvingenQuery.Append(", ");
                    }
                    deleteInschrijvingenQuery.Append(");");

                    // 2. Daarna trainingen zelf verwijderen
                    StringBuilder deleteTrainingenQuery = new StringBuilder();
                    deleteTrainingenQuery.Append("DELETE FROM Trainingen WHERE TrainingID IN (");
                    for (int i = 0; i < idsToDelete.Count; i++)
                    {
                        deleteTrainingenQuery.Append($"@Id{i}");
                        if (i < idsToDelete.Count - 1)
                            deleteTrainingenQuery.Append(", ");
                    }
                    deleteTrainingenQuery.Append(");");

                    // Combineer beide queries
                    deleteCommand.CommandText = deleteInschrijvingenQuery.ToString() + " " + deleteTrainingenQuery.ToString();

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

    }
}
