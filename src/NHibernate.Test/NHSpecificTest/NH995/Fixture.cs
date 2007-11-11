using System.Collections;
using System.Collections.Generic;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Plugin;
using log4net.Repository.Hierarchy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH995
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private Logger sqlLogger;
		Level prevLogLevel;
		private MemoryAppender appender;
		
		public override string BugNumber
		{
			get
			{
				return "NH995";
			}
		}

		protected override void OnSetUp()
		{
			// Get the logger that logs the SQL
			ILog log = LogManager.GetLogger("NHibernate.SQL");
			sqlLogger = log.Logger as Logger;
			if (sqlLogger == null)
			{
				Assert.Fail("Unable to get the SQL logger");
			}

			// Change the log level to DEBUG and temporarily save the previous log level
			prevLogLevel = sqlLogger.Level;
			sqlLogger.Level = Level.Debug;

			// Add a new MemoryAppender to the logger.
			appender = new MemoryAppender();
			sqlLogger.AddAppender(appender);
		}

		protected override void OnTearDown()
		{
			// Restore the previous log level of the SQL logger and remove the MemoryAppender
			sqlLogger.Level = prevLogLevel;
			sqlLogger.RemoveAppender(appender);

			using (ISession s = OpenSession())
			using(ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from ClassC");
				s.Delete("from ClassB");
				s.Delete("from ClassA");
				tx.Commit();
			}
		}

		[Test]
		public void Test()
		{
			int a_id;
			using(ISession s = OpenSession())
			using(ITransaction tx = s.BeginTransaction())
			{
				// Create an A and save it
				ClassA a = new ClassA();
				a.Name = "a1";
				s.Save(a);

				// Create a B and save it
				ClassB b = new ClassB();
				b.Id = new ClassBId("bbb", a);
				b.SomeProp = "Some property";
				s.Save(b);

				// Create a C and save it
				ClassC c = new ClassC();
				c.B = b;
				s.Save(c);

				tx.Commit();

				a_id = a.Id;
			}

			// Clear the cache
			sessions.Evict(typeof(ClassA));
			sessions.Evict(typeof(ClassB));
			sessions.Evict(typeof(ClassC));
			
			using(ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				// Load a so we can use it to load b
				ClassA a = s.Get<ClassA>(a_id);

				// Load b so b will be in cache
				ClassB b = s.Get<ClassB>(new ClassBId("bbb", a));

				tx.Commit();
			}
			
			using(ISession s = OpenSession())
			using(ITransaction tx = s.BeginTransaction())
			{
				appender.Clear();

				IList<ClassC> c_list = s.CreateCriteria(typeof (ClassC)).List<ClassC>();
				// make sure we initialize B
				NHibernateUtil.Initialize(c_list[0].B);

				Assert.AreEqual(1, appender.GetEvents().Length,
				                "Only one SQL should have been issued");

				tx.Commit();
			}
		}
	}
}
