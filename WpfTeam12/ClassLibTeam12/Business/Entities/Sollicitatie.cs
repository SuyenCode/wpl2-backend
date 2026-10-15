using System;

namespace ClassLibTeam12.Business.Entities
{
    public class Sollicitatie
    {
        public DateTime Datum { get; set; }
        public int GebruikerID { get; set; }
        public int JobID { get; set; }
        public int SollicitatieID { get; set; }
        public bool Status { get; set; }
    }
}
