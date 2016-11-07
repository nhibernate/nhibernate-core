using System.Text;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1270
{
	public class Fixture
	{
		private HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<User>(rt =>
							   {
								rt.Id(x => x.Id, map => map.Generator(Generators.Guid));
								rt.Property(x => x.Name);
													rt.Set(x => x.Roles, map =>
																							 {
																								 map.Table("UsersToRoles");
																								 map.Inverse(true);
																								 map.Key(km => km.Column("UserId"));
																							 }, rel => rel.ManyToMany(mm =>
																													  {
																																					mm.Column("RoleId");
																														mm.ForeignKey("FK_RoleInUser");
																													  }));
							   });
			mapper.Class<Role>(rt =>
							   {
								rt.Id(x => x.Id, map => map.Generator(Generators.Guid));
								rt.Property(x => x.Name);
								rt.Set(x => x.Users, map =>
													 {
														map.Table("UsersToRoles");
																								map.Key(km => km.Column("RoleId"));
																							 }, rel => rel.ManyToMany(mm =>
																													  {
																																					mm.Column("UserId");
																														mm.ForeignKey("FK_UserInRole");
																													  }));
							   });
			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
			return mappings;
		}

		[Test]
		public void WhenMapCustomFkNamesThenUseIt()
		{
			var conf = TestConfigurationHelper.GetDefaultConfiguration();
			conf.DataBaseIntegration(i=> i.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote);
			conf.AddMapping(GetMappings());
			var sb = new StringBuilder();
			(new SchemaExport(conf)).Create(s => sb.AppendLine(s), true);

			Assert.That(sb.ToString(), Is.StringContaining("FK_RoleInUser").And.StringContaining("FK_UserInRole"));
			(new SchemaExport(conf)).Drop(false, true);
		}
	}
}