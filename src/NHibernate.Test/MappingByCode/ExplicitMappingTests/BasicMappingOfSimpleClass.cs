using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ExplicitMappingTests
{
	public class BasicMappingOfSimpleClass
	{
		public class MyClass
		{
			public int Id { get; set; }
			public string Something { get; set; }
		}

		[Test]
		public void AbstractClass()
		{
			//NH-3527
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(ca =>
			{
				ca.Abstract(true);
				ca.Id(x => x.Id, map =>
				{
					map.Column("MyClassId");
					map.Generator(Generators.HighLow, gmap => gmap.Params(new { max_low = 100 }));
				});
				ca.Property(x => x.Something, map => map.Length(150));
			});
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			Assert.AreEqual(hbmMapping.RootClasses[0].@abstract, true);
		}

		[Test]
		public void MapClassWithIdAndProperty()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(ca =>
			{
				ca.Id(x => x.Id, map =>
				{
					map.Column("MyClassId");
					map.Generator(Generators.HighLow, gmap => gmap.Params(new { max_low = 100 }));
				});
				ca.Property(x => x.Something, map => map.Length(150));
			});
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			ModelIsWellFormed(hbmMapping);
		}

		[Test]
		public void MapClassWithIdAndPropertyWithParamsDictionary()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(ca =>
			{
				ca.Id(x => x.Id, map =>
				{
					map.Column("MyClassId");
					//NH-3415
					map.Generator(Generators.HighLow, gmap => gmap.Params(new Dictionary<string, object> { { "max_low", 100 } }));
				});
				ca.Property(x => x.Something, map => map.Length(150));
			});
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			ModelIsWellFormed(hbmMapping);
		}

		[Test]
		public void WhenMapClassWithoutIdThenApplyTypeOfGeneratorDef()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(ca => ca.Id(null, map =>
													{
														map.Column("MyClassId");
														map.Generator(Generators.HighLow, gmap => gmap.Params(new { max_low = 100 }));
													}));
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var hbmClass = hbmMapping.RootClasses[0];
			Assert.That(hbmClass, Is.Not.Null);
			var hbmId = hbmClass.Id;
			Assert.That(hbmId, Is.Not.Null);
			Assert.That(hbmId.column1, Is.EqualTo("MyClassId"));
			Assert.That(hbmId.type1, Is.EqualTo(NHibernateUtil.Int32.Name));
		}

		[Test]
		public void WhenMapClassWithoutIdAndWithoutGeneratorThenTypeShouldHaveValue()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(ca => ca.Id(null, map =>
			{
				map.Column("MyClassId");
			}));
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var hbmClass = hbmMapping.RootClasses[0];
			Assert.That(hbmClass, Is.Not.Null);
			var hbmId = hbmClass.Id;
			Assert.That(hbmId, Is.Not.Null);
			Assert.That(hbmId.column1, Is.EqualTo("MyClassId"));
			Assert.That(hbmId.type1, Is.Not.Null);
		}

		[Test]
		public void WhenDuplicatePropertiesDoesNotDuplicateMapping()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(ca =>
			{
				ca.Id(x => x.Id, map =>
				{
					map.Column("MyClassId");
				});
				ca.Id(x => x.Id, map =>
				{
					map.Generator(Generators.HighLow, gmap => gmap.Params(new { max_low = 100 }));
				});
				ca.Property(x => x.Something);
				ca.Property(x => x.Something, map => map.Length(150));
			});
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			ModelIsWellFormed(hbmMapping);
		}

		[Test]
		public void WhenDuplicateClassDoesNotDuplicateMapping()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(ca =>
			{
				ca.Id(x => x.Id, map =>
				{
					map.Generator(Generators.HighLow, gmap => gmap.Params(new { max_low = 100 }));
				});
				ca.Property(x => x.Something);
			});

			mapper.Class<MyClass>(ca =>
			{
				ca.Id(x => x.Id, map =>
				{
					map.Column("MyClassId");
				});
				ca.Property(x => x.Something, map => map.Length(150));
			});
			var hbmMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
			ModelIsWellFormed(hbmMapping);
		}

		private void ModelIsWellFormed(HbmMapping hbmMapping)
		{
			var hbmClass = hbmMapping.RootClasses[0];
			Assert.That(hbmClass, Is.Not.Null);
			var hbmId = hbmClass.Id;
			Assert.That(hbmId, Is.Not.Null);
			Assert.That(hbmId.name, Is.EqualTo("Id"));
			var hbmGenerator = hbmId.generator;
			Assert.That(hbmGenerator, Is.Not.Null);
			Assert.That(hbmGenerator.@class, Is.EqualTo("hilo"));
			Assert.That(hbmGenerator.param[0].name, Is.EqualTo("max_low"));
			Assert.That(hbmGenerator.param[0].GetText(), Is.EqualTo("100"));
			var hbmProperty = hbmClass.Properties.OfType<HbmProperty>().Single();
			Assert.That(hbmProperty.name, Is.EqualTo("Something"));
			Assert.That(hbmProperty.length, Is.EqualTo("150"));
		}
	}
}