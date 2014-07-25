using System;
using System.Linq;
using System.Collections.Generic;
using NHibernate.Dialect;
using NHibernate.DomainModel;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class FunctionTests : LinqTestCase
	{
		[Test]
		public void LikeFunction()
		{
			var query = (from e in db.Employees
						 where NHibernate.Linq.SqlMethods.Like(e.FirstName, "Ma%et")
						 select e).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
			Assert.That(query[0].FirstName, Is.EqualTo("Margaret"));
		}

		private static class SqlMethods
		{
			public static bool Like(string expression, string pattern)
			{
				throw new NotImplementedException();
			}
		}

		[Test]
		public void LikeFunctionUserDefined()
		{
			// Verify that any method named Like, in a class named SqlMethods, will be translated.

			var query = (from e in db.Employees
						 where NHibernate.Test.Linq.FunctionTests.SqlMethods.Like(e.FirstName, "Ma%et")
						 select e).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
			Assert.That(query[0].FirstName, Is.EqualTo("Margaret"));
		}

		[Test]
		public void SubstringFunction2()
		{
			if (Dialect is FirebirdDialect)
				Assert.Ignore("Firebird before 2.0 only support integer literals for substring() - NH generates parameters.");

			var query = (from e in db.Employees
						 where e.FirstName.Substring(0, 2) == "An"
						 select e).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void SubstringFunction1()
		{
			if (Dialect is FirebirdDialect)
				Assert.Ignore("Firebird before 2.0 only support integer literals for substring() - NH generates parameters.");

			var query = (from e in db.Employees
						 where e.FirstName.Substring(3) == "rew"
						 select e).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
			Assert.That(query[0].FirstName, Is.EqualTo("Andrew"));
		}

		[Test]
		public void LeftFunction()
		{
			var query = (from e in db.Employees
						 where e.FirstName.Substring(0, 2) == "An"
						 select e.FirstName.Substring(3)).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
			Assert.That(query[0], Is.EqualTo("rew")); //Andrew
			Assert.That(query[1], Is.EqualTo("e")); //Anne
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
			using (session.BeginTransaction())
			{
				AnotherEntity ae1 = new AnotherEntity {Input = " hi "};
				AnotherEntity ae2 = new AnotherEntity {Input = "hi"};
				AnotherEntity ae3 = new AnotherEntity {Input = "heh"};
				session.Save(ae1);
				session.Save(ae2);
				session.Save(ae3);
				session.Flush();

				Assert.AreEqual(2, session.Query<AnotherEntity>().Where(e => e.Input.Trim() == "hi").Count());
				Assert.AreEqual(1, session.Query<AnotherEntity>().Where(e => e.Input.TrimEnd() == " hi").Count());

				// Emulated trim does not support multiple trim characters, but for many databases it should work fine anyways.
				Assert.AreEqual(1, session.Query<AnotherEntity>().Where(e => e.Input.Trim('h') == "e").Count());
				Assert.AreEqual(1, session.Query<AnotherEntity>().Where(e => e.Input.TrimStart('h') == "eh").Count());
				Assert.AreEqual(1, session.Query<AnotherEntity>().Where(e => e.Input.TrimEnd('h') == "he").Count());

				// Let it rollback to get rid of temporary changes.
			}
		}

		[Test, Ignore]
		public void TrimTrailingWhitespace()
		{
			using (session.BeginTransaction())
			{
				session.Save(new AnotherEntity {Input = " hi "});
				session.Save(new AnotherEntity {Input = "hi"});
				session.Save(new AnotherEntity {Input = "heh"});
				session.Flush();

				Assert.AreEqual(TestDialect.IgnoresTrailingWhitespace ? 2 : 1, session.Query<AnotherEntity>().Where(e => e.Input.TrimStart() == "hi ").Count());

				// Let it rollback to get rid of temporary changes.
			}
		}

		[Test]
		public void WhereStringEqual()
		{
			var query = (from item in db.Users
						 where item.Name.Equals("ayende")
						 select item).ToList();
			ObjectDumper.Write(query);
		}

		[Test, Description("NH-3367")]
		public void WhereStaticStringEqual()
		{
			var query = (from item in db.Users
						 where string.Equals(item.Name, "ayende")
						 select item).ToList();
			ObjectDumper.Write(query);
		}

		[Test]
		public void WhereIntEqual()
		{
			var query = (from item in db.Users
						 where item.Id.Equals(-1)
						 select item).ToList();

			ObjectDumper.Write(query);
		}

		[Test]
		public void WhereShortEqual()
		{
			var query = from item in session.Query<Foo>()
						where item.Short.Equals(-1)
						select item;

			ObjectDumper.Write(query);
		}

		[Test]
		public void WhereBoolConstantEqual()
		{
			var query = from item in db.Role
						where item.IsActive.Equals(true)
						select item;
			
			ObjectDumper.Write(query);
		}

		[Test]
		public void WhereBoolParameterEqual()
		{
			var query = from item in db.Role
						where item.IsActive.Equals(1 == 1)
						select item;
			
			ObjectDumper.Write(query);
		}

		[Test]
		public void WhereBoolFuncEqual()
		{
			Func<bool> f = () => 1 == 1;

			var query = from item in db.Role
						where item.IsActive.Equals(f())
						select item;

			ObjectDumper.Write(query);
		}

		[Test]
		public void WhereLongEqual()
		{
			var query = from item in db.PatientRecords
						 where item.Id.Equals(-1)
						 select item;

			ObjectDumper.Write(query);
		}

		[Test]
		public void WhereDateTimeEqual()
		{
			var query = from item in db.Users
						where item.RegisteredAt.Equals(DateTime.Today)
						select item;

			ObjectDumper.Write(query);
		}
		
		[Test]
		public void WhereGuidEqual()
		{
			var query = from item in db.Shippers
						where item.Reference.Equals(Guid.Empty)
						select item;

			ObjectDumper.Write(query);
		}		

		[Test]
		public void WhereDoubleEqual()
		{
			var query = from item in db.Animals
						where item.BodyWeight.Equals(-1)
						select item;

			ObjectDumper.Write(query);
		}	
	
		[Test]
		public void WhereFloatEqual()
		{
			var query = from item in session.Query<Foo>()
						where item.Float.Equals(-1)
						select item;

			ObjectDumper.Write(query);
		}	

		[Test]
		public void WhereCharEqual()
		{
			var query = from item in session.Query<Foo>()
						where item.Char.Equals('A')
						select item;

			ObjectDumper.Write(query);
		}

		[Test]
		public void WhereByteEqual()
		{
			var query = from item in session.Query<Foo>()
						where item.Byte.Equals(1)
						select item;

			ObjectDumper.Write(query);
		}

		[Test]
		public void WhereDecimalEqual()
		{
			var query = from item in db.OrderLines
						where item.Discount.Equals(-1)
						select item;

			ObjectDumper.Write(query);
		}
	}
}