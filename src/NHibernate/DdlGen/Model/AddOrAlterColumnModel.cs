namespace NHibernate.DdlGen.Model
{
    public class AddOrAlterColumnModel
    {
        public DbName Table { get; set; }
        public ColumnModel Column { get; set; }
    }
}