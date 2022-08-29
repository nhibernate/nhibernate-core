using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Engine.Query.Sql;
using NUnit.Framework;

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
