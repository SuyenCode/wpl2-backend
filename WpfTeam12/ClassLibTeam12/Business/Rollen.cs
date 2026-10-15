using ClassLibTeam12.Business.Entities;
using ClassLibTeam12.Data;
using ClassLibTeam12.Data.Framework;

namespace ClassLibTeam12.Business
{
    /// <summary>
    ///  Deze statische klasse biedt methoden voor het beheren van gebruikersrollen (Rollen).
    /// </summary>
    public static class Rollen
    {
        /// <summary>
        /// Haalt alle rollen op.
        /// </summary>
        /// <returns>Een SelectResult-object met de resultaten van het ophalen van de rollen.</returns>
        public static SelectResult GetRollen()
        {
            RolData rolData = new RolData();
            SelectResult result = rolData.Select();
            return result;
        }

        /// <summary>
        /// Voegt een nieuwe rol toe.
        /// </summary>
        /// <param name="naam">De naam van de nieuwe rol.</param>
        /// <returns>Een InsertResult-object met de resultaten van het toevoegen van de rol.</returns>
        public static InsertResult Add(string naam)
        {
            Rol rol = new Rol
            {
                Naam = naam
            };

            RolData rolData = new RolData();
            InsertResult result = rolData.Insert(rol);
            return result;
        }
    }
}
