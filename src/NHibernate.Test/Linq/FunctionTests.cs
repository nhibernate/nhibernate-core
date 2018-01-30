using System;
using System.Linq;
using System.Text.RegularExpressions;
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

		[Test]
		public void LikeFunctionWithEscapeCharacter()
		{
			using (var tx = session.BeginTransaction())
			{
				var employeeName = "Mar%aret";
				var escapeChar = '#';
				var employeeNameEscaped = employeeName.Replace("%", escapeChar + "%");

				//This entity will be flushed to the db, but rolled back when the test completes

				session.Save(new Employee { FirstName = employeeName, LastName = "LastName" });
				session.Flush();


				var query = (from e in db.Employees
				             where NHibernate.Linq.SqlMethods.Like(e.FirstName, employeeNameEscaped, escapeChar)
				             select e).ToList();

				Assert.That(query.Count, Is.EqualTo(1));
				Assert.That(query[0].FirstName, Is.EqualTo(employeeName));

				Assert.Throws<ArgumentException>(() =>
				{
					(from e in db.Employees
					 where NHibernate.Linq.SqlMethods.Like(e.FirstName, employeeNameEscaped, e.FirstName.First())
					 select e).ToList();
				});
				tx.Rollback();
			}
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

			// ReSharper disable RedundantNameQualifier
			// NOTE: Deliberately use full namespace for our SqlMethods class below, to reduce
			// risk of accidentally referencing NHibernate.Linq.SqlMethods.
			var query = (from e in db.Employees
						 where NHibernate.Test.Linq.FunctionTests.SqlMethods.Like(e.FirstName, "Ma%et")
						 select e).ToList();
			// ReSharper restore RedundantNameQualifier

			Assert.That(query.Count, Is.EqualTo(1));
			Assert.That(query[0].FirstName, Is.EqualTo("Margaret"));
		}

		[Test]
		public void SubstringFunction2()
		{
			var query = (from e in db.Employees
				where e.FirstName.Substring(0, 2) == "An"
				select e).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void SubstringFunction1()
		{
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
			var suppliedName = "Anne";
			var query = from e in db.Employees
						where e.FirstName.StartsWith("An")
						select new
							{
								Before = e.FirstName,
								// This one call the standard string.Replace, not the extension. The linq registry handles it.
								AfterMethod = e.FirstName.Replace("An", "Zan"),
								AfterExtension = ExtensionMethods.Replace(e.FirstName, "An", "Zan"),
								AfterNamedExtension = e.FirstName.ReplaceExtension("An", "Zan"),
								AfterEvaluableExtension = e.FirstName.ReplaceWithEvaluation("An", "Zan"),
								AfterEvaluable2Extension = e.FirstName.ReplaceWithEvaluation2("An", "Zan"),
							BeforeConst = suppliedName,
								// This one call the standard string.Replace, not the extension. The linq registry handles it.
								AfterMethodConst = suppliedName.Replace("An", "Zan"),
								AfterExtensionConst = ExtensionMethods.Replace(suppliedName, "An", "Zan"),
								AfterNamedExtensionConst = suppliedName.ReplaceExtension("An", "Zan"),
								AfterEvaluableExtensionConst = suppliedName.ReplaceWithEvaluation("An", "Zan"),
								AfterEvaluable2ExtensionConst = suppliedName.ReplaceWithEvaluation2("An", "Zan")
						};
			var results = query.ToList();
			var s = ObjectDumper.Write(results);

			foreach (var result in results)
			{
				var expectedDbResult = Regex.Replace(result.Before, "An", "Zan", RegexOptions.Compiled | RegexOptions.IgnoreCase);
				Assert.That(result.AfterMethod, Is.EqualTo(expectedDbResult), $"Wrong {nameof(result.AfterMethod)} value");
				Assert.That(result.AfterExtension, Is.EqualTo(expectedDbResult), $"Wrong {nameof(result.AfterExtension)} value");
				Assert.That(result.AfterNamedExtension, Is.EqualTo(expectedDbResult), $"Wrong {nameof(result.AfterNamedExtension)} value");
				Assert.That(result.AfterEvaluableExtension, Is.EqualTo(expectedDbResult), $"Wrong {nameof(result.AfterEvaluableExtension)} value");
				Assert.That(result.AfterEvaluable2Extension, Is.EqualTo(expectedDbResult), $"Wrong {nameof(result.AfterEvaluable2Extension)} value");

				var expectedDbResultConst = Regex.Replace(result.BeforeConst, "An", "Zan", RegexOptions.Compiled | RegexOptions.IgnoreCase);
				var expectedInMemoryResultConst = result.BeforeConst.Replace("An", "Zan");
				var expectedInMemoryExtensionResultConst = result.BeforeConst.ReplaceWithEvaluation("An", "Zan");
				Assert.That(result.AfterMethodConst, Is.EqualTo(expectedInMemoryResultConst), $"Wrong {nameof(result.AfterMethodConst)} value");
				Assert.That(result.AfterExtensionConst, Is.EqualTo(expectedDbResultConst), $"Wrong {nameof(result.AfterExtensionConst)} value");
				Assert.That(result.AfterNamedExtensionConst, Is.EqualTo(expectedDbResultConst), $"Wrong {nameof(result.AfterNamedExtensionConst)} value");
				Assert.That(result.AfterEvaluableExtensionConst, Is.EqualTo(expectedInMemoryExtensionResultConst), $"Wrong {nameof(result.AfterEvaluableExtensionConst)} value");
				Assert.That(result.AfterEvaluable2ExtensionConst, Is.EqualTo(expectedInMemoryExtensionResultConst), $"Wrong {nameof(result.AfterEvaluable2ExtensionConst)} value");
			}

			// Should cause ReplaceWithEvaluation to fail
			suppliedName = null;
			var failingQuery = from e in db.Employees
						where e.FirstName.StartsWith("An")
						select new
						{
							Before = e.FirstName,
							AfterEvaluableExtensionConst = suppliedName.ReplaceWithEvaluation("An", "Zan")
						};
			Assert.That(() => failingQuery.ToList(), Throws.InstanceOf<HibernateException>().And.InnerException.InstanceOf<ArgumentNullException>());
		}

		[Test]
		public void CharIndexFunction()
		{
			if (!TestDialect.SupportsLocate)
				Assert.Ignore("Locate function not supported.");

			var raw = (from e in db.Employees select e.FirstName).ToList();
			var expected = raw.Select(x => x.ToLower()).Where(x => x.IndexOf('a') == 0).ToList();

			var query = from e in db.Employees
						let lowerName = e.FirstName.ToLower()
						where lowerName.IndexOf('a') == 0
						select lowerName;
			var result = query.ToList();

			Assert.That(result, Is.EqualTo(expected), "Expected {0} but was {1}", string.Join("|", expected), string.Join("|", result));
			ObjectDumper.Write(query);
		}

		[Test]
		public void CharIndexOffsetNegativeFunction()
		{
			if (!TestDialect.SupportsLocate)
				Assert.Ignore("Locate function not supported.");

			var raw = (from e in db.Employees select e.FirstName).ToList();
			var expected = raw.Select(x => x.ToLower()).Where(x => x.IndexOf('a', 2) == -1).ToList();

			var query = from e in db.Employees
						let lowerName = e.FirstName.ToLower()
						where lowerName.IndexOf('a', 2) == -1
						select lowerName;
			var result = query.ToList();

			Assert.That(result, Is.EqualTo(expected), "Expected {0} but was {1}", string.Join("|", expected), string.Join("|", result));
			ObjectDumper.Write(query);
		}

		[Test]
		public void IndexOfFunctionExpression()
		{
			if (!TestDialect.SupportsLocate)
				Assert.Ignore("Locate function not supported.");

			var raw = (from e in db.Employees select e.FirstName).ToList();
			var expected = raw.Select(x => x.ToLower()).Where(x => x.IndexOf("an") == 0).ToList();

			var query = from e in db.Employees
						let lowerName = e.FirstName.ToLower()
						where lowerName.IndexOf("an") == 0
						select lowerName;
			var result = query.ToList();

			Assert.That(result, Is.EqualTo(expected), "Expected {0} but was {1}", string.Join("|", expected), string.Join("|", result));
			ObjectDumper.Write(query);
		}

		[Test]
		public void IndexOfFunctionProjection()
		{
			if (!TestDialect.SupportsLocate)
				Assert.Ignore("Locate function not supported.");

			var raw = (from e in db.Employees select e.FirstName).ToList();
			var expected = raw.Select(x => x.ToLower()).Where(x => x.Contains("a")).Select(x => x.IndexOf("a", 1)).ToList();

			var query = from e in db.Employees
						let lowerName = e.FirstName.ToLower()
						where lowerName.Contains("a")
						select lowerName.IndexOf("a", 1);
			var result = query.ToList();

			Assert.That(result, Is.EqualTo(expected), "Expected {0} but was {1}", string.Join("|", expected), string.Join("|", result));
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
			Assert.AreEqual(2, session.Query<AnotherEntity>().Count(e => (e.Input ?? "hello") == "hello"));
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

				Assert.AreEqual(2, session.Query<AnotherEntity>().Count(e => e.Input.Trim() == "hi"));
				Assert.AreEqual(1, session.Query<AnotherEntity>().Count(e => e.Input.TrimEnd() == " hi"));

				// Emulated trim does not support multiple trim characters, but for many databases it should work fine anyways.
				Assert.AreEqual(1, session.Query<AnotherEntity>().Count(e => e.Input.Trim('h') == "e"));
				Assert.AreEqual(1, session.Query<AnotherEntity>().Count(e => e.Input.TrimStart('h') == "eh"));
				Assert.AreEqual(1, session.Query<AnotherEntity>().Count(e => e.Input.TrimEnd('h') == "he"));

				// Check when passed as array (new overloads in .NET Core App 2.0).
				Assert.AreEqual(1, session.Query<AnotherEntity>().Count(e => e.Input.Trim(new [] { 'h' }) == "e"));
				Assert.AreEqual(1, session.Query<AnotherEntity>().Count(e => e.Input.TrimStart(new[] { 'h' }) == "eh"));
				Assert.AreEqual(1, session.Query<AnotherEntity>().Count(e => e.Input.TrimEnd(new[] { 'h' }) == "he"));

				// Let it rollback to get rid of temporary changes.
			}
		}

		[Test]
		public void TrimInitialWhitespace()
		{
			using (session.BeginTransaction())
			{
				session.Save(new AnotherEntity {Input = " hi"});
				session.Save(new AnotherEntity {Input = "hi"});
				session.Save(new AnotherEntity {Input = "heh"});
				session.Flush();

				Assert.That(session.Query<AnotherEntity>().Count(e => e.Input.TrimStart() == "hi"), Is.EqualTo(2));

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
		public void WhereBoolConditionEquals()
		{
			var query = from item in db.Role
						where item.IsActive.Equals(item.Name != null)
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
