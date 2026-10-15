using ClassLibTeam12.Business.Entities;
using ClassLibTeam12.Data;
using ClassLibTeam12.Data.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace ClassLibTeam12.Business
{
    /// <summary>
    /// Deze statische klasse biedt methoden voor het beheren van inschrijvingen.
    /// </summary>
    public static class Inschrijvingen
    {
        /// <summary>
        /// Haalt alle inschrijvingen op uit de database.
        /// </summary>
        /// <returns>
        /// Een <see cref="SelectResult"/>-object dat de resultaten bevat van het ophalen van de inschrijvingen.
        /// </returns>
        public static SelectResult GetInschrijvingen()
        {
            InschrijvingData gebruikerData = new InschrijvingData();
            SelectResult result = gebruikerData.Select();
            return result;
        }

        /// <summary>
        /// Voegt een nieuwe inschrijving toe aan de database.
        /// </summary>
        /// <param name="datum">De datum en tijd van de inschrijving.</param>
        /// <param name="gebruikerId">Het ID van de gebruiker die zich inschrijft.</param>
        /// <param name="inschrijvingId">Het ID van de inschrijving waarvoor wordt ingeschreven.</param>
        /// <returns>
        /// Een <see cref="InsertResult"/>-object dat de resultaten bevat van het toevoegen van de inschrijving.
        /// </returns>
        /// <remarks>
        /// Zorg ervoor dat zowel de gebruiker als de inschrijving bestaan voordat je deze methode aanroept.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Wordt opgegooid als een van de parameters ongeldig is, bijvoorbeeld een negatieve gebruikerId of inschrijvingId.
        /// </exception>
        public static InsertResult AddEmail(DateTime datum, int gebruikerId, int trainingId)
        {
            if (gebruikerId <= 0 || trainingId <= 0)
            {
                throw new ArgumentException("GebruikerID en TrainingID moeten groter zijn dan 0.");
            }

            Inschrijving inschrijving = new Inschrijving
            {
                Datum = datum,
                GebruikerID = gebruikerId,
                TrainingID = trainingId
            };

            InschrijvingData inschrijvingData = new InschrijvingData();
            InsertResult result = inschrijvingData.InsertMail(inschrijving);
            return result;
        }
        public static InsertResult Add(int gebruikerId, int trainingId, DateTime datum, bool status)
        {
            if (gebruikerId <= 0)
            {
                throw new ArgumentException("GebruikerID groter zijn dan 0.");
            }

            Inschrijving inschrijving = new Inschrijving
            {
                GebruikerID = gebruikerId,
                TrainingID = trainingId,
                Datum = datum,
                Status = status
            };

            InschrijvingData inschrijvingData = new InschrijvingData();
            InsertResult result = inschrijvingData.Insert(inschrijving);
            return result;
        }

        public static List<object> ProcessAllInschrijvingen(DataTable dataTable)
        {
            var allInschrijvingen = new List<object>();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    allInschrijvingen.Add(new
                    {
                        InschrijvingId = row["InschrijvingID"],
                        VolledigeNaam = row["VolledigeNaam"],
                        TrainingTitel = row["TrainingNaam"],
                        Datum = row["Datum"],
                        Status = row["Status"],
                             
                    });
                }
            }
            return allInschrijvingen;
        }

        public static SelectResult GetAllInschrijvingen()
        {
            InschrijvingData inschrijvingData = new InschrijvingData();
            return inschrijvingData.SelectAllInschrijvingen();
        }

        public static UpdateResult CancelInschrijving(int inschrijvingID, int gebruikerID)
        {
            InschrijvingData inschrijvingData = new InschrijvingData();
            return inschrijvingData.CancelInschrijvingData(inschrijvingID, gebruikerID);
        }

        public static SelectResult getTrainingNaam(int trainingId, int gebruikerID)
        {
            InschrijvingData inschrijvingData = new InschrijvingData();
            return inschrijvingData.SearchTrainingNaam(trainingId, gebruikerID);
        }

        public static UpdateResult UpdateInschrijvingAdmin(int? inschrijvingId, int? gebruikerId, int? trainingId, bool? status)
        {
            InschrijvingData inschrijvingData = new InschrijvingData();
            return inschrijvingData.AdminUpdateInschrijving(inschrijvingId, gebruikerId, trainingId, status);
        }

        public static SelectResult GetFilteredInschrijvingen(int? inschrijvingId, string volledigeNaam, string trainingNaam = null, bool? status = null, DateTime? vanafDatum = null, DateTime? totDatum = null)
        {
            InschrijvingData inschrijvingData = new InschrijvingData();
            return inschrijvingData.SelectFilteredInscrijvingen(inschrijvingId, volledigeNaam, trainingNaam, status, vanafDatum, totDatum);
        }

        public static SelectResult GetInschrijvingById(int inschrijvingId)
        {
            InschrijvingData inschrijvingData = new InschrijvingData();
            return inschrijvingData.SelectById(inschrijvingId);
        }

        public static string ConvertDataTableToJson(DataTable dataTable)
        {
            return JsonConvert.SerializeObject(dataTable);
        }

        public static UpdateResult DeleteInschrijvingen(int inschrijvingId)
        {
            InschrijvingData deleteInschrijving = new InschrijvingData();
            return deleteInschrijving.DeleteSingleInschrijvingen(inschrijvingId);
        }

        public static DeleteResult DeleteBulkInschrijving(List<int> idsToDelete)
        {
            InschrijvingData inschrijvingData = new InschrijvingData();
            DeleteResult result = inschrijvingData.BulkDelete(idsToDelete);
            return result;
        }
    }
}
