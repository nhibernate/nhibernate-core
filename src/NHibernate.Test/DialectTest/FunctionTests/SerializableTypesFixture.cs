using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NHibernate.Dialect.Function;
using NUnit.Framework;

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
