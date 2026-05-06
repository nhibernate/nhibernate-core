using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3607
{
	/// <summary>
	/// By code mapping serialization failure since v5.4.1. Adapted from <see href="https://github.com/craigfowler/NHibernate.XmlConversionBug" />.
	/// </summary>
	[TestFixture]
	public class FixtureByCode : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.AddMappings(new[] { typeof(OrderMapping), typeof(LineItemMapping), typeof(LineItemDataMapping) });
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void SerializeMappingToXml()
		{
			var mapping = GetMappings();
			string serialized = "";
			Assert.That(() => serialized = mapping.AsString(), Throws.Nothing, "Mapping serialization failure");
			var config = new Configuration();
			Assert.That(() => config.AddXml(serialized), Throws.Nothing, "Configuration with serialized mapping has failed");
		}
	}
}
