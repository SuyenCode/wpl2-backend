namespace ClassLibTeam12.Business.Entities
{
    public class Job
    {
        public int JobID { get; set; }
        public string Naam { get; set; }
        public string Beschrijving { get; set; }
        public int? Positie { get; set; }
        public int BedrijfID { get; set; }

    }
}
