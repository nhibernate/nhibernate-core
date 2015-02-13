using NHibernate.DdlGen.Operations;
using NHibernate.Engine;
using NHibernate.Mapping;

namespace NHibernate.Test.NHSpecificTest.NH1290
{
	public class AuxType : AbstractAuxiliaryDatabaseObject
	{
		public override IDdlOperation GetCreateOperation(Dialect.Dialect dialect, IMapping p, string defaultCatalog, string defaultSchema)
		{
			return  new SqlDdlOperation("select 1");
		}

        public override IDdlOperation GetDropOperation(Dialect.Dialect dialect, IMapping p, string defaultCatalog, string defaultSchema)
		{
			return new SqlDdlOperation("select 1");
		}
	}
}