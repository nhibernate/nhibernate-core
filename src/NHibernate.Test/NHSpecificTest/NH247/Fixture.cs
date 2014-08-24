using NHibernate.Criterion;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH247
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH247"; }
		}

		private void AssertDialect()
		{
			if (!(Dialect is FirebirdDialect))
				Assert.Ignore("This test is specific for FirebirdDialect");
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				s.Delete("from LiteralDescription");
				s.Flush();
			}
		}

		private void FillDB()
		{
			LiteralDescription ld1 = new LiteralDescription("DescriptioN 1");
			LiteralDescription ld2 = new LiteralDescription(" Description 2");
			LiteralDescription ld3 = new LiteralDescription(" Description ");
			LiteralDescription ld4 = new LiteralDescription("1234567890");
			LiteralDescription ld5 = new LiteralDescription("    1234567890    ");
			LiteralDescription ld6 = new LiteralDescription("DescRiptioN TheEnd");
			using (ISession s = OpenSession())
			{
				s.Save(ld1);
				s.Save(ld2);
				s.Save(ld3);
				s.Save(ld4);
				s.Save(ld5);
				s.Save(ld6);
				s.Flush();
			}
		}

		[Test]
		public void CommonLiteralFunctions()
		{
			AssertDialect();
			FillDB();
			using (ISession s = OpenSession())
			{
				IQuery q;
				q = s.CreateQuery("from ld in class LiteralDescription where upper(ld.Description) = 'DESCRIPTION 1'");
				Assert.AreEqual(1, q.List().Count);
				q = s.CreateQuery("from ld in class LiteralDescription where lower(ld.Description) = 'description 1'");
				Assert.AreEqual(1, q.List().Count);
				q = s.CreateQuery("from ld in class LiteralDescription where trim(ld.Description) = 'Description 2'");
				Assert.AreEqual(1, q.List().Count);
				q = s.CreateQuery("from ld in class LiteralDescription where trim(ld.Description) = 'Description'");
				Assert.AreEqual(1, q.List().Count);
				q = s.CreateQuery("from ld in class LiteralDescription where trim(upper(ld.Description)) = 'DESCRIPTION'");
				Assert.AreEqual(1, q.List().Count);
				q = s.CreateQuery("from ld in class LiteralDescription where lower(trim(ld.Description)) = 'description'");
				Assert.AreEqual(1, q.List().Count);
				q = s.CreateQuery("from ld in class LiteralDescription where upper(ld.Description) like 'DESCRIPTION%'");
				Assert.AreEqual(2, q.List().Count);
				q = s.CreateQuery("from ld in class LiteralDescription where lower(ld.Description) like 'description%'");
				Assert.AreEqual(2, q.List().Count);
			}
		}

		[Test]
		public void FirebirdLiteralFunctions()
		{
			AssertDialect();
			FillDB();
			using (ISession s = OpenSession())
			{
				IQuery q;
				q = s.CreateQuery("from ld in class LiteralDescription where char_length(ld.Description) = 10");
				Assert.AreEqual(1, q.List().Count);
				q = s.CreateQuery("from ld in class LiteralDescription where char_length(trim(ld.Description)) = 10");
				Assert.AreEqual(2, q.List().Count);
			}
		}

		[Test]
		public void InsensitiveLikeCriteria()
		{
			FillDB();
			using (ISession s = OpenSession())
			{
				ICriteria c;
				c = s.CreateCriteria(typeof(LiteralDescription));
				c.Add(new InsensitiveLikeExpression("Description", "DeScripTion%"));
				Assert.AreEqual(2, c.List().Count);

				c = s.CreateCriteria(typeof(LiteralDescription));
				c.Add(Expression.InsensitiveLike("Description", "DeScripTion", MatchMode.Start));
				Assert.AreEqual(2, c.List().Count);

				c = s.CreateCriteria(typeof(LiteralDescription));
				c.Add(Expression.InsensitiveLike("Description", "DeScripTion", MatchMode.Anywhere));
				Assert.AreEqual(4, c.List().Count);

				c = s.CreateCriteria(typeof(LiteralDescription));
				c.Add(Expression.InsensitiveLike("Description", "tHeeND", MatchMode.End));
				Assert.AreEqual(1, c.List().Count);

				c = s.CreateCriteria(typeof(LiteralDescription));
				c.Add(Expression.InsensitiveLike("Description", "DescRiptioN TheEnd", MatchMode.Exact));
				Assert.AreEqual(1, c.List().Count);
			}
		}
	}
}