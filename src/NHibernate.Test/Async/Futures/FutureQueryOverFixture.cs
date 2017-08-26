﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Futures
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FutureQueryOverFixtureAsync : FutureFixture
	{

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(new Person());
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				tx.Commit();
			}
		}

		[Test]
		public async Task CanCombineSingleFutureValueWithEnumerableFuturesAsync()
		{
			using (var s = Sfi.OpenSession())
			{
				IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();

				var persons = s.QueryOver<Person>()
					.Take(10)
					.Future();

				var personIds = s.QueryOver<Person>()
					.Select(p => p.Id)
					.FutureValue<int>();

				var singlePerson = s.QueryOver<Person>()
					.FutureValue();

				using (var logSpy = new SqlLogSpy())
				{
					Person singlePersonValue = await (singlePerson.GetValueAsync());
					int personId = await (personIds.GetValueAsync());

					foreach (var person in persons)
					{

					}

					var events = logSpy.Appender.GetEvents();
					Assert.AreEqual(1, events.Length);

					Assert.That(singlePersonValue, Is.Not.Null);
					Assert.That(personId, Is.Not.EqualTo(0));
				}
			}
		}
	}
}
