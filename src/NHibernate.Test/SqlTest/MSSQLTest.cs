using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.SqlTest
{
	[TestFixture]
	public class MSSQLTest : HandSQLTest
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] {"SqlTest.MSSQLEmployment.hbm.xml"}; }
		}
	}
}
