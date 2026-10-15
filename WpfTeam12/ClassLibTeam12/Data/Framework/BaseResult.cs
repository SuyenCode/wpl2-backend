using System.Collections.Generic;

namespace ClassLibTeam12.Data.Framework
{
    public abstract class BaseResult
    {
        private List<string> errors = new List<string>();
        public bool Succeeded { get; set; }
        public void AddError(string error)
        {
            errors.Add(error);
        }
    }



}
