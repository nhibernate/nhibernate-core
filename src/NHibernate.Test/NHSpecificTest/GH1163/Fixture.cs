using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1163
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.SupportsSequences;
		}

		[Test]
		public void ValidateSequences()
		{
			var validator = new SchemaValidator(cfg);
			validator.Validate();
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Account>(m =>
			{
				m.Table("account");
				m.Id(x => x.Id, c =>
				{
					c.Column("account_id");
					c.Generator(Generators.Sequence, g => g.Params(new { sequence = "account_id_gen" }));
				});
				m.Property(x => x.Login, c => { c.Column("login_name"); c.NotNullable(true); });
				m.Property(x => x.PasswordHash, c => { c.Column("password_hash"); c.NotNullable(true); });
				m.Property(x => x.ValidFrom, c => { c.Column("valid_from"); c.NotNullable(true); });
				m.Property(x => x.ValidUntil, c => { c.Column("valid_until"); });
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}
	}
}
