using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	public class ComposedIdTests
	{
		private class MyClass
		{
			public int Code { get; set; }
			private MyOther relation;
			public MyOther Relation
			{
				get { return relation; }
				set { relation = value; }
			}
		}

		private class MyOther
		{
			public int Id { get; set; }
		}

		private class MyOtherSubclass : MyOther
		{
			public int SubId { get; set; }
		}

		[Test]
		public void WhenPropertyUsedAsComposedIdThenRegister()
		{
			var inspector = new ExplicitlyDeclaredModel();
			var mapper = new ModelMapper(inspector);
			mapper.Class<MyClass>(map =>
															map.ComposedId(cm=>
															               {
																							 cm.Property(x => x.Code);
																							 cm.ManyToOne(x => x.Relation);
															               })
			                      );

			inspector.IsMemberOfComposedId(For<MyClass>.Property(x => x.Code)).Should().Be.True();
			inspector.IsMemberOfComposedId(For<MyClass>.Property(x => x.Relation)).Should().Be.True();
			inspector.IsPersistentProperty(For<MyClass>.Property(x => x.Code)).Should().Be.True();
			inspector.IsPersistentProperty(For<MyClass>.Property(x => x.Relation)).Should().Be.True();
			inspector.IsPersistentId(For<MyClass>.Property(x => x.Code)).Should().Be.False();
			inspector.IsPersistentId(For<MyClass>.Property(x => x.Relation)).Should().Be.False();
		}

		[Test]
		public void WhenPropertyUsedAsComposedIdThenNotUsedAsSimpleProperties()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(map =>
															map.ComposedId(cm =>
															{
																cm.Property(x => x.Code);
																cm.ManyToOne(x => x.Relation);
															})
														);
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmCompositId = hbmClass.CompositeId;
			hbmCompositId.Items.Should().Have.Count.EqualTo(2);
			hbmClass.Properties.Should().Be.Empty();
		}

		[Test]
		public void WhenPropertyUsedAsComposedIdAndPropertiesThenNotUsedAsSimpleProperties()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(map =>
			                      {
			                      	map.ComposedId(cm =>
			                      	               {
			                      	               	cm.Property(x => x.Code);
			                      	               	cm.ManyToOne(x => x.Relation);
			                      	               });
			                      	map.Property(x => x.Code);
			                      	map.ManyToOne(x => x.Relation);
			                      }
				);
			HbmMapping hbmMapping = mapper.CompileMappingFor(new[] {typeof (MyClass)});
			HbmClass hbmClass = hbmMapping.RootClasses[0];
			HbmCompositeId hbmCompositId = hbmClass.CompositeId;
			hbmCompositId.Items.Should().Have.Count.EqualTo(2);
			hbmClass.Properties.Should().Be.Empty();
		}

		[Test]
		public void WhenPropertyUsedAsComposedIdAndPropertiesAndNaturalIdThenMapOnlyAsComposedId()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(map =>
			{
				map.ComposedId(cm =>
				{
					cm.Property(x => x.Code);
					cm.ManyToOne(x => x.Relation);
				});
				map.NaturalId(nm =>
				              {
												nm.Property(x => x.Code);
												nm.ManyToOne(x => x.Relation);
				              });
				map.Property(x => x.Code);
				map.ManyToOne(x => x.Relation);
			}
				);
			HbmMapping hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			HbmClass hbmClass = hbmMapping.RootClasses[0];
			HbmCompositeId hbmCompositId = hbmClass.CompositeId;
			hbmCompositId.Items.Should().Have.Count.EqualTo(2);
			hbmClass.naturalid.Should().Be.Null();
			hbmClass.Properties.Should().Be.Empty();
		}

		[Test]
		public void WhenSuperclassPropertyUsedAsComposedIdThenRegister()
		{
			var inspector = new ExplicitlyDeclaredModel();
			var mapper = new ModelMapper(inspector);
			mapper.Class<MyOtherSubclass>(map =>
				map.ComposedId(cm =>
				{
					cm.Property(x => x.Id);
					cm.Property(x => x.SubId);
				})
			);
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyOtherSubclass) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmCompositeId = hbmClass.CompositeId;
			hbmCompositeId.Items.Should().Have.Count.EqualTo(2);
		}

	}
}