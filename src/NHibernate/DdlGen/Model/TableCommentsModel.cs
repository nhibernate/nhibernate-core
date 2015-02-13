using System.Collections.Generic;

namespace NHibernate.DdlGen.Model
{
    public class TableCommentsModel
    {
        public List<ColumnCommentModel> Columns { get; set; }

        public DbName TableName { get; set; }

        public string Comment { get; set; }
    }
}

