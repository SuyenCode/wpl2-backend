using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibTeam12.Data.Framework
{
    public class DeleteResult
    {
        public bool Succeeded { get; set; }
        public int RowsAffected { get; set; }
        public object AddError { get; set; }
    }
}
