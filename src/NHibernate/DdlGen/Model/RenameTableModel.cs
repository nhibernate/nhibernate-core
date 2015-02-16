namespace NHibernate.DdlGen.Model
{
  public class RenameTableModel
  {
    public DbName OldTableName { get; set; }
    public DbName NewTableName { get; set; }
  }
}