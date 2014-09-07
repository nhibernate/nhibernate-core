using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	public class ComponentAsIdTests
	{
		private class MyClass
		{
			private IMyCompo id;
			public IMyCompo Id
			{
				get { return id; }
				set { id = value; }
			}
		}

		private interface IMyCompo
		{
			string Code { get; set; }
			string Name { get; set; }
		}
		private class MyComponent : IMyCompo
		{
			public string Code { get; set; }
			public string Name { get; set; }
		}

		[Test]
		public void CanSpecifyUnsavedValue()
		{
			//NH-3048
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(map => map.ComponentAsId(x => x.Id, x =>
				{
					x.UnsavedValue(UnsavedValueType.Any);
				}));

			var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

			Assert.AreEqual(mapping.RootClasses[0].CompositeId.unsavedvalue, HbmUnsavedValueType.Any);
		}

		[Test]
		public void WhenPropertyUsedAsComposedIdThenRegister()
		{
			var inspector = new ExplicitlyDeclaredModel();
			var mapper = new ModelMapper(inspector);
			mapper.Class<MyClass>(map => map.ComponentAsId(x => x.Id));

			inspector.IsPersistentId(For<MyClass>.Property(x => x.Id)).Should().Be.True();
			inspector.IsPersistentProperty(For<MyClass>.Property(x => x.Id)).Should().Be.True();
			inspector.IsComponent(typeof(IMyCompo)).Should().Be.True();
		}

		[Test]
		public void WhenMapComponentAsIdThenMapItAndItsProperties()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(map => map.ComponentAsId(x => x.Id, idmap =>
			                                                          {
			                                                          	idmap.Property(y => y.Code);
			                                                          	idmap.Property(y => y.Name);
			                                                          }));

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmCompositId = hbmClass.CompositeId;
			var keyProperties = hbmCompositId.Items.OfType<HbmKeyProperty>();
			keyProperties.Should().Have.Count.EqualTo(2);
			keyProperties.Select(x => x.Name).Should().Have.SameValuesAs("Code", "Name");
			hbmCompositId.name.Should().Be(For<MyClass>.Property(x => x.Id).Name);
		}

		[Test]
		public void WhenMapComponentAsIdAttributesThenMapAttributes()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(map => map.ComponentAsId(x => x.Id, idmap =>
			{
				idmap.Access(Accessor.Field);
				idmap.Class<MyComponent>();
			}));

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmCompositId = hbmClass.CompositeId;
			hbmCompositId.access.Should().Contain("field");
			hbmCompositId.@class.Should().Contain("MyComponent");
		}

		[Test]
		public void WhenMapComponentUsedAsComponentAsIdThenMapItAndItsProperties()
		{
			var mapper = new ModelMapper();
			mapper.Component<IMyCompo>(x =>
			                           {
																	 x.Property(y => y.Code, pm => pm.Length(10));
																	 x.Property(y => y.Name);
			                           });
			mapper.Class<MyClass>(map => map.ComponentAsId(x => x.Id));

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmCompositId = hbmClass.CompositeId;
			var keyProperties = hbmCompositId.Items.OfType<HbmKeyProperty>();
			keyProperties.Should().Have.Count.EqualTo(2);
			keyProperties.Select(x => x.Name).Should().Have.SameValuesAs("Code", "Name");
			keyProperties.Where(x => x.Name == "Code").Single().length.Should().Be("10");
		}

		[Test]
		public void WhenMapCustomizedComponentUsedAsComponentAsIdWithCustomizationThenUseComponentAsIdCustomization()
		{
			var mapper = new ModelMapper();
			mapper.Component<IMyCompo>(x =>
			{
				x.Property(y => y.Code, pm=> pm.Length(10));
				x.Property(y => y.Name, pm => pm.Length(20));
			});
			mapper.Class<MyClass>(map => map.ComponentAsId(x => x.Id, idmap =>
			{
				idmap.Property(y => y.Code, pm => pm.Length(15));
				idmap.Property(y => y.Name, pm => pm.Length(25));
			}));

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmCompositId = hbmClass.CompositeId;
			var keyProperties = hbmCompositId.Items.OfType<HbmKeyProperty>();
			keyProperties.Select(x => x.length).Should().Have.SameValuesAs("15", "25");
		}

		[Test]
		public void WhenMapAttributesOfCustomizedComponentUsedAsComponentAsIdWithCustomizationThenUseInComponentAsIdCustomization()
		{
			var mapper = new ModelMapper();
			mapper.Component<IMyCompo>(x =>
			{
				x.Access(Accessor.Field);
				x.Class<MyComponent>();
			});
			mapper.Class<MyClass>(map => map.ComponentAsId(x => x.Id));

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmCompositId = hbmClass.CompositeId;
			hbmCompositId.access.Should().Contain("field");
			hbmCompositId.@class.Should().Contain("MyComponent");
		}

		[Test]
		public void WhenMapAttributesOfCustomizedComponentUsedAsComponentAsIdWithCustomizationOverrideThenUseComponentAsIdCustomization()
		{
			var mapper = new ModelMapper();
			mapper.Component<IMyCompo>(x =>
			{
				x.Access(Accessor.Field);
				x.Class<MyComponent>();
			});
			mapper.Class<MyClass>(map => map.ComponentAsId(x => x.Id, idmap => idmap.Access(Accessor.NoSetter)));

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmCompositId = hbmClass.CompositeId;
			hbmCompositId.access.Should().Contain("nosetter");
			hbmCompositId.@class.Should().Contain("MyComponent");
		}
	}
}