using System.Collections;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH2825
{
	[TestFixture]
	public class FixtureByCode : Fixture
	{
		protected override IList Mappings
		{
			get { return new string[0]; }
		}

		protected override string MappingsAssembly
		{
			get { return null; }
		}

		protected override void AddMappings(Cfg.Configuration configuration)
		{
			configuration.AddDeserializedMapping(GetMappings(), "TestDomain");
		}

		private HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			mapper.BeforeMapClass += (mi, t, map) => map.Id(x => x.Generator(Generators.GuidComb));

			mapper.Class<Parent>(rc =>
			{
				rc.Id(p => p.Id);
				rc.Property(p => p.ParentCode, m => m.Unique(true));
				rc.Property(p => p.Name);
				rc.Bag(p => p.Children, m =>
				{
					m.Key(km => { km.Column(cm => cm.Name("ParentParentCode")); km.PropertyRef(pg => pg.ParentCode); });
					m.Inverse(true);
					m.Cascade(Mapping.ByCode.Cascade.Persist);
				}, rel => rel.OneToMany());
			});

			mapper.Class<Child>(rc =>
			{
				rc.Id(p => p.Id);
				rc.Property(p => p.Name);
				rc.ManyToOne<Parent>(p => p.Parent, m => { m.Column("ParentParentCode"); m.PropertyRef("ParentCode"); });
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}
	}
}