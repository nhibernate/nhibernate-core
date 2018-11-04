using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	[TestFixture]
	public class MapKeyMapperTest
	{
		[Test]
		public void WhenSetTypeByICompositeUserTypeThenSetTypeName()
		{
			var mapping = new HbmMapKey();
			var mapper = new MapKeyMapper(mapping);

			Assert.That(() => mapper.Type<MyCompoType>(), Throws.Nothing);
			Assert.That(mapping.Type.name, Does.Contain(nameof(MyCompoType)));
		}
	}
}
