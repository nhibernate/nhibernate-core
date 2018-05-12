using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Docs.Associations.BiM21
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] {"NHSpecificTest.Docs.Associations.BiM21.Mappings.hbm.xml"}; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return TestDialect.SupportsEmptyInsertsOrHasNonIdentityNativeGenerator;
		}

		[Test]
		public void TestCorrectUse()
		{
			ISession session = OpenSession();

			Person fred = new Person();
			Person wilma = new Person();

			Address flinstoneWay = new Address();

			fred.Address = flinstoneWay;
			wilma.Address = flinstoneWay;

			session.Save(flinstoneWay);
			session.Save(fred);
			session.Save(wilma);

			session.Close();

			// clean up
			session = OpenSession();

			session.Delete("from Person");
			session.Delete("from Address");
			session.Flush();
			session.Close();
		}

		[Test]
		public void TestErrorUsage()
		{
			using (ISession session = OpenSession())
			{
				Person fred = new Person();
				Person wilma = new Person();

				Address flinstoneWay = new Address();

				fred.Address = flinstoneWay;
				wilma.Address = flinstoneWay;

				Assert.Throws<PropertyValueException>(() => session.Save(fred));
			}
		}
	}
}
