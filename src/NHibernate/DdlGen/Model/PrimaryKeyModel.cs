using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.DdlGen.Model
{
    public class PrimaryKeyModel
    {
        public ICollection<ColumnModel> Columns { get; set; }
        public bool Identity { get; set; }
        public string Name { get; set; }
        public bool Clustered { get; set; }

        public PrimaryKeyModel(){}
        public PrimaryKeyModel(IEnumerable<ColumnModel> columns, bool identity)
        {
            Columns = columns.ToList();
            Identity = identity;
            Name = null;
            Clustered = true;
        }
        public PrimaryKeyModel(IEnumerable<ColumnModel> columns, bool identity, string name, bool clustered)
        {
            Columns = columns.ToList();
            Identity = identity;
            Name = name;
            Clustered = clustered;
        }
    }
}