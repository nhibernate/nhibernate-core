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

			string s = ObjectDumper.Write(query);
		}

		[Test]
		public void CharIndexFunction()
		{
			var query = from e in db.Employees
                        where e.FirstName.IndexOf('A') == 1
						select e.FirstName;

			ObjectDumper.Write(query);
		}

		[Test]
		public void IndexOfFunctionExpression()
		{
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
		    var query = from e in db.Employees
		                where e.FirstName.IndexOf("A") == e.BirthDate.Value.Month 
		    			select e.FirstName;

			ObjectDumper.Write(query);
		}
	}
}