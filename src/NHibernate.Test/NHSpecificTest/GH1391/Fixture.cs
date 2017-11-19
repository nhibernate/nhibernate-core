using System;
using System.Threading.Tasks;
using System.Xml;
using NHibernate.Cfg;
using NHibernate.Impl;
using NHibernate.Util;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.GH1391
{
	[TestFixture]
	public class Fixture
	{
		private readonly Random _random = new Random();
		
		[Test]
		public void Concurrent()
		{
			// Simulating two session factories, where one has tracking enabled and the other disabled
			Parallel.For(0, 100, i =>
			{
				if (_random.Next(2) == 0)
				{
					Enabled();
				}
				else
				{
					Disabled();
				}
			});
		}

		[Test]
		public void ConcurrentAsync()
		{
			async Task RunAsync(bool enabled)
			{
				for (var i = 0; i < 50; i++)
				{
					if (enabled)
					{
						await EnabledAsync().ConfigureAwait(false);
					}
					else
					{
						await DisabledAsync().ConfigureAwait(false);
					}
				}
			}
			// Simulating two session factories, where one has tracking enabled and the other disabled
			Task.WaitAll(RunAsync(true), RunAsync(false));
		}
		
		[Test]
		public void Enabled()
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
		}
		
		[Test]
		public async Task EnabledAsync()
		{
			var guid = Guid.NewGuid();
			using (new SessionIdLoggingContext(guid))
			{
				Assert.That(SessionIdLoggingContext.SessionId, Is.EqualTo(guid));
				await Task.Delay(1).ConfigureAwait(false);
				Assert.That(SessionIdLoggingContext.SessionId, Is.EqualTo(guid));

				var guid2 = Guid.NewGuid();
				using (new SessionIdLoggingContext(guid2))
				{
					Assert.That(SessionIdLoggingContext.SessionId, Is.EqualTo(guid2));
					await Task.Delay(1).ConfigureAwait(false);
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
				using (new SessionIdLoggingContext(guid))
				{
					Assert.That(SessionIdLoggingContext.SessionId, Is.Null);
				}
				Assert.That(SessionIdLoggingContext.SessionId, Is.Null);
			}
			Assert.That(SessionIdLoggingContext.SessionId, Is.Null);
		}
		
		[Test]
		public async Task DisabledAsync()
		{
			var guid = Guid.Empty;
			using (new SessionIdLoggingContext(guid))
			{
				Assert.That(SessionIdLoggingContext.SessionId, Is.Null);
				await Task.Delay(1).ConfigureAwait(false);
				Assert.That(SessionIdLoggingContext.SessionId, Is.Null);
				
				using (new SessionIdLoggingContext(guid))
				{
					Assert.That(SessionIdLoggingContext.SessionId, Is.Null);
					await Task.Delay(1).ConfigureAwait(false);
					Assert.That(SessionIdLoggingContext.SessionId, Is.Null);
				}
				Assert.That(SessionIdLoggingContext.SessionId, Is.Null);
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
