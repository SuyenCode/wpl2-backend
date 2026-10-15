using System.Collections.Generic;

namespace ClassLibTeam12.Data.Framework
{
    public class UpdateResult : BaseResult
    {
        public int RowsAffected { get; set; }
        public bool Succeeded { get; internal set; }
       
    }
}