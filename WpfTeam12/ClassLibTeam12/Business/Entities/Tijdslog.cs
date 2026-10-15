using System;


namespace ClassLibTeam12.Business.Entities
{
    public class Tijdslog
    {
        public int Id { get; set; }  
        public int GebruikerID { get; set; }  
        public DateTime InlogTime { get; set; }

        //public Gebruiker Gebruiker { get; set; }
    }
}
