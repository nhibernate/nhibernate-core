using System;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH2738
{
	public enum MyEmptyEnum
	{
	}

	public enum WierdEnum
	{
		MinusOne = -1,
		Zero = 0,
		PlusOne = 1
	}

	public class MyClass
	{
		public virtual int Id { get; set; }
		public virtual MyEmptyEnum MyEmptyEnum { get; set; }
	}

	public class Fixture
	{
		[Test]
		public void WhenMapEmptyEnumThenDoesNotThrowExplicitException()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(rc =>
			{
				rc.Id(x => x.Id);
				rc.Property(x => x.MyEmptyEnum);
			});
			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
			var conf = TestConfigurationHelper.GetDefaultConfiguration();
			conf.AddMapping(mappings);
			Assert.That(() => conf.BuildSessionFactory(), Throws.Nothing);
		}

		[Test]
		public void DefaultOfWierdEnumIsZero()
		{
			Assert.That(Activator.CreateInstance(typeof (WierdEnum)), Is.EqualTo(WierdEnum.Zero));
			Assert.That(Enum.ToObject(typeof (WierdEnum), 0), Is.EqualTo(WierdEnum.Zero));
			Assert.That(Enum.GetValues(typeof (WierdEnum)).GetValue(0), Is.EqualTo(WierdEnum.Zero)); // This depends on implementation and can be wrong
		}
	}
}