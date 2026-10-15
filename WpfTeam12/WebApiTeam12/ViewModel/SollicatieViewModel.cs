namespace WebApiTeam12.ViewModel
{
    public class SollicatieViewModel
    {
        public DateTime Datum => DateTime.Now;
        public int GebruikerID { get; set; }
        public int JobID { get; set; }
        public bool Status { get; set; }
    }
}
