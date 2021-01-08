using System;
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

		[Test,TestCaseSource(nameof(TestCases))]
		public object ReadsSettingsCorrectly(string key, string value, Func<Settings,object> settingsProp)
		{
			//Dialect needed to prevent exception
			var properties = new Dictionary<string, string>
			{
				{"dialect", typeof (Dialect.MsSql2005Dialect).FullName}
			};

			properties[key] = value;

			var settings = new SettingsFactory().BuildSettings(properties);
			
			return settingsProp(settings);
		}

		public static IEnumerable<TestCaseData> TestCases
		{
			get
			{
				yield return new SettingsTestCaseData("query.plan_cache_max_size", "256", x => x.QueryPlanCacheMaxSize).Returns(256);
				yield return new SettingsTestCaseData("query.plan_parameter_metadata_max_size", "512", x => x.QueryPlanCacheParameterMetadataMaxSize).Returns(512);
			}
		}

		private class SettingsTestCaseData : TestCaseData
		{
			public SettingsTestCaseData(string key, string value, Func<Settings, object> settingsProp) : base(key, value, settingsProp)
			{

			}
		}
	}
}
