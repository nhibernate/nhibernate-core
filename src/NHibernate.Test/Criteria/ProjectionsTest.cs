namespace NHibernate.Test.Criteria
{
	using System.Collections;
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
						new TypedValue(NHibernateUtil.String, " "),
						Projections.Property("Name")
					))
					.UniqueResult<string>();
				Assert.AreEqual("ayende ayende", result);
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

		public class AddNumberProjection : SimpleProjection
		{
			private readonly string propertyName;
			private readonly int numberToAdd;

			public AddNumberProjection(string propertyName, int numberToAdd)
			{
				this.propertyName = propertyName;
				this.numberToAdd = numberToAdd;
			}

			public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery)
			{
				string[] projection = criteriaQuery.GetColumnsUsingProjection(criteria, propertyName);
				return new SqlStringBuilder()
					.Add("(")
					.Add(projection[0])
					.Add(" + ")
					.AddParameter()
					.Add(") as ")
					.Add(GetColumnAliases(0)[0])
					.ToSqlString();
			}

			public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
			{
				IType projection = criteriaQuery.GetTypeUsingProjection(criteria, propertyName);
				return new IType[] {projection};
			}

			public override NHibernate.Engine.TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
			{
				return new TypedValue[]
					{
						new TypedValue(NHibernateUtil.Int32, numberToAdd),
					};
			}
		}
	}
}