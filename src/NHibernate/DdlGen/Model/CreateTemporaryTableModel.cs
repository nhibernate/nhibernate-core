using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.DdlGen.Model
{
    public class CreateTemporaryTableModel
    {
        public DbName Name { get; set; }

        public List<ColumnModel> Columns { get; set; }
    }
}

