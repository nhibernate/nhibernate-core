using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	[TestFixture]
	public class KeyPropertyMapperTest
	{
		private class MyClass
		{
			public string ReadOnly { get { return ""; } }
		}

		[Test]
		public void WhenSetTypeByICompositeUserTypeThenSetTypeName()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmKeyProperty();
			var mapper = new KeyPropertyMapper(member, mapping);

			Assert.That(() => mapper.Type<MyCompoType>(), Throws.Nothing);
			Assert.That(mapping.Type.name, Does.Contain(nameof(MyCompoType)));
			Assert.That(mapping.type, Is.Null);
		}
	}
}
