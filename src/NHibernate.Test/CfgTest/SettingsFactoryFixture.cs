using System.Collections.Generic;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.CfgTest
{
	[TestFixture]
	public class SettingsFactoryFixture
	{
		[Test]
		public void DefaultValueForKeyWords()
		{
			var properties = new Dictionary<string, string>
			                 	{
			                 		{"dialect", typeof (Dialect.MsSql2005Dialect).FullName}
			                 	};
			var settings = new SettingsFactory().BuildSettings(properties);
			Assert.That(settings.IsKeywordsImportEnabled);
			Assert.That(!settings.IsAutoQuoteEnabled);
		}
	}
}