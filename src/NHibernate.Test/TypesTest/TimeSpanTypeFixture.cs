using System;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for TimeSpanTypeFixture.
	/// </summary>
	[TestFixture]
	public class TimeSpanTypeFixture
	{
		[Test]
		public void Next()
		{
			var type = (TimeSpanType) NHibernateUtil.TimeSpan;
			object current = new TimeSpan(DateTime.Now.Ticks - 5);
			object next = type.Next(current, null);

			Assert.IsTrue(next is TimeSpan, "Next should be TimeSpan");
			Assert.IsTrue((TimeSpan) next > (TimeSpan) current,
			              "next should be greater than current (could be equal depending on how quickly this occurs)");
		}

		[Test]
		public void Seed()
		{
			var type = (TimeSpanType) NHibernateUtil.TimeSpan;
			Assert.IsTrue(type.Seed(null) is TimeSpan, "seed should be TimeSpan");
		}
	}

	[TestFixture]
	public class TimeSpanTypeFixture2 : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "TimeSpan"; }
		}

		[Test]
		public void SavingAndRetrieving()
		{
			var ticks = new TimeSpan(1982);

			var entity = new TimeSpanClass
			             	{
			             		TimeSpanValue = ticks
			             	};

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(entity);
				tx.Commit();
			}

			TimeSpanClass entityReturned;

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				entityReturned = s.CreateQuery("from TimeSpanClass").UniqueResult<TimeSpanClass>();
				Assert.AreEqual(ticks, entityReturned.TimeSpanValue);
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete(entityReturned);
				tx.Commit();
			}
		}
	}
}