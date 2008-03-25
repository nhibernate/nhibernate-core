using System;
using System.Collections;

using NHibernate.Cfg;

using NUnit.Framework;

namespace NHibernate.Examples.QuickStart
{
	/// <summary>
	/// Summary description for UserFixture.
	/// </summary>
	[TestFixture]
	public class UserFixture
	{
		[Test]
		public void ValidateQuickStart()
		{
			Configuration cfg = new Configuration();
			cfg.AddAssembly("NHibernate.Examples");

			ISessionFactory factory = cfg.BuildSessionFactory();
			ISession session = factory.OpenSession();
			ITransaction transaction = session.BeginTransaction();

			User newUser = new User();
			newUser.Id = "joe_cool";
			newUser.UserName = "Joseph Cool";
			newUser.Password = "abc123";
			newUser.EmailAddress = "joe@cool.com";
			newUser.LastLogon = DateTime.Now;

			// Tell NHibernate that this object should be saved
			session.Save(newUser);

			// commit all of the changes to the DB and close the ISession
			transaction.Commit();
			session.Close();

			// open another session to retrieve the just inserted user
			session = factory.OpenSession();

			User joeCool = (User) session.Load(typeof(User), "joe_cool");

			// set Joe Cool's Last Login property
			joeCool.LastLogon = DateTime.Now;

			// flush the changes from the Session to the Database
			session.Flush();

			IList recentUsers = session.CreateCriteria(typeof(User))
				.Add(Criterion.Expression.Gt("LastLogon", new DateTime(2004, 03, 14, 20, 0, 0)))
				.List();

			foreach (User user in recentUsers)
			{
				Assert.IsTrue(user.LastLogon > (new DateTime(2004, 03, 14, 20, 0, 0)));
			}

			session.Close();
		}
	}
}