using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NHibernate.Engine.Query.Sql;

namespace NHibernate.Test.EngineTest
{
	[TestFixture]
	public class NativeSqlQueryReturnTest
	{
		[Test]
		public void AllEmbeddedTypesAreMarkedSerializable()
		{
			NHAssert.InheritedAreMarkedSerializable(typeof(INativeSQLQueryReturn));
		}
	}
}
