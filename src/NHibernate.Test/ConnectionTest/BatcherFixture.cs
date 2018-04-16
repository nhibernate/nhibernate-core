using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Util;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.ConnectionTest
{
	[TestFixture]
	public class BatcherFixture : ConnectionManagementTestCase
	{
		protected override void Configure(Configuration config)
		{
			base.Configure(config);
			config.SetProperty(Environment.BatchSize, "10");
		}

		protected override ISession GetSessionUnderTest()
			=> OpenSession();

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			{
				s.CreateQuery("delete from System.Object").ExecuteUpdate();
			}
		}

		[Test]
		public void CanCloseCommandsAndUseBatcher()
		{
			using (var s = OpenSession())
			{
				// Need a generator strategy not causing insert at save.
				var silly = new YetAnother { Name = "Silly" };
				s.Save(silly);
				s.GetSessionImplementation().ConnectionManager.Batcher.CloseCommands();
				
				Assert.DoesNotThrow(s.Flush, "Flush failure after closing commands.");
			}
		}
	}
}