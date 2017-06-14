﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Linq;
using NHibernate.Dialect;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2441
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return ((dialect is Dialect.SQLiteDialect) || (dialect is Dialect.MsSql2008Dialect));
		}
		
		protected override void OnSetUp()
		{
			base.OnSetUp();
			
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person e1 = new Person("Tuna Toksoz", "Born in Istanbul :Turkey");
				Person e2 = new Person("Tuna Toksoz", "Born in Istanbul :Turkiye");
				s.Save(e1);
				s.Save(e2);
				tx.Commit();
			}
		}
		
		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete("from Person");
				tx.Commit();
			}
			
			base.OnTearDown();
		}

		[Test]
		public async Task LinqQueryBooleanSQLiteAsync()
		{
			using (ISession session = OpenSession())
			{
				var query1 = session.Query<Person>().Where(p => true);
				var query2 = session.Query<Person>().Where(p => p.Id != null);
				var query3 = session.Query<Person>();
				
				Assert.That(await (query1.CountAsync()), Is.EqualTo(await (query2.CountAsync())));
				Assert.That(await (query3.CountAsync()), Is.EqualTo(await (query1.CountAsync())));
			}
		}
	}
}
