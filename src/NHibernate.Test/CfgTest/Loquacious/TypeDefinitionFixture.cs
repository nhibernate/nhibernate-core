using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Id;
using NUnit.Framework;

namespace NHibernate.Test.CfgTest.Loquacious
{
	[TestFixture]
	public class TypeDefinitionFixture
	{
		[Test]
		public void AddTypeDef()
		{
			var configure = new Configuration()
				.ByCode(
					x => x.DataBaseIntegration(db => db.Dialect<MsSql2005Dialect>())
						.TypeDefinition<TableHiLoGenerator>(
							c =>
							{
								c.Alias = "HighLow";
								c.Properties = new {max_lo = 99};
							}));
			var mappings = configure.CreateMappings();
			var typeDef = mappings.GetTypeDef("HighLow");
			Assert.That(typeDef, Is.Not.Null);
			Assert.That(typeDef.Parameters["max_lo"], Is.EqualTo("99"));
		}
	}
}
