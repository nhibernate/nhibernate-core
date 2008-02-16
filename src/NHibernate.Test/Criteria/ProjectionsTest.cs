namespace NHibernate.Test.Criteria
{
	using System.Collections;
	using System.Collections.Generic;
	using Engine;
	using Expressions;
	using NUnit.Framework;
	using SqlCommand;
	using Type;
	using TestCase = NHibernate.Test.TestCase;

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
			using (ISession session = sessions.OpenSession())
			{
				session.Delete("from System.Object");
				session.Flush();
			}
		}

		[Test]
		public void UsingSqlFunctions_Concat()
		{
			using (ISession session = sessions.OpenSession())
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
			using (ISession session = sessions.OpenSession())
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
			using(ISession session = sessions.OpenSession())
			{
				long result = session.CreateCriteria(typeof(Student))
					.SetProjection(new AddNumberProjection("id",15))
					.UniqueResult<long>();
				Assert.AreEqual(42L, result);
			}
		}

		[Test]
		public void UsingConditionals()
		{
			using (ISession session = sessions.OpenSession())
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
	}
}