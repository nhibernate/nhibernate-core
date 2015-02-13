using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.DdlGen.Model
{
    public class DropColumnModel
    {
        public DbName Table { get; set; }
        public string Column { get; set; }
    }
}
