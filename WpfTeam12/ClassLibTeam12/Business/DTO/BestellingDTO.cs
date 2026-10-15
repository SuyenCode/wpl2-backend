using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibTeam12.Business.DTO
{
    public class BestellingDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }  // "Inschrijving" of "Sollicitatie"
        public string Naam { get; set; }  // TrainingNaam of JobNaam
        public DateTime Datum { get; set; }
        public bool Status { get; set; }
    }
}
