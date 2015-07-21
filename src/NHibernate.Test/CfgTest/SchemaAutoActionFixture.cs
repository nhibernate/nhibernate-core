using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.CfgTest
{
	[TestFixture]
	public class SchemaAutoActionFixture
	{
		[Test]
		public void Equality()
		{
			Assert.That(SchemaAutoAction.Recreate.Equals("create-drop"));
			Assert.That(SchemaAutoAction.Recreate == "create-drop");
			Assert.That(SchemaAutoAction.Create.Equals("create"));
			Assert.That(SchemaAutoAction.Create == "create");
			Assert.That(SchemaAutoAction.Update.Equals("update"));
			Assert.That(SchemaAutoAction.Update == "update");
			Assert.That(SchemaAutoAction.Validate.Equals("validate"));
			Assert.That(SchemaAutoAction.Validate == "validate");
		}
	}
}