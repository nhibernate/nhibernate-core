using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	public class ColumnsNamingConvetions
	{
		public class MyClass
		{
			public int Id { get; set; }
			public string Something { get; set; }
		}

		[Test]
		public void MapClassWithConventions()
		{
			var mapper = new ModelMapper();

			mapper.BeforeMapClass += 
				(mi, t, map) => map.Id(x => x.Column((t.Name+"id").ToUpper()));
			mapper.BeforeMapProperty += 
				(mi, propertyPath, map) => map.Column(propertyPath.ToColumnName().ToUpper());

			mapper.Class<MyClass>(ca =>
			{
				ca.Id(x => x.Id, map => { });
				ca.Property(x => x.Something);
			});
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			var hbmClass = hbmMapping.RootClasses[0];
			hbmClass.Should().Not.Be.Null();
			var hbmId = hbmClass.Id;
			hbmId.column1.Should().Be("MYCLASSID");
			var hbmProperty = hbmClass.Properties.OfType<HbmProperty>().Single();
			hbmProperty.column.Should().Be("SOMETHING");
		}

		[Test]
		public void MapClassWithHardConventions()
		{
			var mapper = new ModelMapper();

			mapper.AfterMapClass +=
				(mi, t, map) => map.Id(x => x.Column((t.Name + "id").ToUpper()));
			mapper.AfterMapProperty +=
				(mi, propertyPath, map) => map.Column(propertyPath.ToColumnName().ToUpper());

			mapper.Class<MyClass>(ca =>
			{
				ca.Id(x => x.Id, map => map.Column("Whatever"));
				ca.Property(x => x.Something, map => map.Column("Whatever"));
			});
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			var hbmClass = hbmMapping.RootClasses[0];
			hbmClass.Should().Not.Be.Null();
			var hbmId = hbmClass.Id;
			hbmId.column1.Should().Be("MYCLASSID");
			var hbmProperty = hbmClass.Properties.OfType<HbmProperty>().Single();
			hbmProperty.column.Should().Be("SOMETHING");
		}
	}
}