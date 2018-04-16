using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3793
{
	[TestFixture]
	public class Fixture 
	{
		[Test]
		public void CanCreateSchemaWithCompositeIdWithKeyManyToOneUsesEntityName()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH3793.Mappings.hbm.xml", GetType().Assembly);
			Assert.DoesNotThrow(() => cfg.BuildMappings());
		}
	}
}