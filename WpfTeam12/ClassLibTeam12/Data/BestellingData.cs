using ClassLibTeam12.Business.DTO;
using ClassLibTeam12.Data.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ClassLibTeam12.Data
{
    public class BestellingData
    {
        public List<BestellingDTO> GetBestellingenVoorGebruiker(int gebruikerId)
        {
            var bestellingen = new List<BestellingDTO>();

            using (var conn = new SqlConnection(Settings.GetConnectionString()))
            {
                conn.Open();

                // Inschrijvingen
                string inschrijvingQuery = @"
        SELECT 
            i.InschrijvingID AS Id,
            'Inschrijving' AS Type,
            t.Naam AS Naam,
            i.Datum,
            i.Status
        FROM Inschrijvingen i
        LEFT JOIN Trainingen t ON i.TrainingID = t.TrainingID
        WHERE i.GebruikerID = @GebruikerID";

                using (var cmd = new SqlCommand(inschrijvingQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@GebruikerID", gebruikerId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bestellingen.Add(new BestellingDTO
                            {
                                Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                Type = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                Naam = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Datum = reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3),
                                Status = reader.IsDBNull(4) ? false : reader.GetBoolean(4)
                            });
                        }
                    }
                }

                // Sollicitaties
                string sollicitatieQuery = @"
        SELECT 
            s.SollicitatieID AS Id,
            'Sollicitatie' AS Type,
            j.Naam AS Naam,
            s.Datum,
            s.Status
        FROM Sollicitaties s
        INNER JOIN Jobs j ON s.JobID = j.JobID
        WHERE s.GebruikerID = @GebruikerID";

                using (var cmd = new SqlCommand(sollicitatieQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@GebruikerID", gebruikerId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bestellingen.Add(new BestellingDTO
                            {
                                Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                Type = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                Naam = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Datum = reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3),
                                Status = reader.IsDBNull(4) ? false : reader.GetBoolean(4)
                            });
                        }
                    }
                }
            }

            return bestellingen;
        }


        public List<BestellingDTO> GetFilteredBestellingen(int? gebruikerId, string type, string naam, bool? status, DateTime? vanafDatum, DateTime? totDatum)
        {
            var bestellingen = new List<BestellingDTO>();

            using (var conn = new SqlConnection(Settings.GetConnectionString()))
            {
                conn.Open();

                // Inschrijvingen
                var inschrijvingQuery = new StringBuilder(@"
        SELECT 
            i.InschrijvingID AS Id,
            'Inschrijving' AS Type,
            t.Naam AS Naam,
            i.Datum,
            i.Status
        FROM Inschrijvingen i
        LEFT JOIN Trainingen t ON i.TrainingID = t.TrainingID
        WHERE 1 = 1");

                var inschrijvingCmd = new SqlCommand();
                inschrijvingCmd.Connection = conn;

                if (gebruikerId.HasValue)
                {
                    inschrijvingQuery.Append(" AND i.GebruikerID = @GebruikerID");
                    inschrijvingCmd.Parameters.AddWithValue("@GebruikerID", gebruikerId.Value);
                }

                if (status.HasValue)
                {
                    inschrijvingQuery.Append(" AND i.Status = @Status");
                    inschrijvingCmd.Parameters.AddWithValue("@Status", status.Value);
                }

                if (vanafDatum.HasValue)
                {
                    inschrijvingQuery.Append(" AND i.Datum >= @VanafDatum");
                    inschrijvingCmd.Parameters.AddWithValue("@VanafDatum", vanafDatum.Value);
                }

                if (totDatum.HasValue)
                {
                    inschrijvingQuery.Append(" AND i.Datum <= @TotDatum");
                    inschrijvingCmd.Parameters.AddWithValue("@TotDatum", totDatum.Value);
                }

                if (!string.IsNullOrEmpty(naam))
                {
                    inschrijvingQuery.Append(" AND t.Naam LIKE @Naam");
                    inschrijvingCmd.Parameters.AddWithValue("@Naam", naam + "%");
                }

                if (type != "Sollicitatie")
                {
                    inschrijvingCmd.CommandText = inschrijvingQuery.ToString();
                    using (var reader = inschrijvingCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bestellingen.Add(new BestellingDTO
                            {
                                Id = reader.GetInt32(0),
                                Type = reader.GetString(1),
                                Naam = reader.GetString(2),
                                Datum = reader.GetDateTime(3),
                                Status = reader.GetBoolean(4)
                            });
                        }
                    }
                }

                // Sollicitaties
                var sollicitatieQuery = new StringBuilder(@"
        SELECT 
            s.SollicitatieID AS Id,
            'Sollicitatie' AS Type,
            j.Naam AS Naam,
            s.Datum,
            s.Status
        FROM Sollicitaties s
        LEFT JOIN Jobs j ON s.JobID = j.JobID
        WHERE 1 = 1");

                var sollicitatieCmd = new SqlCommand();
                sollicitatieCmd.Connection = conn;

                if (gebruikerId.HasValue)
                {
                    sollicitatieQuery.Append(" AND s.GebruikerID = @GebruikerID");
                    sollicitatieCmd.Parameters.AddWithValue("@GebruikerID", gebruikerId.Value);
                }

                if (status.HasValue)
                {
                    sollicitatieQuery.Append(" AND s.Status = @Status");
                    sollicitatieCmd.Parameters.AddWithValue("@Status", status.Value);
                }

                if (vanafDatum.HasValue)
                {
                    sollicitatieQuery.Append(" AND s.Datum >= @VanafDatum");
                    sollicitatieCmd.Parameters.AddWithValue("@VanafDatum", vanafDatum.Value);
                }

                if (totDatum.HasValue)
                {
                    sollicitatieQuery.Append(" AND s.Datum <= @TotDatum");
                    sollicitatieCmd.Parameters.AddWithValue("@TotDatum", totDatum.Value);
                }

                if (!string.IsNullOrEmpty(naam))
                {
                    sollicitatieQuery.Append(" AND j.Naam LIKE @Naam");
                    sollicitatieCmd.Parameters.AddWithValue("@Naam", naam + "%");
                }

                if (type != "Inschrijving")
                {
                    sollicitatieCmd.CommandText = sollicitatieQuery.ToString();
                    using (var reader = sollicitatieCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bestellingen.Add(new BestellingDTO
                            {
                                Id = reader.GetInt32(0),
                                Type = reader.GetString(1),
                                Naam = reader.GetString(2),
                                Datum = reader.GetDateTime(3),
                                Status = reader.GetBoolean(4)
                            });
                        }
                    }
                }
            }

            return bestellingen;
        }

    }
}
