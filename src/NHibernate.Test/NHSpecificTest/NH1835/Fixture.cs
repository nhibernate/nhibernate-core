using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1835
{
	[TestFixture]
	public class Fixture: BugTestCase
	{
		[Test, Ignore("Not fixed yet.")]
		public void ColumnTypeBinaryBlob()
		{
			var pc = sessions.GetEntityPersister(typeof (Document).FullName);
			var type = pc.GetPropertyType("Contents");
			Assert.That(type.SqlTypes(sessions)[0].Length, Is.EqualTo(3000));
		}
	}
}