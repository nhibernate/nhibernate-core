using System.Linq;
using System.Collections.Generic;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Linq;
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
			if (!TestDialect.SupportsLocate)
				Assert.Ignore("Locate function not supported.");
				
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

        [Test]
        public void Coalesce()
        {
            Assert.AreEqual(2, session.Query<AnotherEntity>().Where(e => (e.Input ?? "hello") == "hello").Count());
        }

        [Test]
        public void Trim()
        {
            List<int> idsToDelete = new List<int>();
            try
            {
                AnotherEntity ae1 = new AnotherEntity {Input = " hi "};
                AnotherEntity ae2 = new AnotherEntity {Input = "hi"};
                AnotherEntity ae3 = new AnotherEntity {Input = "heh"};
                session.Save(ae1);
                idsToDelete.Add(ae1.Id);
                session.Save(ae2);
                idsToDelete.Add(ae2.Id);
                session.Save(ae3);
                idsToDelete.Add(ae3.Id);
                session.Flush();

                Assert.AreEqual(2, session.Query<AnotherEntity>().Where(e => e.Input.Trim() == "hi").Count());
                Assert.AreEqual(1, session.Query<AnotherEntity>().Where(e => e.Input.TrimEnd() == " hi").Count());

                // Emulated trim does not support multiple trim characters, but for many databases it should work fine anyways.
                Assert.AreEqual(1, session.Query<AnotherEntity>().Where(e => e.Input.Trim('h') == "e").Count());
                Assert.AreEqual(1, session.Query<AnotherEntity>().Where(e => e.Input.TrimStart('h') == "eh").Count());
                Assert.AreEqual(1, session.Query<AnotherEntity>().Where(e => e.Input.TrimEnd('h') == "he").Count());
            }
            finally
            {
                foreach (int idToDelete in idsToDelete)
                    session.Delete(session.Get<AnotherEntity>(idToDelete));
                session.Flush();
            }
        }

		[Test, Ignore()]
		public void TrimTrailingWhitespace()
		{
			try
			{
				session.Save(new AnotherEntity {Input = " hi "});
				session.Save(new AnotherEntity {Input = "hi"});
				session.Save(new AnotherEntity {Input = "heh"});
				session.Flush();

				Assert.AreEqual(TestDialect.IgnoresTrailingWhitespace ? 2 : 1, session.Query<AnotherEntity>().Where(e => e.Input.TrimStart() == "hi ").Count());
			}
			finally
			{
				session.Delete("from AnotherEntity e where e.Id > 5");
				session.Flush();
			}
		}
	}
}