using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			var cfg = new Configuration().Configure();

			using (var sessionFactory = cfg.BuildSessionFactory())
			{
				using (var session = sessionFactory.OpenSession())
				{
					Assert.AreEqual(session.FlushMode, FlushMode.Commit);
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
					Assert.AreEqual(session.FlushMode, FlushMode.Always);
				}
			}

			cfg.Properties[Environment.DefaultFlushMode] = FlushMode.Commit.ToString();

			using (var sessionFactory = cfg.BuildSessionFactory())
			{
				using (var session = sessionFactory.OpenSession())
				{
					Assert.AreEqual(session.FlushMode, FlushMode.Commit);
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
					Assert.AreEqual(session.FlushMode, FlushMode.Always);
				}
			}

			cfg.Configure()
				.SessionFactory()
				.DefaultFlushMode(FlushMode.Commit);

			using (var sessionFactory = cfg.BuildSessionFactory())
			{
				using (var session = sessionFactory.OpenSession())
				{
					Assert.AreEqual(session.FlushMode, FlushMode.Commit);
				}
			}
		}
	}
}
