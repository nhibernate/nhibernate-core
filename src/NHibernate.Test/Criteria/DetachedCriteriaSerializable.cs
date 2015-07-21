using System;
using System.Collections;
using NHibernate.Criterion;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.Criteria
{
	[TestFixture]
	public class DetachedCriteriaSerializable : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "Criteria.Enrolment.hbm.xml" }; }
		}

		private void SerializeAndList(DetachedCriteria dc)
		{
			byte[] bytes = SerializationHelper.Serialize(dc);

			DetachedCriteria dcs = (DetachedCriteria)SerializationHelper.Deserialize(bytes);

			using (ISession s = OpenSession())
			{
				dcs.GetExecutableCriteria(s).List();
			}
		}

		[Test]
		public void AllCriterionAreSerializable()
		{
			NHAssert.InheritedAreMarkedSerializable(typeof(ICriterion));
		}

		[Test]
		public void AllProjectionAreSerializable()
		{
			NHAssert.InheritedAreMarkedSerializable(typeof(IProjection));
		}

		[Test]
		public void AllEmbeddedResultTransformersAreSerializable()
		{
			NHAssert.InheritedAreMarkedSerializable(typeof(IResultTransformer));
		}

		[Test]
		public void DetachedCriteriaItSelf()
		{
			DetachedCriteria dc = DetachedCriteria.For(typeof(Student))
				.Add(Expression.Eq("Name", "Gavin King"));
			SerializeAndList(dc);
		}

		[Test]
		public void BasicCriterions()
		{
			ICriterion c = Expression.Eq("Name", "Gavin King");
			NHAssert.IsSerializable(c);
			IDictionary nameValue = new Hashtable(1);
			nameValue.Add("Name", "Ralph");
			c = Expression.AllEq(nameValue);
			NHAssert.IsSerializable(c);
			c = Expression.Between("Name", "aaaaa", "zzzzz");
			NHAssert.IsSerializable(c);
			c = Expression.EqProperty("Name", "Name");
			NHAssert.IsSerializable(c);
			c = Expression.Ge("Name", "a");
			NHAssert.IsSerializable(c);
			c = Expression.GeProperty("Name", "Name");
			NHAssert.IsSerializable(c);
			c = Expression.Gt("Name", "z");
			NHAssert.IsSerializable(c);
			c = Expression.GtProperty("Name", "Name");
			NHAssert.IsSerializable(c);
			c = Expression.IdEq(1);
			NHAssert.IsSerializable(c);
			c = Expression.In("Name", new string[] { "Gavin", "Ralph" });
			NHAssert.IsSerializable(c);
			c = Expression.InsensitiveLike("Name", "GAVIN");
			NHAssert.IsSerializable(c);
			c = Expression.IsEmpty("Enrolments");
			NHAssert.IsSerializable(c);
			c = Expression.IsNotEmpty("Enrolments");
			NHAssert.IsSerializable(c);
			c = Expression.IsNotNull("PreferredCourse");
			NHAssert.IsSerializable(c);
			c = Expression.IsNull("PreferredCourse");
			NHAssert.IsSerializable(c);
			c = Expression.Le("Name", "a");
			NHAssert.IsSerializable(c);
			c = Expression.LeProperty("Name", "Name");
			NHAssert.IsSerializable(c);
			c = Expression.Lt("Name", "z");
			NHAssert.IsSerializable(c);
			c = Expression.LtProperty("Name", "Name");
			NHAssert.IsSerializable(c);
			c = Expression.Like("Name", "G%");
			NHAssert.IsSerializable(c);
			c = Expression.Not(Expression.Eq("Name", "Ralph"));
			NHAssert.IsSerializable(c);
			c = Expression.NotEqProperty("Name", "Name");
			NHAssert.IsSerializable(c);
		}

		[Test]
		public void LikeCriterions()
		{
			ICriterion c = Expression.Like("Name", "Gavin", MatchMode.Anywhere);
			NHAssert.IsSerializable(c);
			c = Expression.Like("Name", "Gavin", MatchMode.End);
			NHAssert.IsSerializable(c);
			c = Expression.Like("Name", "Gavin", MatchMode.Exact);
			NHAssert.IsSerializable(c);
			c = Expression.Like("Name", "Gavin", MatchMode.Start);
			NHAssert.IsSerializable(c);
		}

		[Test]
		public void LogicalCriterions()
		{
			ICriterion c =
				Expression.Or(Expression.Eq("Name", "Ralph"), Expression.Eq("Name", "Gavin"));
			NHAssert.IsSerializable(c);
			c =
				Expression.And(Expression.Gt("StudentNumber", 1),
										  Expression.Lt("StudentNumber", 10));
			NHAssert.IsSerializable(c);
		}

		[Test]
		public void ProjectionsExpressions()
		{
			IProjection p = Projections.Avg("StudentNumber");
			NHAssert.IsSerializable(p);
			p = Projections.Count("StudentNumber");
			NHAssert.IsSerializable(p);
			p = Projections.CountDistinct("StudentNumber");
			NHAssert.IsSerializable(p);
			p = Projections.GroupProperty("Name");
			NHAssert.IsSerializable(p);
			p = Projections.Id();
			NHAssert.IsSerializable(p);
			p = Projections.Max("StudentNumber");
			NHAssert.IsSerializable(p);
			p = Projections.Min("StudentNumber");
			NHAssert.IsSerializable(p);
			p = Projections.Property("StudentNumber");
			NHAssert.IsSerializable(p);
			p = Projections.RowCount();
			NHAssert.IsSerializable(p);
			p = Projections.Sum("StudentNumber");
			NHAssert.IsSerializable(p);

			p = Projections.Alias(Projections.Count("StudentNumber"), "alias");
			NHAssert.IsSerializable(p);
			p = Projections.Distinct(Projections.Id());
			NHAssert.IsSerializable(p);
			p = Projections.ProjectionList().Add(Projections.Max("StudentNumber"));
			NHAssert.IsSerializable(p);
		}

		[Test]
		public void Junctions()
		{
			ICriterion c = Expression.Conjunction()
				.Add(Expression.Eq("Name", "Ralph"))
				.Add(Expression.Eq("StudentNumber", "1"));
			NHAssert.IsSerializable(c);
			c = Expression.Disjunction()
				.Add(Expression.Eq("Name", "Ralph"))
				.Add(Expression.Eq("Name", "Gavin"));
			NHAssert.IsSerializable(c);
		}

		[Test]
		public void SubqueriesExpressions()
		{
			DetachedCriteria dc = DetachedCriteria.For(typeof(Student))
				.Add(Expression.Eq("Name", "Gavin King"));
			ICriterion c = Subqueries.Eq("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.EqAll("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.Exists(dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.Ge("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.GeAll("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.GeSome("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.Gt("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.GtAll("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.GtSome("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.In("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.Le("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.LeAll("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.LeSome("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.Lt("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.LtAll("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.LtSome("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.Ne("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.NotExists(dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.NotIn("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.PropertyEq("Name", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.PropertyEqAll("Name", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.PropertyGe("Name", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.PropertyGeAll("Name", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.PropertyGeSome("Name", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.PropertyGt("Name", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.PropertyGtAll("Name", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.PropertyGtSome("Name", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.PropertyIn("Name", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.PropertyLe("Name", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.PropertyLeAll("Name", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.PropertyLeSome("Name", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.PropertyLt("Name", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.PropertyLtAll("Name", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.PropertyLtSome("Name", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.PropertyNe("Name", dc);
			NHAssert.IsSerializable(c);
			c = Subqueries.PropertyNotIn("Name", dc);
			NHAssert.IsSerializable(c);
		}

		[Test]
		public void SQLCriterion()
		{
			ICriterion c = Expression.Sql("SELECT Name FROM Student");
			NHAssert.IsSerializable(c);
		}

		[Test]
		public void SQLProjection()
		{
			IProjection p = Projections.SqlProjection("COUNT(*)",
													  new string[] { "tStudent" }, new IType[] { NHibernateUtil.Int32 });
			NHAssert.IsSerializable(p);
			p =
				Projections.SqlGroupProjection("COUNT({alias}.studentId), {alias}.preferredCourseCode",
											   "{alias}.preferredCourseCode",
											   new string[] { "studentsOfCourse", "CourseCode" },
											   new IType[] { NHibernateUtil.Int32, NHibernateUtil.Int32 });
			NHAssert.IsSerializable(p);
		}

		[Test]
		public void ResultTransformes()
		{
			IResultTransformer rt = new RootEntityResultTransformer();
			NHAssert.IsSerializable(rt);

			rt = new AliasToBeanConstructorResultTransformer(typeof(StudentDTO).GetConstructor(new System.Type[] { }));
			NHAssert.IsSerializable(rt);

			rt = new AliasToBeanResultTransformer(typeof(StudentDTO));
			NHAssert.IsSerializable(rt);

			rt = new DistinctRootEntityResultTransformer();
			NHAssert.IsSerializable(rt);

			rt = new PassThroughResultTransformer();
			NHAssert.IsSerializable(rt);
		}

		[Test]
		public void ExecutableCriteria()
		{
			// All query below don't have sense, are only to test if all needed classes are serializable

			// Basic criterion
			DetachedCriteria dc = DetachedCriteria.For(typeof(Student))
				.Add(Expression.Between("Name", "aaaaa", "zzzzz"))
				.Add(Expression.EqProperty("Name", "Name"))
				.Add(Expression.Ge("Name", "a"))
				.Add(Expression.GeProperty("Name", "Name"))
				.Add(Expression.Gt("Name", "z"))
				.Add(Expression.GtProperty("Name", "Name"))
				.Add(Expression.IdEq(1))
				.Add(Expression.In("Name", new string[] { "Gavin", "Ralph" }))
				.Add(Expression.InsensitiveLike("Name", "GAVIN"))
				.Add(Expression.IsEmpty("Enrolments"))
				.Add(Expression.IsNotEmpty("Enrolments"))
				.Add(Expression.IsNotNull("PreferredCourse"))
				.Add(Expression.IsNull("PreferredCourse"))
				.Add(Expression.Le("Name", "a"))
				.Add(Expression.LeProperty("Name", "Name"))
				.Add(Expression.Lt("Name", "z"))
				.Add(Expression.LtProperty("Name", "Name"))
				.Add(Expression.Like("Name", "G%"))
				.Add(Expression.Not(Expression.Eq("Name", "Ralph")))
				.Add(Expression.NotEqProperty("Name", "Name"))
				.AddOrder(Order.Asc("StudentNumber"))
				.SetProjection(Property.ForName("StudentNumber"));

			SerializeAndList(dc);

			// Like match modes
			dc = DetachedCriteria.For(typeof(Student))
				.Add(Expression.Like("Name", "Gavin", MatchMode.Anywhere))
				.Add(Expression.Like("Name", "Gavin", MatchMode.End))
				.Add(Expression.Like("Name", "Gavin", MatchMode.Exact))
				.Add(Expression.Like("Name", "Gavin", MatchMode.Start));

			SerializeAndList(dc);

			// Logical Expression
			dc = DetachedCriteria.For(typeof(Student))
				.Add(Expression.Or(Expression.Eq("Name", "Ralph"), Expression.Eq("Name", "Gavin")))
				.Add(
				Expression.And(Expression.Gt("StudentNumber", 1L),
										  Expression.Lt("StudentNumber", 10L)));

			SerializeAndList(dc);

			// Projections
			dc = DetachedCriteria.For(typeof(Enrolment))
				.SetProjection(Projections.Distinct(Projections.ProjectionList()
														.Add(Projections.Property("StudentNumber"), "stNumber")
														.Add(Projections.Property("CourseCode"), "cCode")))
				.Add(Expression.Lt("StudentNumber", 668L));
			SerializeAndList(dc);

			dc = DetachedCriteria.For(typeof(Enrolment))
				.SetProjection(Projections.Count("StudentNumber").SetDistinct());
			SerializeAndList(dc);

			dc = DetachedCriteria.For(typeof(Enrolment))
				.SetProjection(Projections.ProjectionList()
								.Add(Projections.Count("StudentNumber"))
								.Add(Projections.Max("StudentNumber"))
								.Add(Projections.Min("StudentNumber"))
								.Add(Projections.Avg("StudentNumber")));
			SerializeAndList(dc);

			// Junctions
			dc = DetachedCriteria.For(typeof(Student))
				.Add(Expression.Conjunction()
						.Add(Expression.Eq("Name", "Ralph"))
						.Add(Expression.Eq("StudentNumber", 1L)))
				.Add(Expression.Disjunction()
						.Add(Expression.Eq("Name", "Ralph"))
						.Add(Expression.Eq("Name", "Gavin")));
			SerializeAndList(dc);

			// Subquery
			dc = DetachedCriteria.For(typeof(Student))
				.Add(Property.ForName("StudentNumber").Eq(232L))
				.SetProjection(Property.ForName("Name"));

			DetachedCriteria dcs = DetachedCriteria.For(typeof(Student))
				.Add(Subqueries.PropertyEqAll("Name", dc));
			SerializeAndList(dc);

			// SQLCriterion
			dc = DetachedCriteria.For(typeof(Student))
				.Add(Expression.Sql("{alias}.Name = 'Gavin'"));
			SerializeAndList(dc);

			// SQLProjection
			dc = DetachedCriteria.For(typeof(Enrolment))
				.SetProjection(Projections.SqlProjection("1 as constOne, count(*) as countStar",
														 new String[] { "constOne", "countStar" },
														 new IType[] { NHibernateUtil.Int32, NHibernateUtil.Int32 }));
			SerializeAndList(dc);

			dc = DetachedCriteria.For(typeof(Student))
				.SetProjection(
				Projections.SqlGroupProjection("COUNT({alias}.studentId), {alias}.preferredCourseCode",
											   "{alias}.preferredCourseCode",
											   new string[] { "studentsOfCourse", "CourseCode" },
											   new IType[] { NHibernateUtil.Int32, NHibernateUtil.Int32 }));
			SerializeAndList(dc);

			// Result transformers
			dc = DetachedCriteria.For(typeof(Enrolment))
				.CreateAlias("Student", "st")
				.CreateAlias("Course", "co")
				.SetProjection(Projections.ProjectionList()
								.Add(Projections.Property("st.Name"), "studentName")
								.Add(Projections.Property("co.Description"), "courseDescription")
				)
				.AddOrder(Order.Desc("studentName"))
				.SetResultTransformer(Transformers.AliasToBean(typeof(StudentDTO)));
			SerializeAndList(dc);
		}
	}
}