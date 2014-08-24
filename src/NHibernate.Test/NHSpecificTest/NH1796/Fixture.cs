using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1796
{
	[TestFixture]
	public class Fixture: BugTestCase
	{
		[Test]
		public void Merge()
		{
			var entity = new Entity { Name = "Vinnie Luther" };
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(entity);
				t.Commit();
			}

			entity.DynProps = new Dictionary<string, object>();
			entity.DynProps["StrProp"] = "Modified";
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Merge(entity);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateQuery("delete from Entity").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void SaveOrUpdate()
		{
			var entity = new Entity { Name = "Vinnie Luther" };
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.SaveOrUpdate(entity);
				t.Commit();
			}

			entity.DynProps = new Dictionary<string, object>();
			entity.DynProps["StrProp"] = "Modified";
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.SaveOrUpdate(entity);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateQuery("delete from Entity").ExecuteUpdate();
				t.Commit();
			}
		}
	}
}