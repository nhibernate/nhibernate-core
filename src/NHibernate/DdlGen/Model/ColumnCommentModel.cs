namespace NHibernate.DdlGen.Model
{
    public class ColumnCommentModel
    {
        public ColumnCommentModel(){}

        public ColumnCommentModel(string columnName, string comment)
        {
            ColumnName = columnName;
            Comment = comment;
        }

        public string Comment { get; set; }

        public string ColumnName { get; set; }
    }
}