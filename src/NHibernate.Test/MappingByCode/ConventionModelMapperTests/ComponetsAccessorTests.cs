using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ConventionModelMapperTests
{
	[TestFixture]
	public class ComponetsAccessorTests
	{
		private class MyClass
		{
			public int Id { get; set; }
			// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			private MyCompo compo;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
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

			Assert.That(hbmMyCompo.Access, Does.Contain("camelcase"));
		}
	}
}
