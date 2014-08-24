using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.UserCollection
{
	[TestFixture]
	public class UserCollectionTypeTest : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] {"UserCollection.UserPermissions.hbm.xml"}; }
		}


		[Test]
		public void BasicOperation()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			User u = new User("max");
			u.EmailAddresses.Add(new Email("max@hibernate.org"));
			u.EmailAddresses.Add(new Email("max.andersen@jboss.com"));
			s.Save(u);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			User u2 = (User) s.CreateCriteria(typeof(User)).UniqueResult();
			Assert.IsTrue(NHibernateUtil.IsInitialized(u2.EmailAddresses));
			Assert.AreEqual(2, u2.EmailAddresses.Count);
			s.Delete(u2);
			t.Commit();
			s.Close();
		}
	}
}