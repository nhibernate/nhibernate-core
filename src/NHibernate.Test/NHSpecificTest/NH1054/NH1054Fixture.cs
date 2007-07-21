using System;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1054
{
	[TestFixture]
	public class NH1054Fixture
	{
		[Test]
		public void SettingsTransactionFactoryReturnsConfiguredTransactionFactory()
		{
			Configuration configuration = new Configuration();
			configuration.Properties[Cfg.Environment.TransactionStrategy] =
				"NHibernate.Test.NHSpecificTest.NH1054.DummyTransactionFactory, " + this.GetType().Assembly.FullName;

			ISessionFactory sessionFactory = configuration.BuildSessionFactory();

			Assert.IsInstanceOfType(typeof(DummyTransactionFactory), 
				sessionFactory.Settings.TransactionFactory);
		}

		[Test]
		public void AdoNetTransactionFactoryIsDefaultTransactionFactory()
		{
			Configuration configuration = new Configuration();
			ISessionFactory sessionFactory = configuration.BuildSessionFactory();

			Assert.IsInstanceOfType(typeof(NHibernate.Transaction.AdoNetTransactionFactory), 
				sessionFactory.Settings.TransactionFactory);
		}
	}
}
