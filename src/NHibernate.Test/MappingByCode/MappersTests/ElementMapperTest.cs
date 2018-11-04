using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	[TestFixture]
	public class ElementMapperTest
	{
		[Test]
		public void WhenSetTypeByICompositeUserTypeThenSetTypeName()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(object), mapping);

			Assert.That(() => mapper.Type<MyCompoType>(), Throws.Nothing);
			Assert.That(mapping.Type.name, Does.Contain(nameof(MyCompoType)));
			Assert.That(mapping.type, Is.Null);
		}
	}
}
