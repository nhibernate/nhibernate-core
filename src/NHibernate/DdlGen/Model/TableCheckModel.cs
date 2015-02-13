namespace NHibernate.DdlGen.Model
{
    public class TableCheckModel
    {
        public string Name { get; set; }
        public string Expression { get; set; }
        public DbName TableName { get; set; }
    }
}