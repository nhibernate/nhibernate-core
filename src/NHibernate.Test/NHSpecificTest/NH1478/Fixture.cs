using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1478
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
					Person e1 = new Person("Tuna Toksoz","Born in Istanbul :Turkey");
					Person e2 = new Person("Tuna Toksoz", "Born in Istanbul :Turkiye");
					s.Save(e1);
					s.Save(e2);
					tx.Commit();
				}
			}
		}

		[Test]
		public void TestIfColonInStringIsNotInterpretedAsParameterInSQL()
		{
			using (ISession session=OpenSession())
			{


				IList lst = session.CreateSQLQuery("select Biography from Person where Biography='Born in Istanbul :Turkey'")
					.AddScalar("Biography", NHibernateUtil.String).List();
				Assert.AreEqual(1,lst.Count);
			}

		}

		[Test]
		public void TestIfColonInStringIsNotInterpretedAsParameterInHQL()
		{
			using (ISession session = OpenSession())
			{


				IList lst = session.CreateSQLQuery("select p.Biography from Person p where p.Biography='Born in Istanbul :Turkey'")
					.List();
				Assert.AreEqual(1, lst.Count);
			}

		}

	}
}
