﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;
using System;
using NHibernate.Dialect;

namespace NHibernate.Test.NHSpecificTest.NH3377
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
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
		public async Task ShouldBeAbleToCallConvertToInt32FromStringParameterAsync()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = await ((from e in session.Query<Entity>()
							  where e.Name == "Bob"
							  select Convert.ToInt32(e.Age)).ToListAsync());

				Assert.That(result, Has.Count.EqualTo(1));
				Assert.That(result[0], Is.EqualTo(17));
			}
		}

		[Test]
		public async Task ShouldBeAbleToCallConvertToInt32FromStringParameterInMaxAsync()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = await (session.Query<Entity>().MaxAsync(e => Convert.ToInt32(e.Age)));

				Assert.That(result, Is.EqualTo(17));
			}
		}

		[Test]
		public async Task ShouldBeAbleToCallConvertToBooleanFromStringParameterAsync()
		{
			if (Dialect is SQLiteDialect || Dialect is FirebirdDialect || Dialect is MySQLDialect ||
				Dialect is Oracle8iDialect || Dialect is SapSQLAnywhere17Dialect)
				Assert.Ignore(Dialect.GetType() + " is not supported");

			//NH-3720
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = await (session.Query<Entity>().Where(x => x.Name == "true").Select(x => Convert.ToBoolean(x.Name)).SingleAsync());

				Assert.That(result, Is.True);
			}
		}

		[Test]
		public async Task ShouldBeAbleToCallConvertToDateTimeFromStringParameterAsync()
		{
			if (Dialect is Oracle8iDialect)
				Assert.Ignore(Dialect.GetType() + " is not supported");

			//NH-3720
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = await (session.Query<Entity>().Where(x => x.Name == "2014-10-13").Select(x => Convert.ToDateTime(x.Name)).SingleAsync());

				Assert.That(result, Is.EqualTo(new DateTime(2014, 10, 13)));
			}
		}
	}
}
