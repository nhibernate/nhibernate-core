using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.DdlGen.Model
{
    public class TableCommentsModel
    {
        public List<ColumnCommentModel> Columns { get; set; }

        public DbName TableName { get; set; }

        public string Comment { get; set; }
    }
}
