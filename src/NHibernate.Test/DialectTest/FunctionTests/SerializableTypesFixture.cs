using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NHibernate.Dialect.Function;
using System.Reflection;

namespace NHibernate.Test.DialectTest.FunctionTests
{
	[TestFixture]
	public class SerializableTypesFixture
	{
		[Test]
		public void AllEmbeddedTypesAreMarkedSerializable()
		{
			NHAssert.InheritedAreMarkedSerializable(typeof(ISQLFunction));
		}
	}
}
