using System.Collections;
using NHibernate.Criterion;
using NHibernate.Dialect.Function;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1393
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Person");
					tx.Commit();
				}
			}
		}

		protected override void OnSetUp()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					Person e1 = new Person("Joe", 10, 9);
					Person e2 = new Person("Sally", 100, 8);
					Person e3 = new Person("Tim", 20, 7); //20
					Person e4 = new Person("Fred", 40, 40);
					Person e5 = new Person("Mike", 50, 50);
					s.Save(e1);
					s.Save(e2);
					s.Save(e3);
					s.Save(e4);
					s.Save(e5);
					tx.Commit();
				}
			}
		}

		[Test]
		public void CanSumProjectionOnSqlFunction()
		{
			using (ISession s = OpenSession())
			{
				ISQLFunction arithmaticAddition = new VarArgsSQLFunction("(", "+", ")");
				ICriteria c =
					s.CreateCriteria(typeof (Person)).SetProjection(
						Projections.Sum(Projections.SqlFunction(arithmaticAddition, NHibernateUtil.GuessType(typeof (double)),
						                                        Projections.Property("IQ"), Projections.Property("ShoeSize"))));
				IList list = c.List();
				Assert.AreEqual(334, list[0]);
			}
		}

		[Test]
		public void CanAvgProjectionOnSqlFunction()
		{
			using (ISession s = OpenSession())
			{
				ISQLFunction arithmaticAddition = new VarArgsSQLFunction("(", "+", ")");
				ICriteria c =
					s.CreateCriteria(typeof (Person)).SetProjection(
						Projections.Avg(Projections.SqlFunction(arithmaticAddition, NHibernateUtil.GuessType(typeof (double)),
						                                        Projections.Property("IQ"), Projections.Property("ShoeSize"))));
				IList list = c.List();
				Assert.AreEqual(((double) 334) / 5, list[0]);
			}
		}

		[Test]
		public void CanMinProjectionOnIdentityProjection()
		{
			using (ISession s = OpenSession())
			{
				ISQLFunction arithmaticAddition = new VarArgsSQLFunction("(", "+", ")");
				ICriteria c =
					s.CreateCriteria(typeof (Person)).SetProjection(
						Projections.Min(Projections.SqlFunction(arithmaticAddition, NHibernateUtil.GuessType(typeof (double)),
						                                        Projections.Property("IQ"), Projections.Property("ShoeSize"))));
				IList list = c.List();
				Assert.AreEqual(19, list[0]);
			}
		}

		[Test]
		public void CanMaxProjectionOnIdentityProjection()
		{
			using (ISession s = OpenSession())
			{
				ISQLFunction arithmaticAddition = new VarArgsSQLFunction("(", "+", ")");
				ICriteria c =
					s.CreateCriteria(typeof (Person)).SetProjection(
						Projections.Max(Projections.SqlFunction(arithmaticAddition, NHibernateUtil.GuessType(typeof (double)),
						                                        Projections.Property("IQ"), Projections.Property("ShoeSize"))));
				IList list = c.List();
				Assert.AreEqual(108, list[0]);
			}
		}
	}
}