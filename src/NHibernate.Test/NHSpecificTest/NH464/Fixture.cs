using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH464
{
	/// <summary>
	/// This is a test class for composite-element with reflection optimizer
	/// </summary>
	[TestFixture]
	public class Fixture : BugTestCase
	{
		/// <summary>
		/// Mapping files used in the TestCase
		/// </summary>
		protected override string[] Mappings => new[] {"Promotion.hbm.xml"};

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				session.Delete("from System.Object"); // clear everything from database
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				session.Delete("from System.Object"); // clear everything from database
				t.Commit();
			}
			base.OnTearDown();
		}

		[Test]
		public void CompositeElement()
		{
			Promotion promo = new Promotion();
			promo.Description = "test promo";
			promo.Window = new PromotionWindow();
			promo.Window.Dates.Add(new DateRange(DateTime.Today, DateTime.Today.AddDays(20)));

			int id = 0;
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				id = (int) session.Save(promo);
				tx.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				promo = (Promotion) session.Load(typeof(Promotion), id);

				Assert.AreEqual(1, promo.Window.Dates.Count);
				Assert.AreEqual(DateTime.Today, ((DateRange) promo.Window.Dates[0]).Start);
				Assert.AreEqual(DateTime.Today.AddDays(20), ((DateRange) promo.Window.Dates[0]).End);

				tx.Commit();
			}
		}
	}
}