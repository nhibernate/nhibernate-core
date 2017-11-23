using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.SqlCommand;
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

		protected override IList Mappings
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

		private void  PrepareDataForEntityProjectionTests(out Student gavin, out string courseCode)
		{
			courseCode = "HIB";
			using (ISession session = Sfi.OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				Course course = new Course
				{
					CourseCode = "HIB",
					Description = "Hibernate Training"
				};
				session.Save(course);

				gavin = new Student
				{
					Name = "Gavin King",
					StudentNumber = 667,
					
				};
				session.Save(gavin);

				Enrolment enrolment = new Enrolment
				{
					Course = course,
					CourseCode = course.CourseCode,
					Semester = 1,
					Year = 1999,
					Student = gavin,
					StudentNumber = gavin.StudentNumber
				};
				gavin.Enrolments.Add(enrolment);
				session.Save(enrolment);

				session.Flush();
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = Sfi.OpenSession())
			{
				session.Delete($"from {typeof(Enrolment).FullName}");
				session.Delete("from System.Object");
				session.Flush();
			}
		}

		[Test]
		public void UsingSqlFunctions_Concat()
		{
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
			if(Dialect is Oracle8iDialect)
			{
				Assert.Ignore("Not supported by the active dialect:{0}.", Dialect);
			}
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

		[Test]
		public void UseRootProjection_Eager()
		{
			//NH-3435
			PrepareDataForEntityProjectionTests(out Student gavin, out string _);

			using (ISession session = Sfi.OpenSession())
			{
				Student g = session.CreateCriteria(typeof(Student))
									.Add(Expression.IdEq(gavin.StudentNumber))
									.SetFetchMode("Enrolments", FetchMode.Join)
									.SetProjection(Projections.RootEntity(lazy: false))
									.UniqueResult<Student>();

				Assert.That(NHibernateUtil.IsInitialized(g), Is.True, "object must be initialized");
				Assert.That(g, Is.EqualTo(gavin).Using((Student x, Student y) => x.StudentNumber == y.StudentNumber && x.Name == y.Name ? 0 : 1));
			}
		}

		[Test]
		public void UseRootProjection_Lazy()
		{
			//NH-3435
			PrepareDataForEntityProjectionTests(out Student gavin, out string _);

			using (ISession session = Sfi.OpenSession())
			{
				Student g = session.CreateCriteria(typeof(Student))
									.Add(Expression.IdEq(gavin.StudentNumber))
									.SetFetchMode("Enrolments", FetchMode.Join)
									.SetProjection(Projections.RootEntity(lazy: true))
									.UniqueResult<Student>();

				Assert.That(NHibernateUtil.IsInitialized(g), Is.False, "object must be lazy and not initialized");
				Assert.That(g, Is.EqualTo(gavin).Using((Student x, Student y) => x.StudentNumber == y.StudentNumber && x.Name == y.Name ? 0 : 1));
			}
		}

		[Test]
		public void UseEntityProjection_Eager()
		{
			//NH-3435
			PrepareDataForEntityProjectionTests(out Student gavin, out string courseCode);

			using (ISession session = Sfi.OpenSession())
			{
				Student g = session.CreateCriteria(typeof(Enrolment))
									.Add(Expression.And(Expression.Eq("StudentNumber", gavin.StudentNumber), Expression.Eq("CourseCode", courseCode)))
									.CreateAlias("Student", "s", JoinType.InnerJoin)
									.SetProjection(Projections.Entity<Student>("s", lazy:false))
									.UniqueResult<Student>();

				Assert.That(NHibernateUtil.IsInitialized(g), Is.True, "object must be initialized");
				Assert.That(g, Is.EqualTo(gavin).Using((Student x, Student y) => x.StudentNumber == y.StudentNumber && x.Name == y.Name ? 0 : 1));
			}
		}

		[Test]
		public void UseEntityProjection_Lazy()
		{
			//NH-3435
			PrepareDataForEntityProjectionTests(out Student gavin, out string courseCode);

			using (ISession session = Sfi.OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				Student g = session.CreateCriteria(typeof(Enrolment))
									.Add(Expression.And(Expression.Eq("StudentNumber", gavin.StudentNumber), Expression.Eq("CourseCode", courseCode)))
									.CreateAlias("Student", "s", JoinType.InnerJoin)
									.SetProjection(Projections.Entity<Student>("s", lazy:true))
									.UniqueResult<Student>();

				Assert.That(NHibernateUtil.IsInitialized(g), Is.False, "object must be lazy and not initialized");
				Assert.That(g, Is.EqualTo(gavin).Using((Student x, Student y) => x.StudentNumber == y.StudentNumber && x.Name == y.Name ? 0 : 1));
			}
		}

		[Test]
		public void UseMultipleEntityProjections()
		{
			//NH-3435
			PrepareDataForEntityProjectionTests(out Student gavin, out string courseCode);

			using (ISession session = Sfi.OpenSession())
			{
				Enrolment en = null;
				Student s = null;
				Course c = null;

				var result = session.QueryOver(() => en)
						.Where(e => e.StudentNumber == gavin.StudentNumber && e.CourseCode == courseCode)
						.JoinAlias(e => e.Student, () => s)
						.JoinAlias(e => e.Course, () => c)
						.Select(Projections.RootEntity(lazy: true), Projections.Entity(() => s, lazy:false), Projections.Entity(() => c, lazy:false))
						.SingleOrDefault<object[]>();

				en = (Enrolment) result[0];
				s = (Student) result[1];
				c = (Course) result[2];

				Assert.That(NHibernateUtil.IsInitialized(en), Is.False, "Object must be lazy and not initialized");
				Assert.That(NHibernateUtil.IsInitialized(s), Is.True, "Object must be initialized");
				Assert.That(NHibernateUtil.IsInitialized(c), Is.True, "Object must be initialized");
			}
		}
	}
}
