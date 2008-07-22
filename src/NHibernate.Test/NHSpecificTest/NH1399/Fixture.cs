using NHibernate.Mapping;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1399
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void Bug()
		{
			Table table1 = new Table("ATABLE");

			Column table1ITestManyA = new Column("itestmanyaid");
			Column table1ITestManyB = new Column("itestmanybid");
			string t1Fk = table1.UniqueColumnString(new object[] { table1ITestManyA }, "BluewireTechnologies.Core.Framework.DynamicTypes2.Albatross.ITestManyA");
			string t2Fk = table1.UniqueColumnString(new object[] { table1ITestManyB }, "BluewireTechnologies.Core.Framework.DynamicTypes2.Albatross.ITestManyB");
			Assert.AreNotEqual(t1Fk, t2Fk, "Different columns in differents tables create the same FK name.");
		}
	}
}