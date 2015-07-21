using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.CfgTest
{
	[TestFixture]
	public class EntityCacheUsageParserFixture
	{
		[Test]
		public void CovertToString()
		{
			Assert.That(EntityCacheUsageParser.ToString(EntityCacheUsage.Readonly), Is.EqualTo("read-only"));
			Assert.That(EntityCacheUsageParser.ToString(EntityCacheUsage.ReadWrite), Is.EqualTo("read-write"));
			Assert.That(EntityCacheUsageParser.ToString(EntityCacheUsage.NonStrictReadWrite), Is.EqualTo("nonstrict-read-write"));
			Assert.That(EntityCacheUsageParser.ToString(EntityCacheUsage.Transactional), Is.EqualTo("transactional"));
		}

		[Test]
		public void Parse()
		{
			Assert.That(EntityCacheUsageParser.Parse("read-only"), Is.EqualTo(EntityCacheUsage.Readonly));
			Assert.That(EntityCacheUsageParser.Parse("read-write"), Is.EqualTo(EntityCacheUsage.ReadWrite));
			Assert.That(EntityCacheUsageParser.Parse("nonstrict-read-write"), Is.EqualTo(EntityCacheUsage.NonStrictReadWrite));
			Assert.That(EntityCacheUsageParser.Parse("transactional"), Is.EqualTo(EntityCacheUsage.Transactional));
		}
	}
}