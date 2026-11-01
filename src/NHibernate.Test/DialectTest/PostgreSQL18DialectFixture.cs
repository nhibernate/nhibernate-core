using System.Collections.Generic;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class PostgreSQL18DialectFixture
	{
		[Test]
		public void ExposesOptInUuidV7Function()
		{
			var dialect = new PostgreSQL18Dialect();

			Assert.That(dialect.SelectGUIDString, Is.EqualTo("select gen_random_uuid()"));
			Assert.That(dialect.Functions["new_uuid"].Render(new List<object>(), null).ToString(), Is.EqualTo("gen_random_uuid()"));
			Assert.That(dialect.Functions["uuidv7"].Render(new List<object> { "interval '1 second'" }, null).ToString(), Is.EqualTo("uuidv7(interval '1 second')"));
			Assert.That(dialect.Functions["new_uuid_v7"].Render(new List<object>(), null).ToString(), Is.EqualTo("uuidv7()"));
		}
	}
}
