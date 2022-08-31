using System;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	[TestFixture]
	[Obsolete]
	public class TimestampTypeFixture : AbstractDateTimeTypeFixture
	{
		protected override string TypeName => "Timestamp";
		protected override AbstractDateTimeType Type => NHibernateUtil.Timestamp;

		[Test]
		public void ObsoleteMessage()
		{
			using (var spy = new LogSpy(typeof(TypeFactory)))
			{
				var config = TestConfigurationHelper.GetDefaultConfiguration();
				AddMappings(config);
				Configure(config);
				using (config.BuildSessionFactory())
				{
					var log = spy.GetWholeLog();
					Assert.That(
						log,
						Does.Contain("NHibernate.Type.TimestampType is obsolete. Please use DateTimeType instead.").IgnoreCase);
				}
			}
		}
	}
}
