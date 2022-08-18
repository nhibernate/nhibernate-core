using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;

namespace NHibernate.Test.NHSpecificTest.NH3530
{
	class EntityMappings
	{
		public static HbmMapping CreateMapping()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(x => x.Name);
					rc.BatchSize(3);
					rc.Bag(
						x => x.Children,
						m =>
						{
							m.Key(
								km =>
								{
									km.Column("ParentId");
									km.ForeignKey("none");
								});
							m.BatchSize(10);
							m.Cascade(Mapping.ByCode.Cascade.All);
						},
						xr => xr.OneToMany());
				});

			mapper.Class<EntityComponent>(
				rc =>
				{
					rc.ComposedId(
						x =>
						{
							x.Property(t => t.Id1);
							x.Property(e => e.Id2);
						});
					rc.Property(x => x.Name);
					rc.BatchSize(3);
					rc.Bag(
						x => x.Children,
						m =>
						{
							m.Key(
								km =>
								{
									km.Columns(x => x.Name("ParentId1"), x => x.Name("ParentId2"));
									km.ForeignKey("none");
								});
							m.BatchSize(10);
							m.Cascade(Mapping.ByCode.Cascade.All);
						},
						xr => xr.OneToMany());
				});

			mapper.Class<EntityComponentId>(
				rc =>
				{
					rc.ComponentAsId(
						x => x.Id,
						m =>
						{
							m.Property(t => t.Id1);
							m.Property(e => e.Id2);
						}
					);
					rc.Property(x => x.Name);
					rc.BatchSize(3);
					rc.Bag(
						x => x.Children,
						m =>
						{
							m.Key(
								km =>
								{
									km.Columns(x => x.Name("ParentId1"), x => x.Name("ParentId2"));
									km.ForeignKey("none");
								});
							m.BatchSize(10);
							m.Cascade(Mapping.ByCode.Cascade.All);
						},
						xr => xr.OneToMany());
				});

			mapper.Class<Child>(
				rc =>
				{
					rc.Id(m => m.Id, m => m.Generator(Generators.Guid));
					rc.Property(m => m.Name);
					rc.Property(m => m.ParentId);
				});

			mapper.Class<ChildForComponent>(
				rc =>
				{
					rc.Id(m => m.Id, m => m.Generator(Generators.Guid));
					rc.Property(m => m.Name);
					rc.Property(m => m.ParentId1);
					rc.Property(m => m.ParentId2);
				});
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}
	}
}
