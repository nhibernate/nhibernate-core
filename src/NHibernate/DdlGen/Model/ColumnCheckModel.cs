namespace NHibernate.DdlGen.Model
{
    public class ColumnCheckModel
    {
        public string Expression { get; set; }
        public DbName TableName { get; set; }
        
        public string ColumnName { get; set; }
        
    }
}