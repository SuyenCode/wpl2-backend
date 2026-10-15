using ClassLibTeam12.Business.Entities;
using ClassLibTeam12.Data;
using ClassLibTeam12.Data.Framework;
using System;
using System.Collections.Generic;
using System.Data;

namespace ClassLibTeam12.Business
{
    /// <summary>
    ///  Deze statische klasse biedt methoden voor het beheren van sollicitaties.
    /// </summary>
    public static class Sollicitaties
    {
        /// <summary>
        /// Haalt alle sollicitaties op.
        /// </summary>
        /// <returns>Een SelectResult-object met de resultaten van het ophalen van de sollicitaties.</returns>
        public static SelectResult GetSollicitaties()
        {
            SollicitatieData sollicitatieData = new SollicitatieData();
            SelectResult result = sollicitatieData.Select();
            return result;
        }

        /// <summary>
        /// Voegt een nieuwe sollicitatie toe.
        /// </summary>
        /// <param name="datum">De datum van de sollicitatie.</param>
        /// <returns>Een InsertResult-object met de resultaten van het toevoegen van de sollicitatie.</returns>
        public static InsertResult Add(DateTime datum, int gebruikerId, int jobId, bool status)
        {
            Sollicitatie sollicitatie = new Sollicitatie
            {
                Datum = datum,
                GebruikerID = gebruikerId,
                JobID = jobId,
                Status = status
            };

            SollicitatieData sollicitatieData = new SollicitatieData();
            InsertResult result = sollicitatieData.Insert(sollicitatie);
            return result;
        }
        public static SelectResult GetAllSollicitaties()
        {
            SollicitatieData sollicitatieData = new SollicitatieData();
            return sollicitatieData.SelectAllSollicitaties();
        } 
        public static List<object> ProcessAllSollicitaties(DataTable dataTable)
        {
            var allSollicitaties = new List<object>();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    allSollicitaties.Add(new
                    {
                        SollicitatieId = row["SollicitatieID"],
                        VolledigeNaam = row["VolledigeNaam"],
                        JobNaam = row["Naam"],
                        Datum = row["Datum"],
                        Status = row["Status"]
                    });

                }
            }
            return allSollicitaties;
        }
        public static UpdateResult CancelSollicitatie(int sollicitatieId, int gebruikerId)
        {
            SollicitatieData data = new SollicitatieData();
            return data.CancelSollicitatieData(sollicitatieId, gebruikerId);
        }

        public static SelectResult GetFilteredSollicitaties(int? sollicitatieId, string volledigeNaam = null, string jobNaam = null, bool? status = null, DateTime? vanafDatum = null, DateTime? totDatum = null)
        {
            SollicitatieData sollicitatieData = new SollicitatieData();
            return sollicitatieData.SelectFilteredSollicitaties(sollicitatieId, volledigeNaam, jobNaam, status, vanafDatum, totDatum);
        }
        public static InsertResult AddMail(DateTime datum, int gebruikerId, int jobId)
        {
            Sollicitatie sollicitatie = new Sollicitatie
            {
                Datum = datum,
                GebruikerID = gebruikerId,
                JobID = jobId,
                Status = true
            };

            //dit nakijken
            SollicitatieData sollicitatieData = new SollicitatieData();
            InsertResult result = sollicitatieData.Insert(sollicitatie);
            return result;
        }

        public static UpdateResult UpdateSollicitatieAdmin(int? sollicitatieId, int? gebruikerId, int? jobId, bool? status)
        {
            SollicitatieData sollicitatieData = new SollicitatieData();
            return sollicitatieData.AdminUpdateSollicitatie(sollicitatieId, gebruikerId, jobId, status);
        }

        public static SelectResult GetSollicitatieById(int sollicitatieId)
        {
            SollicitatieData sollicitatieData = new SollicitatieData();
            return sollicitatieData.SelectById(sollicitatieId);
        }

        public static DeleteResult DeleteBulkSollicitaties(List<int> idsToDelete)
        {
            SollicitatieData sollicitatieData = new SollicitatieData();
            DeleteResult result = sollicitatieData.BulkDelete(idsToDelete);
            return result;
        }

        public static UpdateResult DeleteSollicitaties(int sollicitatieId)
        {
            SollicitatieData deleteSollicitatie = new SollicitatieData();
            return deleteSollicitatie.DeleteSingleSollicitaties(sollicitatieId);
        }

    }
}
