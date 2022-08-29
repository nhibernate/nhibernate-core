using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UserCollection.Parameterized
{
	public class ParameterizedUserCollectionTypeByCodeFixture : TestCaseMappingByCode
	{
		protected override string MappingsAssembly => "NHibernate.Test";

		protected override void AddMappings(Configuration configuration)
		{
			configuration.TypeDefinition<DefaultableListType>(
				c =>
				{
					c.Alias = "DefaultableList";
					c.Properties = new { @default = "Hello" };
				});

			base.AddMappings(configuration);
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			mapper.Class<Entity>(
				c =>
				{
					c.Id(e => e.Name, im => im.Access(Accessor.Field));
					c.List(
						e => e.Values,
						lpm =>
						{
							lpm.Fetch(CollectionFetchMode.Join);
							lpm.Table("ENT_VAL");
							lpm.Type("DefaultableList");
							lpm.Key(km => km.Column("ENT_ID"));
							lpm.Index(lim => lim.Column("POS"));
						},
						lm =>
						{
							lm.Element(em => em.Column("VAL"));
						});
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void BasicOperation()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var entity = new Entity("tester");
				entity.Values.Add("value-1");
				s.Persist(entity);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var entity = s.Get<Entity>("tester");
				Assert.That(NHibernateUtil.IsInitialized(entity.Values), Is.True);
				Assert.That(entity.Values.Count, Is.EqualTo(1));
				Assert.That(((IDefaultableList) entity.Values).DefaultValue, Is.EqualTo("Hello"));

				s.Delete(entity);
				t.Commit();
			}
		}
	}
}
