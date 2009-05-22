using System;
using NHibernate.Cfg;
using NHibernate.Engine;
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

			ISessionFactoryImplementor sessionFactory = (ISessionFactoryImplementor)configuration.BuildSessionFactory();

			Assert.IsInstanceOfType(typeof(DummyTransactionFactory), 
				sessionFactory.Settings.TransactionFactory);
		}

		[Test]
		public void AdoNetWithDistributedTransactionFactoryIsDefaultTransactionFactory()
		{
			Configuration configuration = new Configuration();
			ISessionFactoryImplementor sessionFactory = (ISessionFactoryImplementor)configuration.BuildSessionFactory();

			Assert.IsInstanceOfType(typeof(NHibernate.Transaction.AdoNetWithDistrubtedTransactionFactory), 
				sessionFactory.Settings.TransactionFactory);
		}
	}
}
