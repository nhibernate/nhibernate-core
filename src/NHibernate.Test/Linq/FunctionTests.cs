using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class FunctionTests : LinqTestCase
	{
		[Test]
		public void SubstringFunction()
		{
			var query = from e in db.Employees
						where e.FirstName.Substring(1, 2) == "An"
						select e;

			ObjectDumper.Write(query);
		}

		[Test]
		public void LeftFunction()
		{
			var query = from e in db.Employees
                        where e.FirstName.Substring(1, 2) == "An"
                        select e.FirstName.Substring(3);

			ObjectDumper.Write(query);
		}

		[Test]
		public void ReplaceFunction()
		{
			var query = from e in db.Employees
						where e.FirstName.StartsWith("An")
						select new
								{
									Before = e.FirstName,
									AfterMethod = e.FirstName.Replace("An", "Zan"),
                                    AfterExtension = ExtensionMethods.Replace(e.FirstName, "An", "Zan"),
                                    AfterExtension2 = e.FirstName.ReplaceExtension("An", "Zan")
								};

			var s = ObjectDumper.Write(query);
		}

		[Test]
		public void CharIndexFunction()
		{
			if (!TestDialect.SupportsLocate)
				Assert.Ignore("Locate function not supported.");

			var query = from e in db.Employees
                        where e.FirstName.IndexOf('A') == 1
						select e.FirstName;

			ObjectDumper.Write(query);
		}

		[Test]
		public void IndexOfFunctionExpression()
		{
			if (!TestDialect.SupportsLocate)
				Assert.Ignore("Locate function not supported.");

			var query = from e in db.Employees
						where e.FirstName.IndexOf("An") == 1
						select e.FirstName;

			ObjectDumper.Write(query);
		}

		[Test]
		public void IndexOfFunctionProjection()
		{
			var query = from e in db.Employees
						where e.FirstName.Contains("a")
						select e.FirstName.IndexOf('A', 3);

			ObjectDumper.Write(query);
		}

		[Test]
		public void TwoFunctionExpression()
		{
			if (!TestDialect.SupportsLocate)
				Assert.Ignore("Locate function not supported.");

			var query = from e in db.Employees
		                where e.FirstName.IndexOf("A") == e.BirthDate.Value.Month 
		    			select e.FirstName;

			ObjectDumper.Write(query);
		}

        [Test]
        public void ToStringFunction()
        {
            var query = from ol in db.OrderLines
                        where ol.Quantity.ToString() == "4"
                        select ol;

            Assert.AreEqual(55, query.Count());
        }

        [Test]
        public void ToStringWithContains()
        {
            var query = from ol in db.OrderLines
                        where ol.Quantity.ToString().Contains("5")
                        select ol;

            Assert.AreEqual(498, query.Count());
        }
    }
}