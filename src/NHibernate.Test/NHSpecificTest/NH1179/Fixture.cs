using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1179
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1179"; }
		}

		[Test]
		public void ApplyFilterExplicitJoin()
		{
			RelatedClass rc1 = new RelatedClass(1);
			RelatedClass rc2 = new RelatedClass(2);
			RelatedClass rc11 = new RelatedClass(1);
			RelatedClass rc12 = new RelatedClass(2);
			RelatedClass rc13 = new RelatedClass(2);

			MainClass[] mc =
				new MainClass[]
					{
						new MainClass("d0", rc1), new MainClass("d0", rc2), new MainClass("d1", rc11), new MainClass("d1", rc12),
						new MainClass("d1", rc13)
					};

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				foreach (MainClass mainClass in mc)
					s.Save(mainClass);

				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				s.EnableFilter("RelatedClass_Valued").SetParameter("forValue", 2);
				IQuery q =
					s.CreateQuery("select mc.Description, count(mc.Id) from MainClass mc join mc.Related r group by mc.Description order by mc.Description");

				IList l = q.List();
				Assert.AreEqual(2, l.Count);
				Assert.AreEqual(1, (l[0] as IList)[1]);
				Assert.AreEqual(2, (l[1] as IList)[1]);

				s.DisableFilter("RelatedClass_Valued");
				l = q.List();
				Assert.AreEqual(2, l.Count);
				Assert.AreEqual(2, (l[0] as IList)[1]);
				Assert.AreEqual(3, (l[1] as IList)[1]);
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from MainClass");
				s.Delete("from RelatedClass");
				tx.Commit();
			}

		}
	}
}
