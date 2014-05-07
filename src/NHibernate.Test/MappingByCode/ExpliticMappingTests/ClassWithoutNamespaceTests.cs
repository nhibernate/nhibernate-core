using System;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	[TestFixture]
	public class ClassWithoutNamespaceTests
	{
		[SetUp]
		public void OnSetUp()
		{
			Assert.That(typeof (EntityNH3615).Namespace, Is.Null);
		}

		[Test]
		public void ShouldBeAbleToMapClassWithoutNamespace()
		{
			var mapper = new ModelMapper();
			mapper.Class<EntityNH3615>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
			});

			Assert.DoesNotThrow(() => mapper.CompileMappingForAllExplicitlyAddedEntities());
		}
	}
}

internal class EntityNH3615
{
	public virtual Guid Id { get; set; }
	public virtual string Name { get; set; }
}
