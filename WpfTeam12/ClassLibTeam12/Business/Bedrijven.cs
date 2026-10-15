using ClassLibTeam12.Business.Entities;
using ClassLibTeam12.Data;
using ClassLibTeam12.Data.Framework;

namespace ClassLibTeam12.Business
{
    /// <summary>
    ///   Deze static class biedt methoden voor het beheren van bedrijfsentiteiten.
    /// </summary>
    public static class Bedrijven
    {
        /// <summary>
        ///   Haalt alle bedrijven op.
        /// </summary>
        /// <returns>Een SelectResult-object met de resultaten van het ophalen van de bedrijven.</returns>
        public static SelectResult GetBedrijven()
        {
            BedrijfData bedrijfData = new BedrijfData();
            SelectResult result = bedrijfData.Select();
            return result;
        }

        /// <summary>
        ///   Voegt een nieuw bedrijf toe.
        /// </summary>
        /// <param name="naam">De naam van het nieuwe bedrijf.</param>
        /// <param name="locatie">De locatie van het nieuwe bedrijf.</param>
        /// <returns>Een InsertResult-object met de resultaten van het toevoegen van het bedrijf.</returns>
        public static InsertResult Add(string naam, string locatie)
        {
            Bedrijf bedrijf = new Bedrijf
            {
                Naam = naam,
                Locatie = locatie,
            };

            BedrijfData bedrijfData = new BedrijfData();
            InsertResult result = bedrijfData.Insert(bedrijf);
            return result;
        }
    }
}
