using System;
using System.Collections;
using NHibernate.DomainModel;
using NHibernate.Linq;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test
{
	/// <summary>
	/// Tests for the common query interface.
	/// </summary>
	[TestFixture]
	public class CommonInterfaceTest : TestCase
	{
		private class DebugResultTransformer : IResultTransformer
		{
			public bool TransformTupleCalled { get; set; }
			public bool TransformListCalled { get; set; }

			#region IResultTransformer Members

			public object TransformTuple(object[] tuple, string[] aliases)
			{
				this.TransformTupleCalled = true;
				return tuple[0];
			}

			public IList TransformList(IList collection)
			{
				this.TransformListCalled = true;
				return collection;
			}

			#endregion
		}

		protected override IList Mappings
		{
			get { return new string[] {"Simple.hbm.xml"}; }
		}

		private void TestCommonInterface(ISession session, IQueryOptions query, string alias)
		{
			session.Save(new Simple { Name = "A Name", Address = "A Street", Date = DateTime.Today, Count = 1, Pay = 100f }, 1);
			session.Save(new Simple { Name = "Another Name", Address = "Another Street", Date = DateTime.Today.AddDays(1), Count = 2, Pay = 200f }, 2);
			session.Flush();
			session.Clear();

			Assert.IsNotNull(query);

			var transformer = new DebugResultTransformer();

			query = query.SetReadOnly(true);
			query = query.SetFirstResult(1);
			query = query.SetMaxResults(1);
			query = query.SetFetchSize(100);
			//query = query.SetLockMode(alias, LockMode.Upgrade);
			query = query.SetResultTransformer(transformer);

			var result = query.List<Simple>();

			Assert.AreEqual(2, session.GetIdentifier(result[0]));
			Assert.IsTrue(transformer.TransformListCalled);
			Assert.IsTrue(transformer.TransformTupleCalled);

			Assert.AreEqual(1, result.Count);
			//Assert.AreEqual(LockMode.Upgrade, session.GetCurrentLockMode(result[0]));
			Assert.IsTrue(session.IsReadOnly(result[0]));
		}

		[Test]
		public void TestCommonInterfaceOnLinq()
		{
			//NH-2140
			using (var session = this.OpenSession())
			using (session.BeginTransaction())
			{
				var linqQuery = session.Query<Simple>();
				var common = linqQuery as IQueryOptions;

				TestCommonInterface(session, common, null);
			}
		}

		[Test]
		public void TestCommonInterfaceOnHql()
		{
			//NH-2140
			using (var session = this.OpenSession())
			using (session.BeginTransaction())
			{
				var hqlQuery = session.CreateQuery("from Simple s");
				var common = hqlQuery as IQueryOptions;

				TestCommonInterface(session, common, "s");
			}
		}

		[Test]
		public void TestCommonInterfaceOnSql()
		{
			//NH-2140
			using (var session = this.OpenSession())
			using (session.BeginTransaction())
			{
				var sqlQuery = session.CreateSQLQuery("SELECT {s.*} FROM Simple {s}").AddEntity("s", typeof(Simple));
				var common = sqlQuery as IQueryOptions;

				TestCommonInterface(session, common, "s");
			}
		}

		[Test]
		public void TestCommonInterfaceOnCriteria()
		{
			//NH-2140
			using (var session = this.OpenSession())
			using (session.BeginTransaction())
			{
				var criteriaQuery = session.CreateCriteria<Simple>();
				var common = criteriaQuery as IQueryOptions;

				TestCommonInterface(session, common, "_this");
			}
		}

		[Test]
		public void TestCommonInterfaceOnQueryOver()
		{
			//NH-2140
			using (var session = this.OpenSession())
			using (session.BeginTransaction())
			{
				var criteriaQuery = session.QueryOver<Simple>();
				var common = criteriaQuery as IQueryOptions;

				TestCommonInterface(session, common, "_this");
			}
		}
	}
}