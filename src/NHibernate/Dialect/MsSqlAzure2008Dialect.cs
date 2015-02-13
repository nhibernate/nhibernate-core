namespace NHibernate.Dialect
{
	public class MsSqlAzure2008Dialect : MsSql2008Dialect
	{
        public override bool RequiresClusteredPrimaryKey
        {
            get { return true; }
        }
	}}