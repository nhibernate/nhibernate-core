using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.CfgTest
{
	[TestFixture]
	public class DefaultFlushModeFixture
	{
		[Test]
		public void CanSetDefaultFlushModeThroughXmlConfiguration()
		{
			const string c = @"<hibernate-configuration xmlns=""urn:nhibernate-configuration-2.2"">
		<session-factory name=""NHibernate.Test"">
			<property name=""default_flush_mode"">Commit</property>
		</session-factory>
	</hibernate-configuration>";

			var cfg = new Configuration().Configure(new XmlTextReader(new StringReader(c)));

			using (var sessionFactory = cfg.BuildSessionFactory())
			{
				using (var session = sessionFactory.OpenSession())
				{
					Assert.AreEqual(FlushMode.Commit, session.FlushMode);
				}
			}
		}

		[Test]
		public void CanSetDefaultFlushModeThroughStandardConfiguration()
		{
			var cfg = new Configuration().Configure();
			cfg.Properties[Environment.DefaultFlushMode] = FlushMode.Always.ToString();

			using (var sessionFactory = cfg.BuildSessionFactory())
			{
				using (var session = sessionFactory.OpenSession())
				{
					Assert.AreEqual(FlushMode.Always, session.FlushMode);
				}
			}

			cfg.Properties[Environment.DefaultFlushMode] = FlushMode.Commit.ToString();

			using (var sessionFactory = cfg.BuildSessionFactory())
			{
				using (var session = sessionFactory.OpenSession())
				{
					Assert.AreEqual(FlushMode.Commit, session.FlushMode);
				}
			}
		}

		[Test]
		public void CanSetDefaultFlushModeThroughLoquaciousConfiguration()
		{
			var cfg = new Configuration()
				.Configure();

			cfg
				.SessionFactory()
				.DefaultFlushMode(FlushMode.Always);

			using (var sessionFactory = cfg.BuildSessionFactory())
			{
				using (var session = sessionFactory.OpenSession())
				{
					Assert.AreEqual(FlushMode.Always, session.FlushMode);
				}
			}

			cfg.Configure()
				.SessionFactory()
				.DefaultFlushMode(FlushMode.Commit);

			using (var sessionFactory = cfg.BuildSessionFactory())
			{
				using (var session = sessionFactory.OpenSession())
				{
					Assert.AreEqual(FlushMode.Commit, session.FlushMode);
				}
			}
		}
	}
}
