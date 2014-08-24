using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ConventionModelMapperTests
{
	public class ComponetsAccessorTests
	{
		private class MyClass
		{
			public int Id { get; set; }
			private MyCompo compo;
			public MyCompo Compo
			{
				get { return compo; }
			}
		}

		private class MyCompo
		{
			public string Something { get; set; }
		}
		private HbmMapping GetMappingWithParentInCompo()
		{
			var mapper = new ConventionModelMapper();
			mapper.Class<MyClass>(x =>
			{
				x.Id(c => c.Id);
				x.Component(c => c.Compo);
			});
			mapper.Component<MyCompo>(x =>
			{
				x.Property(c => c.Something);
			});
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void WhenMapComoponetWithNestBidirectionalComponentThenMapParentAccessor()
		{
			HbmMapping mapping = GetMappingWithParentInCompo();

			var hbmClass = mapping.RootClasses[0];
			var hbmMyCompo = hbmClass.Properties.OfType<HbmComponent>().Single();

			hbmMyCompo.Access.Should().Contain("camelcase");
		}
	}
}