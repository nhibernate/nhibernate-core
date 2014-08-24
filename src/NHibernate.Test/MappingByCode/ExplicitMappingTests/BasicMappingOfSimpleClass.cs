using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	public class BasicMappingOfSimpleClass
	{
		public class MyClass
		{
			public int Id { get; set; }
			public string Something { get; set; }
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
			hbmClass.Should().Not.Be.Null();
			var hbmId = hbmClass.Id;
			hbmId.Should().Not.Be.Null();
			hbmId.column1.Should().Be("MyClassId");
			hbmId.type1.Should().Be(NHibernateUtil.Int32.Name);
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
			hbmClass.Should().Not.Be.Null();
			var hbmId = hbmClass.Id;
			hbmId.Should().Not.Be.Null();
			hbmId.column1.Should().Be("MyClassId");
			hbmId.type1.Should().Not.Be.Null();
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
			hbmClass.Should().Not.Be.Null();
			var hbmId = hbmClass.Id;
			hbmId.Should().Not.Be.Null();
			hbmId.name.Should().Be("Id");
			var hbmGenerator = hbmId.generator;
			hbmGenerator.Should().Not.Be.Null();
			hbmGenerator.@class.Should().Be("hilo");
			hbmGenerator.param[0].name.Should().Be("max_low");
			hbmGenerator.param[0].GetText().Should().Be("100");
			var hbmProperty = hbmClass.Properties.OfType<HbmProperty>().Single();
			hbmProperty.name.Should().Be("Something");
			hbmProperty.length.Should().Be("150");
		}
	}
}