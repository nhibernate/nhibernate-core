using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using System.Linq;

namespace NHibernate.Test.NHSpecificTest.NH3491
{
	[TestFixture]
	public class FixtureByCode
	{
		class SomeComponent
		{
			public virtual string PropertyOne { get; set; }
			public virtual string PropertyTwo { get; set; }
			public virtual string PropertyThree { get; set; }
		}

		class ClassWithComponents
		{
			public virtual Guid Id { get; set; }

			public virtual SomeComponent Component1 { get; set; }

			public virtual SomeComponent Component2 { get; set; }
		}

		[Test(Description = "NH-3491"), KnownBug("NH-3491")]
		public void ShouldProperlyMapComponentWhenMappingOnlyPartOfItInSomePlaces()
		{
			var mapper = new ModelMapper();
			mapper.Class<ClassWithComponents>(cm =>
			{
				cm.Component(x => x.Component1, c =>
				{
					c.Property(x => x.PropertyOne, p => p.Column("OnePropertyOne"));
				});

				cm.Component(x => x.Component2, c =>
				{
					c.Property(x => x.PropertyOne, p => p.Column("TwoPropertyOne"));
					c.Property(x => x.PropertyTwo, p => p.Column("TwoPropertyTwo"));
				});
			});

			//Compile, and get the component property in which we mapped only one inner property
			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();

			var component1PropertyMapping = (HbmComponent)mappings.RootClasses[0].Properties.Single(x => x.Name == "Component1");

			//There should be only one inner property in the mapping of this component
			// Note: take a look at how CURRENTLY the test fails with 1 expected vs 2, instead of vs 3. 
			//       This means that the "PropertyThree" property of the component that was never mapped, is not taken into account (which is fine).
			Assert.That(component1PropertyMapping.Items.Length, Is.EqualTo(1));
		}
	}
}