using System;
using System.Collections;
using NUnit.Framework;
using NHibernate.Multi;

namespace NHibernate.Test.NHSpecificTest.DataReaderWrapperTest
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private const int id = 1333;

		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory)
		{
			return factory.ConnectionProvider.Driver.SupportsMultipleQueries;
		}

		protected override void OnSetUp()
		{
			var ent = new TheEntity { TheValue = "Hola", Id = id };
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Save(ent);
				tx.Commit();
			}
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

		[Test, Obsolete]
		public void CanUseDatareadersGetValue()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var crit = s.CreateCriteria(typeof (TheEntity));
				var multi = s.CreateMultiCriteria();
				multi.Add(crit);
				var res = (IList) multi.List()[0];
				Assert.That(res.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void CanUseDatareadersGetValueWithQueryBatch()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var crit = s.CreateCriteria(typeof (TheEntity));
				var multi = s.CreateQueryBatch();
				multi.Add<TheEntity>(crit);
				var res = multi.GetResult<TheEntity>(0);
				Assert.That(res.Count, Is.EqualTo(1));
			}
		}
	}
}
