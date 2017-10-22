using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using NHibernate.Cfg;
using NHibernate.Impl;
using NHibernate.Util;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.GH1391
{
	public class Fixture
	{
		[Test]
		public void MultipleThreadsNested()
		{
			Parallel.For(0, 100, i =>
			{
				var guid = Guid.NewGuid();
				using (new SessionIdLoggingContext(guid))
				{
					Assert.That(SessionIdLoggingContext.SessionId, Is.EqualTo(guid));
					var guid2 = Guid.NewGuid();
					using (new SessionIdLoggingContext(guid2))
					{
						Assert.That(SessionIdLoggingContext.SessionId, Is.EqualTo(guid2));
					}
					Assert.That(SessionIdLoggingContext.SessionId, Is.EqualTo(guid));
				}
				Assert.That(SessionIdLoggingContext.SessionId, Is.Null);
			});
		}
		
		[Test]
		public async Task AsyncContextNested()
		{
			var guid = Guid.NewGuid();
			using (new SessionIdLoggingContext(guid))
			{
				var id = Thread.CurrentThread.ManagedThreadId;
				await Task.Delay(10).ConfigureAwait(false);
				
				Assert.That(SessionIdLoggingContext.SessionId, Is.EqualTo(guid));
				Assert.That(Thread.CurrentThread.ManagedThreadId, Is.Not.EqualTo(id));

				var guid2 = Guid.NewGuid();
				using (new SessionIdLoggingContext(guid2))
				{
					Assert.That(SessionIdLoggingContext.SessionId, Is.EqualTo(guid2));
					await Task.Delay(10).ConfigureAwait(false);
					Assert.That(SessionIdLoggingContext.SessionId, Is.EqualTo(guid2));
				}
				Assert.That(SessionIdLoggingContext.SessionId, Is.EqualTo(guid));
			}
			Assert.That(SessionIdLoggingContext.SessionId, Is.Null);
		}

		[Test]
		public void Disabled()
		{
			var guid = Guid.Empty;
			using (new SessionIdLoggingContext(guid))
			{
				Assert.That(SessionIdLoggingContext.SessionId, Is.Null);
			}
			guid = Guid.NewGuid();
			using (new SessionIdLoggingContext(guid))
			{
				Assert.That(SessionIdLoggingContext.SessionId, Is.EqualTo(guid));
			}
			Assert.That(SessionIdLoggingContext.SessionId, Is.Null);
		}
		
		[Test]
		public void XmlConfiguration()
		{
			const string xml = @"<?xml version='1.0' encoding='utf-8' ?>
<hibernate-configuration xmlns='urn:nhibernate-configuration-2.2'>
	<session-factory name='NHibernate.Test'>
		<property name='track_session_id'>
		false
		</property>
	</session-factory>
</hibernate-configuration>";

			var cfgXml = new XmlDocument();
			cfgXml.LoadXml(xml);

			var cfg = new Configuration();
			using (var xtr = new XmlTextReader(xml, XmlNodeType.Document, null))
			{
				cfg.Configure(xtr);
				Assert.That(PropertiesHelper.GetBoolean(Environment.TrackSessionId, cfg.Properties, true), Is.False);
			}
		}
	}
}
