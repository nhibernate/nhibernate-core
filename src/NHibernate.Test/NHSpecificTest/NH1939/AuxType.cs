using System.Collections.Generic;
using NHibernate;
using NHibernate.DdlGen.Operations;
using NHibernate.Engine;
using NHibernate.Mapping;

namespace NHibernate.Test.NHSpecificTest.NH1939
{
	public class AuxType : AbstractAuxiliaryDatabaseObject
	{

		public override IDdlOperation GetCreateOperation(Dialect.Dialect dialect, IMapping p, string defaultCatalog, string defaultSchema)
		{
			return new SqlDdlOperation("select '" + Parameters["scriptParameter"] + "'");
		}

        public override IDdlOperation GetDropOperation(Dialect.Dialect dialect, IMapping p, string defaultCatalog, string defaultSchema)
		{
			return new SqlDdlOperation("select 'drop script'");
		}

	}
}
