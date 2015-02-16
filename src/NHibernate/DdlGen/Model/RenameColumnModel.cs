using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.DdlGen.Model
{
    public class RenameColumnModel
    {
        public DbName Table { get; set; }
        public string OldColumnName { get; set; }
        public string NewColumnName { get; set; }
    }
}
