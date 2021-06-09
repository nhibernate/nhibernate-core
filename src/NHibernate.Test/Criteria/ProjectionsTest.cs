using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.Criteria
{
	[TestFixture]
	public class ProjectionsTest : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get
			{
				return new string[]
					{
						"Criteria.Enrolment.hbm.xml",
						"Criteria.Animal.hbm.xml",
						"Criteria.MaterialResource.hbm.xml"
					};
			}
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			{
				ITransaction t = session.BeginTransaction();

				Student gavin = new Student();
				gavin.Name = "ayende";
				gavin.StudentNumber = 27;
				session.Save(gavin);

				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = Sfi.OpenSession())
			{
				session.Delete("from System.Object");
				session.Flush();
			}
		}

		[Test]
		public void UsingSqlFunctions_Concat()
		{
			if (TestDialect.HasBrokenTypeInferenceOnSelectedParameters)
				Assert.Ignore("Current dialect does not support this test");

			using (ISession session = Sfi.OpenSession())
			{
				string result = session.CreateCriteria(typeof(Student))
					.SetProjection(new SqlFunctionProjection("concat",
						NHibernateUtil.String,
						Projections.Property("Name"),
						new ConstantProjection(" "),
						Projections.Property("Name")
					))
					.UniqueResult<string>();
				Assert.AreEqual("ayende ayende", result);
			}
		}

		[Test]
		public void UsingSqlFunctions_Concat_WithCast()
		{
			if (Dialect is Oracle8iDialect)
			{
				Assert.Ignore("Not supported by the active dialect:{0}.", Dialect);
			}
			if (TestDialect.HasBrokenTypeInferenceOnSelectedParameters)
				Assert.Ignore("Current dialect does not support this test");

			using (ISession session = Sfi.OpenSession())
			{
				string result = session.CreateCriteria(typeof(Student))
					.SetProjection(Projections.SqlFunction("concat",
						NHibernateUtil.String,
						Projections.Cast(NHibernateUtil.String, Projections.Id()),
						Projections.Constant(" "),
						Projections.Property("Name")
					))
					.UniqueResult<string>();
				Assert.AreEqual("27 ayende", result);
			}
		}

		[Test]
		public void CastWithLength()
		{
			if (Regex.IsMatch(Dialect.GetCastTypeName(SqlTypeFactory.GetString(3)), @"^[^(]*$"))
			{
				Assert.Ignore($"Dialect {Dialect} does not seem to handle string length in cast");
			}

			using (var s = OpenSession())
			{
				try
				{
					var shortName = s
						.CreateCriteria<Student>()
						.SetProjection(
							Projections.Cast(
								TypeFactory.GetStringType(3),
								Projections.Property("Name")))
						.UniqueResult<string>();
					Assert.That(shortName, Is.EqualTo("aye"));
				}
				catch (Exception e)
				{
					if (e.InnerException == null || !e.InnerException.Message.Contains("truncation"))
						throw;
				}
			}
		}

		[Test]
		public void CastWithPrecisionScale()
		{
			if (TestDialect.HasBrokenDecimalType)
				Assert.Ignore("Dialect does not correctly handle decimal.");

			using (var s = OpenSession())
			{
				var value = s
					.CreateCriteria<Student>()
					.SetProjection(
						Projections.Cast(
							TypeFactory.Basic("decimal(18,9)"),
							Projections.Constant(123456789.123456789m, TypeFactory.Basic("decimal(18,9)"))))
					.UniqueResult<decimal>();
				Assert.That(value, Is.EqualTo(123456789.123456789m), "Same type cast");

				value = s
					.CreateCriteria<Student>()
					.SetProjection(
						Projections.Cast(
							TypeFactory.Basic("decimal(18,7)"),
							Projections.Constant(123456789.987654321m, TypeFactory.Basic("decimal(18,9)"))))
					.UniqueResult<decimal>();
				Assert.That(value, Is.EqualTo(123456789.9876543m), "Reduced scale cast");
			}
		}

		[Test]
		public void CanUseParametersWithProjections()
		{
			using (ISession session = Sfi.OpenSession())
			{
				long result = session.CreateCriteria(typeof(Student))
					.SetProjection(new AddNumberProjection("id", 15))
					.UniqueResult<long>();
				Assert.AreEqual(42L, result);
			}
		}

		[Test]
		public void UsingConditionals()
		{
			if (TestDialect.HasBrokenTypeInferenceOnSelectedParameters)
				Assert.Ignore("Current dialect does not support this test");

			using (ISession session = Sfi.OpenSession())
			{
				string result = session.CreateCriteria(typeof(Student))
					.SetProjection(
						Projections.Conditional(
							Expression.Eq("id", 27L),
							Projections.Constant("yes"),
							Projections.Constant("no"))
					)
					.UniqueResult<string>();
				Assert.AreEqual("yes", result);

				result = session.CreateCriteria(typeof(Student))
					.SetProjection(
						Projections.Conditional(
							Expression.Eq("id", 42L),
							Projections.Constant("yes"),
							Projections.Constant("no"))
					)
					.UniqueResult<string>();
				Assert.AreEqual("no", result);
			}
		}

		[Test]
		public void UsingMultiConditionals()
		{
			if (TestDialect.HasBrokenTypeInferenceOnSelectedParameters)
				Assert.Ignore("Current dialect does not support this test");

			var students = new[]
			{
				new Student() { StudentNumber = 6L, Name = "testa", },
				new Student() { StudentNumber = 5L, Name = "testz", },
				new Student() { StudentNumber = 4L, Name = "test1", },
				new Student() { StudentNumber = 3L, Name = "test2", },
				new Student() { StudentNumber = 2L, Name = "test998", },
				new Student() { StudentNumber = 1L, Name = "test999", },
			};

			var expecteds = new[]
			{
				students[0],
				students[1],
				students[2],
				students[3],
			};

			// student, sortingindex
			var testData = new Tuple<Student, string>[]
			{
				System.Tuple.Create(expecteds[0], "1"),
				System.Tuple.Create(expecteds[1], "2"),
				System.Tuple.Create(expecteds[2], "3"),
				System.Tuple.Create(expecteds[3], "4"),
			};

			using (ISession session = this.Sfi.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					session.Save<Student>(students);
					transaction.Commit();
				}

				using (ITransaction transaction = session.BeginTransaction())
				{
					// when Name = "testa" then 1 ...
					var criterionProjections = testData
						.Select(x => new ConditionalCriterionProjectionPair(Expression.Eq(nameof(Student.Name), x.Item1.Name), Projections.Constant(x.Item2)))
						.ToArray();

					// ... else 99
					var elseProjection = Projections.Constant("99");

					var conditionalsProjection = Projections.Conditionals(criterionProjections, elseProjection);

					var order = Order.Asc(conditionalsProjection);

					var criteria = session.CreateCriteria(typeof(Student))
						.AddOrder(order);

					var actuals = criteria.List<Student>();

					Assert.GreaterOrEqual(actuals.Count, expecteds.Length);
					for (int i = 0; i < expecteds.Length; i++)
					{
						var expected = expecteds[i];
						var actual = actuals[i];

						Assert.AreEqual(expected.Name, actual.Name);
					}
				}

				using (ITransaction transaction = session.BeginTransaction())
				{
					session.Delete<Student>(students);
					transaction.Commit();
				}
			}
		}

		[Test]
		public void UseInWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = session.CreateCriteria(typeof(Student))
					.Add(Expression.In(Projections.Id(), new object[] { 27 }))
					.List<Student>();
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public void UseLikeWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = session.CreateCriteria(typeof(Student))
					.Add(Expression.Like(Projections.Property("Name"), "aye", MatchMode.Start))
					.List<Student>();
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public void UseInsensitiveLikeWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = session.CreateCriteria(typeof(Student))
					.Add(Expression.InsensitiveLike(Projections.Property("Name"), "AYE", MatchMode.Start))
					.List<Student>();
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public void UseIdEqWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = session.CreateCriteria(typeof(Student))
					.Add(Expression.IdEq(Projections.Id()))
					.List<Student>();
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public void UseEqWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = session.CreateCriteria(typeof(Student))
					.Add(Expression.Eq(Projections.Id(), 27L))
					.List<Student>();
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public void UseGtWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = session.CreateCriteria(typeof(Student))
					.Add(Expression.Gt(Projections.Id(), 2L))
					.List<Student>();
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public void UseLtWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = session.CreateCriteria(typeof(Student))
					.Add(Expression.Lt(Projections.Id(), 200L))
					.List<Student>();
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public void UseLeWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = session.CreateCriteria(typeof(Student))
					.Add(Expression.Le(Projections.Id(), 27L))
					.List<Student>();
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public void UseGeWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = session.CreateCriteria(typeof(Student))
					.Add(Expression.Ge(Projections.Id(), 27L))
					.List<Student>();
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public void UseBetweenWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = session.CreateCriteria(typeof(Student))
					.Add(Expression.Between(Projections.Id(), 10L, 28L))
					.List<Student>();
				Assert.AreEqual(27L, list[0].StudentNumber);
			}
		}

		[Test]
		public void UseIsNullWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = session.CreateCriteria(typeof(Student))
					.Add(Expression.IsNull(Projections.Id()))
					.List<Student>();
				Assert.AreEqual(0, list.Count);
			}
		}

		[Test]
		public void UseIsNotNullWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = session.CreateCriteria(typeof(Student))
					.Add(Expression.IsNotNull(Projections.Id()))
					.List<Student>();
				Assert.AreEqual(1, list.Count);
			}
		}

		[Test]
		public void UseEqPropertyWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = session.CreateCriteria(typeof(Student))
					.Add(Expression.EqProperty(Projections.Id(), Projections.Id()))
					.List<Student>();
				Assert.AreEqual(1, list.Count);
			}
		}

		[Test]
		public void UseGePropertyWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = session.CreateCriteria(typeof(Student))
					.Add(Expression.GeProperty(Projections.Id(), Projections.Id()))
					.List<Student>();
				Assert.AreEqual(1, list.Count);
			}
		}

		[Test]
		public void UseGtPropertyWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = session.CreateCriteria(typeof(Student))
					.Add(Expression.GtProperty(Projections.Id(), Projections.Id()))
					.List<Student>();
				Assert.AreEqual(0, list.Count);
			}
		}

		[Test]
		public void UseLtPropertyWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = session.CreateCriteria(typeof(Student))
					.Add(Expression.LtProperty(Projections.Id(), Projections.Id()))
					.List<Student>();
				Assert.AreEqual(0, list.Count);
			}
		}

		[Test]
		public void UseLePropertyWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = session.CreateCriteria(typeof(Student))
					.Add(Expression.LeProperty(Projections.Id(), Projections.Id()))
					.List<Student>();
				Assert.AreEqual(1, list.Count);
			}
		}

		[Test]
		public void UseNotEqPropertyWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IList<Student> list = session.CreateCriteria(typeof(Student))
					.Add(Expression.NotEqProperty("id", Projections.Id()))
					.List<Student>();
				Assert.AreEqual(0, list.Count);
			}
		}

		[Test]
		public void UseSumWithNullResultWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				long sum = session.CreateCriteria(typeof(Reptile))
					.SetProjection(Projections.Sum(Projections.Id()))
					.UniqueResult<long>();
				Assert.AreEqual(0, sum);
			}
		}

		[Test]
		public void UseSubquerySumWithNullResultWithProjection()
		{
			using (ISession session = Sfi.OpenSession())
			{
				int sum = session.CreateCriteria(typeof(Enrolment))
					.CreateCriteria("Student", "s")
					.SetProjection(Projections.Sum(Projections.SqlFunction("length", NHibernateUtil.Int32, Projections.Property("s.Name"))))
					.UniqueResult<int>();
				Assert.AreEqual(0, sum);
			}
		}
	}
}
