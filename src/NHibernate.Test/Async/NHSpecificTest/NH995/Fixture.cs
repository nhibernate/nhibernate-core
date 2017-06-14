﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH995
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		public override string BugNumber
		{
			get
			{
				return "NH995";
			}
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using(ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from ClassC");
				s.Delete("from ClassB");
				s.Delete("from ClassA");
				tx.Commit();
			}
		}

		[Test]
		public async Task TestAsync()
		{
			int a_id;
			using(ISession s = OpenSession())
			using(ITransaction tx = s.BeginTransaction())
			{
				// Create an A and save it
				ClassA a = new ClassA();
				a.Name = "a1";
				await (s.SaveAsync(a));

				// Create a B and save it
				ClassB b = new ClassB();
				b.Id = new ClassBId("bbb", a);
				b.SomeProp = "Some property";
				await (s.SaveAsync(b));

				// Create a C and save it
				ClassC c = new ClassC();
				c.B = b;
				await (s.SaveAsync(c));

				await (tx.CommitAsync());

				a_id = a.Id;
			}

			// Clear the cache
			Sfi.Evict(typeof(ClassA));
			Sfi.Evict(typeof(ClassB));
			Sfi.Evict(typeof(ClassC));
			
			using(ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				// Load a so we can use it to load b
				ClassA a = await (s.GetAsync<ClassA>(a_id));

				// Load b so b will be in cache
				ClassB b = await (s.GetAsync<ClassB>(new ClassBId("bbb", a)));

				await (tx.CommitAsync());
			}
			
			using(ISession s = OpenSession())
			using(ITransaction tx = s.BeginTransaction())
			{
				using (SqlLogSpy sqlLogSpy = new SqlLogSpy())
				{
					IList<ClassC> c_list = await (s.CreateCriteria(typeof (ClassC)).ListAsync<ClassC>());
					// make sure we initialize B
					await (NHibernateUtil.InitializeAsync(c_list[0].B));

					Assert.AreEqual(1, sqlLogSpy.Appender.GetEvents().Length,
					                "Only one SQL should have been issued");
				}

				await (tx.CommitAsync());
			}
		}
	}
}
