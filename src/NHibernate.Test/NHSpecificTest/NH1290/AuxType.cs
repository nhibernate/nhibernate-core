using NHibernate.Engine;
using NHibernate.Mapping;

namespace NHibernate.Test.NHSpecificTest.NH1290
{
	public class AuxType : AbstractAuxiliaryDatabaseObject
	{
		public override string SqlCreateString(Dialect.Dialect dialect, IMapping p, string defaultCatalog, string defaultSchema)
		{
			return "select 1";
		}

		public override string SqlDropString(Dialect.Dialect dialect, string defaultCatalog, string defaultSchema)
		{
			return "select 1";
		}
	}
}