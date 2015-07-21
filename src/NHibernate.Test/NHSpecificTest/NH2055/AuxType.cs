using System.Collections.Generic;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Mapping;

namespace NHibernate.Test.NHSpecificTest.NH2055
{
	public class AuxType : AbstractAuxiliaryDatabaseObject
	{

		override public string SqlCreateString(Dialect.Dialect dialect, IMapping p, string defaultCatalog, string defaultSchema)
		{
			return "select '" + Parameters["scriptParameter"] + "'";
		}

		override public string SqlDropString(Dialect.Dialect dialect, string defaultCatalog, string defaultSchema)
		{
			return "select 'drop script'";
		}

	}
}