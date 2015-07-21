using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;
using System;
using NHibernate.Dialect;

namespace NHibernate.Test.NHSpecificTest.NH3377
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity
				{
					Name = "Bob",
					Age = "17",
					Solde = "5.4"
				};
				session.Save(e1);

				var e2 = new Entity
				{
					Name = "Sally",
					Age = "16"
				};
				session.Save(e2);

				var e3 = new Entity
				{
					Name = "true",
					Age = "10"
				};
				session.Save(e3);

				var e4 = new Entity
				{
					Name = "2014-10-13",
					Age = "11"
				};
				session.Save(e4);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void ShouldBeAbleToCallConvertToInt32FromStringParameter()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = (from e in session.Query<Entity>()
							  where e.Name == "Bob"
							  select Convert.ToInt32(e.Age)).ToList();

				Assert.That(result, Has.Count.EqualTo(1));
				Assert.That(result[0], Is.EqualTo(17));
			}
		}

		[Test]
		public void ShouldBeAbleToCallConvertToInt32FromStringParameterInMax()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>().Max(e => Convert.ToInt32(e.Age));

				Assert.That(result, Is.EqualTo(17));
			}
		}

		[Test]
		public void ShouldBeAbleToCallConvertToBooleanFromStringParameter()
		{
			if (Dialect is SQLiteDialect || Dialect is FirebirdDialect || Dialect is MySQLDialect || Dialect is Oracle8iDialect)
				Assert.Ignore(Dialect.GetType() + " is not supported");

			//NH-3720
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>().Where(x => x.Name == "true").Select(x => Convert.ToBoolean(x.Name)).Single();

				Assert.That(result, Is.True);
			}
		}

		[Test]
		public void ShouldBeAbleToCallConvertToDateTimeFromStringParameter()
		{
			if (Dialect is Oracle8iDialect)
				Assert.Ignore(Dialect.GetType() + " is not supported");

			//NH-3720
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>().Where(x => x.Name == "2014-10-13").Select(x => Convert.ToDateTime(x.Name)).Single();

				Assert.That(result, Is.EqualTo(new DateTime(2014, 10, 13)));
			}
		}
	}
}