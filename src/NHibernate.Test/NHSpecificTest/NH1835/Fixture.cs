using NUnit.Framework;
using NHibernate.SqlTypes;

namespace NHibernate.Test.NHSpecificTest.NH1835
{
	[TestFixture]
	public class Fixture: BugTestCase
	{
		[Test]
		public void ColumnTypeBinaryBlob()
		{
			var pc = Sfi.GetEntityPersister(typeof (Document).FullName);
			var type = pc.GetPropertyType("Contents");
			Assert.That(type.SqlTypes(Sfi)[0], Is.InstanceOf<BinaryBlobSqlType>());
		}
	}
}