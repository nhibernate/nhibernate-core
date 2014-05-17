using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;
using System;

namespace NHibernate.Test.NHSpecificTest.NH3377
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity
					         {
						         Name = "Bob",
								 Age = "17",
								 Solde = "5.4"
					         };
				session.Save(e1);

				var e2 = new Entity
					         {
						         Name = "Sally", 
								 Age = "16"
					         };
				session.Save(e2);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void ShouldBeAbleToCallConvertToInt32FromStringParameter()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = from e in session.Query<Entity>()
							 where e.Name == "Bob"
							 select Convert.ToInt32(e.Age);

				Assert.AreEqual(1, result.Count());
				Assert.AreEqual(17, result.First());
			}
		}

		[Test]
		public void ShouldBeAbleToCallConvertToInt32FromStringParameterInMax()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>().Max(e => Convert.ToInt32(e.Age));

				Assert.AreEqual(17, result);
			}
		}
	}
}