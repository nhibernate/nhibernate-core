using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Dialect;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace NHibernate.Test.NHSpecificTest.NH1552
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return dialect is MsSql2005Dialect;
		}
		protected override void OnSetUp()
		{
			using (var session = this.OpenSession())
			{
				using (var tran = session.BeginTransaction())
				{
					MyClass newServ = new MyClass();
					newServ.Name = "tuna";
					session.Save(newServ);
					tran.Commit();
				}
			}
		}
		protected override void OnTearDown()
		{
			using (var session = this.OpenSession())
			{
				using (var tran = session.BeginTransaction())
				{
					session.Delete("from MyClass");
					tran.Commit();
				}
			}
		}

		[Test]
		public void Paging_with_sql_works_as_expected()
		{
			using (var session = this.OpenSession())
			{
				using (var tran = session.BeginTransaction())
				{
					string sql = "select * from MyClass";
					IList list = session.CreateSQLQuery(sql)
						.AddEntity(typeof(MyClass))
						.SetFirstResult(0)
						.SetMaxResults(50)
						.List();
					Assert.That(list.Count, Is.EqualTo(1));
				}
			}
		}
	}
}