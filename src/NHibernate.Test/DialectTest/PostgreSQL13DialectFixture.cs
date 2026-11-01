using System.Collections.Generic;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class PostgreSQL13DialectFixture
	{
		[Test]
		public void UsesNativeUuidV4Function()
		{
			var dialect = new PostgreSQL13Dialect();

			Assert.That(dialect.SelectGUIDString, Is.EqualTo("select gen_random_uuid()"));
			Assert.That(dialect.Functions["gen_random_uuid"].Render(new List<object>(), null).ToString(), Is.EqualTo("gen_random_uuid()"));
			Assert.That(dialect.Functions["new_uuid"].Render(new List<object>(), null).ToString(), Is.EqualTo("gen_random_uuid()"));
		}
	}
}
