using NHibernate.Mapping;
using NUnit.Framework;
using SharpTestsEx;

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

		[Test]
		public void UsingTwoInstancesWithSameValuesTheFkNameIsTheSame()
		{
			// This test is to be sure that an eventual SchemaUpdate will find the FK with the same name
			// The FK name should not use values depending on instence, istead should use values depending on table/columns names.
			Table table1 = new Table("ATABLE");

			Column table1ITestManyA = new Column("itestmanyaid");
			Column table1ITestManyB = new Column("itestmanybid");
			string t1Fk = table1.UniqueColumnString(new object[] { table1ITestManyA }, "BluewireTechnologies.Core.Framework.DynamicTypes2.Albatross.ITestManyA");
			string t2Fk = table1.UniqueColumnString(new object[] { table1ITestManyB }, "BluewireTechnologies.Core.Framework.DynamicTypes2.Albatross.ITestManyB");


			Table table1_ = new Table("ATABLE");

			Column table1ITestManyA_ = new Column("itestmanyaid");
			Column table1ITestManyB_ = new Column("itestmanybid");
			string t1Fk_ = table1_.UniqueColumnString(new object[] { table1ITestManyA_ }, "BluewireTechnologies.Core.Framework.DynamicTypes2.Albatross.ITestManyA");
			string t2Fk_ = table1_.UniqueColumnString(new object[] { table1ITestManyB_ }, "BluewireTechnologies.Core.Framework.DynamicTypes2.Albatross.ITestManyB");

			t1Fk_.Should().Be.EqualTo(t1Fk);
			t2Fk_.Should().Be.EqualTo(t2Fk);
		}
	}
}