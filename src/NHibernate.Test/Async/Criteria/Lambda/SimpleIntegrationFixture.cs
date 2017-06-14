﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Criteria.Lambda
{
	using System.Threading.Tasks;
	[TestFixture]
	public class SimpleIntegrationFixtureAsync : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "Criteria.Lambda.Mappings.hbm.xml" }; }
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(new Person { Name = "test person 1", Age = 20 });
				s.Save(new Person { Name = "test person 2", Age = 30 });
				s.Save(new Person { Name = "test person 3", Age = 40 });

				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from Child").ExecuteUpdate();
				s.CreateQuery("update Person p set p.Father = null").ExecuteUpdate();
				s.CreateQuery("delete from Person").ExecuteUpdate();
				s.CreateQuery("delete from JoinedChild").ExecuteUpdate();
				s.CreateQuery("delete from Parent").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public async Task TestQueryOverAsync()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				Person personAlias = null;

				var actual =
					await (s.QueryOver<Person>(() => personAlias)
						.Where(() => personAlias.Name == "test person 2")
						.And(() => personAlias.Age == 30)
						.ListAsync());

				Assert.That(actual.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public async Task TestQueryOverAliasAsync()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				Person personAlias = null;

				var actual =
					await (s.QueryOver<Person>(() => personAlias)
						.Where(() => personAlias.Name == "test person 2")
						.And(() => personAlias.Age == 30)
						.ListAsync());

				Assert.That(actual.Count, Is.EqualTo(1));
			}
		}
	}
}
