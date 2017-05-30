using System.Collections;
using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.Dialect;
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


	}
}
