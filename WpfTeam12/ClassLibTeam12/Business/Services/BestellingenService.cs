using ClassLibTeam12.Business.DTO;
using ClassLibTeam12.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibTeam12.Business.Services
{
    public class BestellingenService
    {
        private readonly BestellingData _bestellingData;

        public BestellingenService()
        {
            _bestellingData = new BestellingData();
        }

        public List<BestellingDTO> GetBestellingenVoorGebruiker(int gebruikerId)
        {
            return _bestellingData.GetBestellingenVoorGebruiker(gebruikerId);
        }

        public List<BestellingDTO> GetFilteredBestellingen(int gebruikerId, string type, string naam, bool? status, DateTime? vanafDatum, DateTime? totDatum)
        {
            return _bestellingData.GetFilteredBestellingen(gebruikerId, type, naam, status, vanafDatum, totDatum);
        }





    }

}
