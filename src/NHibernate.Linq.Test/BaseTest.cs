using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;

namespace NHibernate.Linq.Test
{
	public class BaseTest
	{
		protected ISession session;

		static BaseTest()
		{
			GlobalSetup.SetupNHibernate();
		}

		[SetUp]
		public virtual void Setup()
		{
			session = GlobalSetup.CreateSession();
		}

		[TearDown]
		public virtual void TearDown()
		{
			session.Connection.Dispose();
			session.Dispose();
			session = null;
		}
	}
}