using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1492
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void RetrieveEntities()
		{
			Entity eDel = new Entity(1, "DeletedEntity");
			eDel.Deleted = "Y";

			Entity eGood = new Entity(2, "GoodEntity");
			eGood.Childs.Add(new ChildEntity(eGood, "GoodEntityChild"));

			// Make "Deleted" entity persistent
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(eDel);
				s.Save(eGood);
				t.Commit();
			}

			// Retrive (check if the entity was well persisted)
			IList<ChildEntity> childs;
			using (ISession s = OpenSession())
			{
				s.EnableFilter("excludeDeletedRows").SetParameter("deleted", "Y");

				IQuery q = s.CreateQuery("FROM ChildEntity c WHERE c.Parent.Code = :parentCode").SetParameter("parentCode", 2);
				childs=	q.List<ChildEntity>();
			}
			Assert.AreEqual(1, childs.Count);

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete("from Entity");
				t.Commit();
			}
		}
	}
}