using System.Collections.Generic;

namespace ClassLibTeam12.Data.Framework
{
    /// <summary>
    /// Representeert het resultaat van een reset-operatie in de database.
    /// Bevat informatie over het aantal beïnvloede rijen en eventuele fouten.
    /// </summary>
    public class ResetResult : BaseResult
    {
        /// <summary>
        /// Geeft het aantal rijen aan dat is beïnvloed door de operatie.
        /// </summary>
        public int RowsAffected { get; set; }

        /// <summary>
        /// Lijst van foutmeldingen die zijn opgetreden tijdens de operatie.
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// Voegt een foutmelding toe aan de lijst van fouten.
        /// </summary>
        /// <param name="error">De foutmelding die moet worden toegevoegd.</param>
        public void AddError(string error)
        {
            Errors.Add(error);
        }
    }
}
