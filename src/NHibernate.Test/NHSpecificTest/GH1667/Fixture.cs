using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1667
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private INHibernateLoggerFactory _defaultLogger;

		private static readonly FieldInfo _loggerFactoryField =
			typeof(NHibernateLogger).GetField("_loggerFactory", BindingFlags.NonPublic | BindingFlags.Static);

		protected override void OnSetUp()
		{
			_defaultLogger = (INHibernateLoggerFactory) _loggerFactoryField.GetValue(null);
			NHibernateLogger.SetLoggersFactory(new EnumeratingLoggerFactory());

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new EntityChild { Name = "Bob" };
				session.Save(e1);

				var e2 = new Entity { Name = "Sally", Children = new HashSet<EntityChild> { e1 } };
				session.Save(e2);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			if (_defaultLogger != null)
				NHibernateLogger.SetLoggersFactory(_defaultLogger);

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from EntityChild").ExecuteUpdate();
				session.CreateQuery("delete from Entity").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void LoggingDoesNotWreckCollectionLoading()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var parent = session.Query<Entity>().First();
				Assert.That(parent.Children, Has.Count.EqualTo(1));
			}
		}
	}

	public class EnumeratingLoggerFactory : INHibernateLoggerFactory
	{
		public INHibernateLogger LoggerFor(string keyName)
		{
			return new EnumeratingLogger();
		}

		public INHibernateLogger LoggerFor(System.Type type)
		{
			return new EnumeratingLogger();
		}
	}

	public class EnumeratingLogger : INHibernateLogger
	{
		public void Log(NHibernateLogLevel logLevel, NHibernateLogValues state, Exception exception)
		{
			if (state.Args == null)
				return;

			foreach (var arg in state.Args)
			{
				if (!(arg is IEnumerable e))
					continue;

				var enumerator = e.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
					}
				}
				finally
				{
					if (enumerator is IDisposable disp)
						disp.Dispose();
				}
			}
		}

		public bool IsEnabled(NHibernateLogLevel logLevel)
		{
			return true;
		}
	}
}
