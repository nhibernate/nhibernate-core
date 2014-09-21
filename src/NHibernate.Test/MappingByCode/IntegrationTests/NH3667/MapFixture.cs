using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH3667
{
	[TestFixture]
	public class MapFixture
	{
		[Test]
		public void TestMapElementElement()
		{
			var cfg = new Configuration().Configure();
			var mapper = new ModelMapper();

			mapper.Class<ClassWithMapElementElement>(c =>
				{
					c.Lazy(false);
					c.Id(id => id.Id, id =>
						{
							id.Generator(Generators.Identity);
						});

					c.Map(m => m.Map, col =>
						{
							col.Table("element_element");
							col.Key(k => k.Column("id"));
						}, key =>
						{
							key.Element(e =>
								{
									e.Column("key");
								});
						}, element =>
						{
							element.Element(e =>
								{
									e.Column("element");
								});
						});
				});

			cfg.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

			var script = cfg.GenerateSchemaCreationScript(new MsSql2012Dialect());

			Assert.False(script.Any(x => x.Contains("idx")));
		}

		[Test]
		public void TestMapEntityEntity()
		{
			var cfg = new Configuration().Configure();
			var mapper = new ModelMapper();

			mapper.Class<ClassWithMapEntityEntity>(c =>
			{
				c.Lazy(false);
				c.Id(id => id.Id, id =>
				{
					id.Generator(Generators.Identity);
				});

				c.Map(m => m.Map, col =>
				{
					col.Table("entity_entity");
					col.Key(k => k.Column("id"));
				}, key =>
				{
					key.ManyToMany(e =>
					{
						e.Column("key");
					});
				}, element =>
				{
					element.ManyToMany(e =>
					{
						e.Column("element");
					});
				});
			});

			mapper.Class<Entity>(c =>
				{
					c.Lazy(false);
					c.Id(id => id.A, id =>
					{
						id.Generator(Generators.Identity);
					});
					c.Property(p => p.B);
				});

			cfg.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

			var script = cfg.GenerateSchemaCreationScript(new MsSql2012Dialect());

			Assert.False(script.Any(x => x.Contains("idx")));
		}

		[Test]
		public void TestMapEntityElement()
		{
			var cfg = new Configuration().Configure();
			var mapper = new ModelMapper();

			mapper.Class<ClassWithMapEntityElement>(c =>
			{
				c.Lazy(false);
				c.Id(id => id.Id, id =>
				{
					id.Generator(Generators.Identity);
				});

				c.Map(m => m.Map, col =>
				{
					col.Table("entity_element");
					col.Key(k => k.Column("id"));
				}, key =>
				{
					key.ManyToMany(e =>
					{
						e.Column("key");
					});
				}, element =>
				{
					element.Element(e =>
					{
						e.Column("element");
					});
				});
			});

			mapper.Class<Entity>(c =>
			{
				c.Lazy(false);
				c.Id(id => id.A, id =>
				{
					id.Generator(Generators.Identity);
				});
				c.Property(p => p.B);
			});

			cfg.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

			var script = cfg.GenerateSchemaCreationScript(new MsSql2012Dialect());

			Assert.False(script.Any(x => x.Contains("idx")));
		}

		[Test]
		public void TestMapElementEntity()
		{
			var cfg = new Configuration().Configure();
			var mapper = new ModelMapper();

			mapper.Class<ClassWithMapElementEntity>(c =>
			{
				c.Lazy(false);
				c.Id(id => id.Id, id =>
				{
					id.Generator(Generators.Identity);
				});

				c.Map(m => m.Map, col =>
				{
					col.Table("element_entity");
					col.Key(k => k.Column("id"));
				}, key =>
				{
					key.Element(e =>
					{
						e.Column("key");
					});
				}, element =>
				{
					element.ManyToMany(e =>
					{
						e.Column("element");
					});
				});
			});

			mapper.Class<Entity>(c =>
			{
				c.Lazy(false);
				c.Id(id => id.A, id =>
				{
					id.Generator(Generators.Identity);
				});
				c.Property(p => p.B);
			});

			cfg.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

			var script = cfg.GenerateSchemaCreationScript(new MsSql2012Dialect());

			Assert.False(script.Any(x => x.Contains("idx")));
		}

		[Test]
		public void TestMapEntityComponent()
		{
			var cfg = new Configuration().Configure();
			var mapper = new ModelMapper();

			mapper.Class<ClassWithMapEntityComponent>(c =>
			{
				c.Lazy(false);
				c.Id(id => id.Id, id =>
				{
					id.Generator(Generators.Identity);
				});

				c.Map(m => m.Map, col =>
				{
					col.Table("entity_component");
					col.Key(k => k.Column("id"));
				}, key =>
				{
					key.ManyToMany(e =>
					{
						e.Column("key");
					});
				}, element =>
				{
					element.Component(cmp =>
						{
							cmp.Class<Component>();
						});
				});
			});

			mapper.Component<Component>(c =>
				 {
					 c.Property(p => p.A);
					 c.Property(p => p.B);
				 });

			mapper.Class<Entity>(c =>
			{
				c.Lazy(false);
				c.Id(id => id.A, id =>
				{
					id.Generator(Generators.Identity);
				});
				c.Property(p => p.B);
			});

			cfg.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

			var script = cfg.GenerateSchemaCreationScript(new MsSql2012Dialect());

			Assert.False(script.Any(x => x.Contains("idx")));
		}

		[Test]
		public void TestMapElementComponent()
		{
			var cfg = new Configuration().Configure();
			var mapper = new ModelMapper();

			mapper.Class<ClassWithMapElementComponent>(c =>
			{
				c.Lazy(false);
				c.Id(id => id.Id, id =>
				{
					id.Generator(Generators.Identity);
				});

				c.Map(m => m.Map, col =>
				{
					col.Table("element_component");
					col.Key(k => k.Column("id"));
				}, key =>
				{
					key.Element(e =>
					{
						e.Column("key");
					});
				}, element =>
				{
					element.Component(cmp =>
					{
						cmp.Class<Component>();
					});
				});
			});

			mapper.Component<Component>(c =>
			{
				c.Property(p => p.A);
				c.Property(p => p.B);
			});

			cfg.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

			var script = cfg.GenerateSchemaCreationScript(new MsSql2012Dialect());

			Assert.False(script.Any(x => x.Contains("idx")));
		}

		[Test]
		public void TestMapComponentComponent()
		{
			var cfg = new Configuration().Configure();
			var mapper = new ModelMapper();

			mapper.Class<ClassWithMapComponentComponent>(c =>
			{
				c.Lazy(false);
				c.Id(id => id.Id, id =>
				{
					id.Generator(Generators.Identity);
				});

				c.Map(m => m.Map, col =>
				{
					col.Table("component_component");
					col.Key(k => k.Column("id"));
				}, key =>
				{
					key.Component(cmp =>
					{
						cmp.Property(p => p.A);
						cmp.Property(p => p.B);
					});
				}, element =>
				{
					element.Component(cmp =>
					{
						cmp.Class<Component>();
					});
				});
			});

			mapper.Component<Component>(c =>
			{
				c.Property(p => p.A);
				c.Property(p => p.B);
			});

			cfg.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

			var script = cfg.GenerateSchemaCreationScript(new MsSql2012Dialect());

			Assert.False(script.Any(x => x.Contains("idx")));
		}

		[Test]
		public void TestMapComponentElement()
		{
			var cfg = new Configuration().Configure();
			var mapper = new ModelMapper();

			mapper.Class<ClassWithMapComponentElement>(c =>
			{
				c.Lazy(false);
				c.Id(id => id.Id, id =>
				{
					id.Generator(Generators.Identity);
				});

				c.Map(m => m.Map, col =>
				{
					col.Table("component_element");
					col.Key(k => k.Column("id"));
				}, key =>
				{
					key.Component(cmp =>
					{
						cmp.Property(p => p.A);
						cmp.Property(p => p.B);
					});
				}, element =>
				{
					element.Element(e =>
					{
						e.Column("element");
					});
				});
			});

			mapper.Component<Component>(c =>
			{
				c.Property(p => p.A);
				c.Property(p => p.B);
			});

			cfg.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

			var script = cfg.GenerateSchemaCreationScript(new MsSql2012Dialect());

			Assert.False(script.Any(x => x.Contains("idx")));
		}

		//OK
		[Test]
		public void TestMapComponentEntity()
		{
			var cfg = new Configuration().Configure();
			var mapper = new ModelMapper();

			mapper.Class<ClassWithMapComponentEntity>(c =>
			{
				c.Lazy(false);
				c.Id(id => id.Id, id =>
				{
					id.Generator(Generators.Identity);
				});

				c.Map(m => m.Map, col =>
				{
					col.Table("component_entity");
					col.Key(k => k.Column("id"));
				}, key =>
				{
					key.Component(cmp =>
					{
						cmp.Property(p => p.A);
						cmp.Property(p => p.B);
					});
				}, element =>
				{
					element.ManyToMany(e =>
					{
						e.Column("element");
					});
				});
			});

			mapper.Component<Component>(c =>
			{
				c.Property(p => p.A);
				c.Property(p => p.B);
			});

			mapper.Class<Entity>(c =>
			{
				c.Lazy(false);
				c.Id(id => id.A, id =>
				{
					id.Generator(Generators.Identity);
				});
				c.Property(p => p.B);
			});

			cfg.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

			var script = cfg.GenerateSchemaCreationScript(new MsSql2012Dialect());

			Assert.False(script.Any(x => x.Contains("idx")));
		}
	}
}
