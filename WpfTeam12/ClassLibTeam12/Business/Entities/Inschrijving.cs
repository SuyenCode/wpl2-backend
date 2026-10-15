using ClassLibTeam12.Data.Framework;
using System;

namespace ClassLibTeam12.Business.Entities
{
    public class Inschrijving
    {
        public int InschrijvingID { get; set; }
        public int GebruikerID { get; set; }
        public int TrainingID { get; set; }
        public DateTime Datum { get; set; }
        public bool Status { get; set; }
    }
}
