using ClassLibTeam12.Business.Entities;
using ClassLibTeam12.Data;
using ClassLibTeam12.Data.Framework;
using System;
using System.Collections.Generic;
using System.Data;

namespace ClassLibTeam12.Business
{
    /// <summary>
    ///  Deze statische klasse biedt methoden voor het beheren van vacatures.
    /// </summary>
    public static class Jobs
    {
        /// <summary>
        /// Haalt alle vacatures op.
        /// </summary>
        /// <returns>Een SelectResult-object met de resultaten van het ophalen van de vacatures.</returns>
        public static SelectResult GetJobs()
        {
            JobData jobData = new JobData();
            SelectResult result = jobData.Select();
            return result;
        }

        /// <summary>
        /// Voegt een nieuwe vacature toe.
        /// </summary>
        /// <param name="naam">De naam van de vacature.</param>
        /// <param name="beschrijving">Een beschrijving van de vacature.</param>
        /// <param name="locatie">De locatie van de vacature.</param>
        /// <param name="positie">Het aantal beschikbare posities.</param>
        /// <returns>Een InsertResult-object met de resultaten van het toevoegen van de vacature.</returns>
        public static InsertResult Add(string naam, string beschrijving, int? positie, int bedrijfid)
        {
            Job job = new Job
            {
                Naam = naam,
                Beschrijving = beschrijving,
                Positie = positie.Value,
                BedrijfID = bedrijfid
            };

            JobData jobData = new JobData();
            InsertResult result = jobData.Insert(job);
            return result;
        }

        /// <summary>
        /// Filtert vacatures op basis van zoekcriteria.
        /// </summary>
        /// <param name="naam">De naam om op te filteren.</param>
        /// <param name="beschrijving">De beschrijving om op te filteren.</param>
        /// <param name="locatie">De locatie om op te filteren.</param>
        /// <param name="positie">Het aantal posities om op te filteren.</param>
        /// <returns>Een lijst met gefilterde vacatures.</returns>
        public static SelectResult FilteredJobs(int? jobID, string naam, string beschrijving, int? positie, string bedrijfNaam)
        {
            JobData jobData = new JobData();
            return jobData.FilteredJobs(jobID, naam, beschrijving, positie, bedrijfNaam);
        }

        /// <summary>
        /// Haalt details op voor een specifieke vacature.
        /// </summary>
        /// <param name="jobID">Het ID van de vacature die moet worden opgehaald.</param>
        /// <returns>Een SelectResult-object met de resultaten van het ophalen van de vacaturedetails.</returns>
        public static SelectResult GetJobDetails(int? jobID)
        {
            JobData jobData = new JobData();
            SelectResult result = jobData.SelectJobDetails(jobID);
            return result;
        }

        /// <summary>
        /// Haalt de index op van een rij in een DataTable op basis van JobID.
        /// </summary>
        /// <param name="dataTable">De DataTable waarin gezocht moet worden.</param>
        /// <param name="jobID">Het JobID waarnaar gezocht moet worden.</param>
        /// <returns>Geeft de index terug van het JobID of null als deze niet overeenkomt.</returns>
        public static int? GetRowIndexByJobID(DataTable dataTable, int? jobID)
        {
            if (dataTable == null || dataTable.Rows.Count == 0 || !jobID.HasValue)
            {
                return null;
            }

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (dataTable.Rows[i]["JobID"] != DBNull.Value &&
                    Convert.ToInt32(dataTable.Rows[i]["JobID"]) == jobID.Value)
                {
                    return i; // Retourneer de index van de rij
                }
            }

            return null; // Geen overeenkomend JobID gevonden
        }
        public static UpdateResult UpdateJobs(int jobId, string naam, string beschrijving, int positie, int bedrijfID)
        {

            JobData jobData = new JobData();
            return jobData.UpdateJobs(jobId, naam, beschrijving, positie, bedrijfID);
        }

        public static UpdateResult DeleteJob(int jobID)
        {
            JobData deleteJobs = new JobData();
            return deleteJobs.DeleteJobs(jobID);
        }

        public static DeleteResult DeleteJobs(List<int> idsToDelete)
        {
            JobData jobData = new JobData();
            DeleteResult result = jobData.BulkDelete(idsToDelete);
            return result;
        }

        public static SelectResult GetAllJobs()
        {
            JobData jobData = new JobData();
            return jobData.SelectAllJobs();
        }

        public static List<object> ProcessAllJobs(DataTable dataTable)
        {
            var allJobs = new List<object>();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    allJobs.Add(new
                    {
                        JobId = row["JobID"],
                        Naam = row["Naam"],
                        Beschrijving = row["Beschrijving"],
                        Positie = row["Positie"],
                        BedrijfNaam = row["BedrijfNaam"],
                    });

                }
            }
            return allJobs;
        }

        public static SelectResult GetFilteredJobs(int? jobID, string naam, string beschrijving, int? positie, string bedrijfNaam)
        {
            JobData jobData = new JobData();
            return jobData.FilteredJobs(jobID, naam, beschrijving, positie, bedrijfNaam);
        }
    }
}
