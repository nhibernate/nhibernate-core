﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1783
{
	using System.Threading.Tasks;
	[TestFixture]
	public class SampleTestAsync : BugTestCase
	{
		[Test]
		public async Task DatePropertyShouldBeStoredWithoutTimePartAsync()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var entity = new DomainClass {Id = 1, BirthDate = new DateTime(1950, 2, 13, 3, 12, 10)};
				await (session.SaveAsync(entity));
				await (tx.CommitAsync());
			}

			using (ISession session = OpenSession())
			{
				// upload the result using DateTime type to verify it does not have the time-part.
				var l = await (session.CreateSQLQuery("SELECT BirthDate AS bd FROM DomainClass")
					.AddScalar("bd",NHibernateUtil.DateTime).ListAsync());
				var actual = (DateTime) l[0];
				var expected = new DateTime(1950, 2, 13);
				Assert.That(actual, Is.EqualTo(expected));
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				await (session.CreateQuery("delete from DomainClass").ExecuteUpdateAsync());
				await (tx.CommitAsync());
			}
		}
	}
}