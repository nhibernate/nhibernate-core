using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.DdlGen.Model
{
    public class CreateTableModel
    {
        public DbName Name { get; set; }
        public string Comment { get; set; }
        public IList<ColumnModel>  Columns { get; set; }
        public PrimaryKeyModel PrimaryKey { get; set; }
        public ICollection<ForeignKeyModel> ForeignKeys { get; set; }
        public ICollection<IndexModel> UniqueIndexes { get; set; }
        public ICollection<TableCheckModel> Checks { get; set; }
    }
}
