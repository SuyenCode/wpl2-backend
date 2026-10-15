using ClassLibTeam12.Business.Entities;
using ClassLibTeam12.Business.Validation;
using ClassLibTeam12.Data;
using ClassLibTeam12.Data.Framework;
using System;
using System.Collections.Generic;
using System.Data;


namespace ClassLibTeam12.Business
{
    /// <summary>
    ///  Deze statische klasse biedt methoden voor het beheren van trainingsgegevens.
    /// </summary>
    public static class Trainingen
    {
        /// <summary>
        /// Haalt alle trainingsrecords op.
        /// </summary>
        /// <returns>Een SelectResult-object met de resultaten van het ophalen van de trainingen.</returns>
        public static SelectResult GetTrainingen()
        {
            TrainingData trainingData = new TrainingData();
            SelectResult result = trainingData.Select();
            return result;
        }

        /// <summary>
        /// Voegt een nieuw trainingsrecord toe.
        /// </summary>
        /// <param name="naam">De naam van de training.</param>
        /// <param name="beschrijving">Een beschrijving van de training.</param>
        /// <param name="categorie">De categorie van de training.</param>
        /// <returns>Een InsertResult-object met de resultaten van het toevoegen van de training.</returns>
        public static InsertResult Add(bool status, string naam, string learningMethod, string duration, string beschrijving, string categorie, int bedrijfID)
        {
            Training training = new Training
            {
                Status = status,
                Naam = naam,
                LearningMethod = learningMethod,
                Duration = duration,
                Beschrijving = beschrijving,
                Categorie = categorie,
                BedrijfID = bedrijfID
            };
            TrainingData trainingData = new TrainingData();
            return trainingData.Insert(training);
        }

        public static SelectResult GetFilteredTrainingen(int? trainingID, string naam, string beschrijving, string categorie, string bedrijfNaam, bool? status, string duration)
        {
            TrainingData trainingData = new TrainingData();
            return trainingData.FilteredTrainingen(trainingID, naam, beschrijving, categorie, bedrijfNaam, status, duration);
        }
        public static List<object> ProcessAllTrainingen(DataTable dataTable)
        {
            var allTrainingen = new List<object>();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    allTrainingen.Add(new
                    {
                        TrainingId = row["TrainingID"],
                        Naam = row["Naam"],
                        Beschrijving = row["Beschrijving"],
                        Categorie = row["Categorie"],
                        BedrijfNaam = row["BedrijfNaam"],
                        //learningMethod = row["LearningMethod"],
                        Duration = row["Duration"],
                        Status = row["Status"]
                    });

                }
            }
            return allTrainingen;
        }
        public static SelectResult GetAllTrainingen()
        {
            TrainingData trainingData = new TrainingData();
            return trainingData.SelectAllTrainingen();
        }
      

        public static UpdateResult UpdateTrainingen(int trainingID, bool status, string naam, string learningMethod, string duration, string beschrijving, string categorie)
        {

            TrainingData trainingData = new TrainingData();
            return trainingData.UpdateTrainingen(trainingID, status, naam, beschrijving, categorie, learningMethod, duration);
        }

        public static UpdateResult DeleteTrainingen(int trainingId)
        {
            TrainingData deleteTraining = new TrainingData();
            return deleteTraining.DeleteTrainingen(trainingId);
        }

        /// <summary>
        /// Haalt details op voor een specifieke training.
        /// </summary>
        /// <param name="trainingID">Het ID van de training die moet worden opgehaald.</param>
        /// <returns>Een SelectResult-object met de resultaten van het ophalen van de trainingdetails.</returns>
        public static SelectResult GetTrainingDetails(int? trainingID)
        {
            TrainingData trainingData = new TrainingData();
            SelectResult result = trainingData.SelectTrainingDetails(trainingID);
            return result;
        }

        /// <summary>
        /// Haalt de index op van een rij in een DataTable op basis van TrainingID.
        /// </summary>
        /// <param name="dataTable">De DataTable waarin gezocht moet worden.</param>
        /// <param name="trainingID">Het TrainingID waarnaar gezocht moet worden.</param>
        /// <returns>Geeft de index van het TrainingID terug of null als deze niet overeenkomt.</returns>
        public static int? GetRowIndexByTrainingID(DataTable dataTable, int? trainingID)
        {
            if (dataTable == null || dataTable.Rows.Count == 0 || !trainingID.HasValue)
            {
                return null;
            }

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                // Controleer of de waarde in de kolom "TrainingID" overeenkomt met de opgegeven trainingID
                if (dataTable.Rows[i]["TrainingID"] != DBNull.Value &&
                    Convert.ToInt32(dataTable.Rows[i]["TrainingID"]) == trainingID.Value)
                {
                    return i; // Retourneer de index van de rij
                }
            }

            return null; // Geen overeenkomende TrainingID gevonden
        }

        public static DeleteResult DeleteBulkTrainingen(List<int> idsToDelete)
        {
            TrainingData trainingData = new TrainingData();
            DeleteResult result = trainingData.BulkDelete(idsToDelete);
            return result;
        }
    }
}
