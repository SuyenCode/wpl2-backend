using ClassLibTeam12.Business.Entities;
using ClassLibTeam12.Data;
using ClassLibTeam12.Data.Framework;
using System;
using System.Collections.Generic;
using System.Data;

namespace ClassLibTeam12.Business
{
    /// <summary>
    ///  Deze statische klasse biedt methoden voor het beheren van tijdlogs (Tijdslogs).
    /// </summary>
    public static class TijdslogHelper
    {

        public static List<object> ProcessAllTijdslogs(DataTable dataTable)
        {
            var allTijdslogs = new List<object>();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    allTijdslogs.Add(new
                    {
                        GebruikerId = row["GebruikerID"],
                        InlogTime = row["InlogTime"]

                    });
                }
            }
            return allTijdslogs;
        }

        /// <summary>
        /// Voegt een nieuw tijdlogrecord toe.
        /// </summary>
        /// <param name="datumtijd">De datum en tijd van het logrecord.</param>
        /// <returns>Een InsertResult-object met de resultaten van het toevoegen van het tijdlog.</returns>
        public static InsertResult Add(DateTime datumtijd, int gebruikerId)
        {
            Entities.Tijdslog tijdslog = new Entities.Tijdslog
            {
                InlogTime = datumtijd,
                GebruikerID = gebruikerId
            };

            TijdslogData tijdslogData = new TijdslogData();
            InsertResult result = tijdslogData.Insert(tijdslog);
            return result;
        }

        /// <summary>
        /// Voegt een inlogtijd toe voor een specifieke gebruiker.
        /// </summary>
        /// <param name="gebruikerId">Het ID van de gebruiker die inlogt.</param>
        /// <returns>Een InsertResult-object met de resultaten van het toevoegen van de inlogtijd.</returns>
        //public static InsertResult InsertLoginTime(int gebruikerId)
        //{
        //    TijdslogData tijdslogData = new TijdslogData();
        //    return tijdslogData.InsertTimelog(gebruikerId);
        //}
        public static InsertResult InsertLoginTime(int gebruikerId)
        {
            DateTime now = DateTime.Now;

            Entities.Tijdslog tijdslog = new Entities.Tijdslog
            {
                InlogTime = now,
                GebruikerID = gebruikerId
            };

            TijdslogData tijdslogData = new TijdslogData();
            return tijdslogData.Insert(tijdslog);
        }

        /// <summary>
        /// Selecteert alle laatste inlogtijden.
        /// </summary>
        /// <returns>Een SelectResult-object met de resultaten van de selectie.</returns>
        public static SelectResult SelectDailyLoginCounts()
        {
            TijdslogData tijdslogData = new TijdslogData();
            return tijdslogData.SelectDailyLoginCounts();
        }
        public static SelectResult GetTijdslogs()
        {
            TijdslogData tijdslogData = new TijdslogData();
            return tijdslogData.SelectAllLastLoginTimes();
        }


    }
}
