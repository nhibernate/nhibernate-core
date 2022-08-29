using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3874
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.SupportsIdentityColumns;
		}

		object _id;

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var one = new One { Name = "One" };
				var two = new Two { One = one };
				two.One.Twos = new[] { two };
				_id = session.Save(one);
				session.Save(two);

				tx.Commit();
			}
		}

		[Test]
		public void EvictShallNotThrowWhenLoggingIsEnabled()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var one = session.Get<One>(_id);

				session.Evict(one);
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Delete("from Two");
				session.Delete("from One");

				tx.Commit();
			}
		}
	}
}
