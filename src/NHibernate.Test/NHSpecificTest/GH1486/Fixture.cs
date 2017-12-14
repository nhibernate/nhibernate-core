using System.Linq;
using NHibernate.Engine;
using NHibernate.Linq;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1486
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Company
				{
					Name = "Company A",
					Contact = new Contact
					{
						Phone = "1112223333",
						Email = "email1@mail.com",
						IsActive = true
					}
				};
				session.Save(e1);

				var e2 = new Company
				{
					Name = "Company B",
					Contact = new Contact
					{
						Phone = "2223334444",
						Email = "email2@mail.com",
						IsActive = false
					}
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
		public void TestIsModified()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var company = session.Query<Company>().FirstOrDefault();
				Assert.IsNotNull(company);
				var sessionImplementor = session as ISessionImplementor;

				var metaData = session.SessionFactory.GetClassMetadata(typeof(Company));
				foreach (var propertyType in metaData.PropertyTypes)
				{
					var componentType = propertyType as ComponentType;
					if (componentType != null && componentType.ReturnedClass.Name == "Contact")
					{
						bool[] checkable = new bool[3] {true,true,true };

						var isModified = componentType.IsModified(company.Contact, company.Contact, checkable, sessionImplementor);
						Assert.IsFalse(isModified);

					}

				}
			}


		}

		

	}
}
