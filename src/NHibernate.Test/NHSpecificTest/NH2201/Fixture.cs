using System;
using System.Collections.Generic;
using NHibernate.Multi;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2201
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory)
		{
			return factory.ConnectionProvider.Driver.SupportsMultipleQueries;
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from System.Object");
				tx.Commit();
			}
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Save(new Parent());
				s.Save(new SubClass() { Name = "test" });

				tx.Commit();
			}
		}

		[Test, Obsolete]
		public void CanUseMutliCriteriaAndFetchSelect()
		{
			using (var s = OpenSession())
			{
				Console.WriteLine("*** start");
				var results =
					s.CreateMultiCriteria()
						.Add<Parent>(s.CreateCriteria<Parent>())
						.Add<Parent>(s.CreateCriteria<Parent>())
						.List();

				var result1 = (IList<Parent>) results[0];
				var result2 = (IList<Parent>) results[1];

				Assert.That(result1.Count, Is.EqualTo(2));
				Assert.That(result2.Count, Is.EqualTo(2));
				Console.WriteLine("*** end");
			}
		}

		[Test]
		public void CanUseQueryBatchAndFetchSelect()
		{
			using (var s = OpenSession())
			{
				var multi =
					s.CreateQueryBatch()
					 .Add<Parent>(s.CreateCriteria<Parent>())
					 .Add<Parent>(s.CreateCriteria<Parent>());

				var result1 = multi.GetResult<Parent>(0);
				var result2 = multi.GetResult<Parent>(1);

				Assert.That(result1.Count, Is.EqualTo(2));
				Assert.That(result2.Count, Is.EqualTo(2));
			}
		}
	}
}
