using System;
using System.Collections;
using System.Reflection;

using NUnit.Framework;
using NHibernate.Expression;
using NHibernate.Util;
using NHibernate.Transform;

namespace NHibernate.Test.Criteria
{
	[TestFixture]
	public class DetachedCriteriaSerializable : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override System.Collections.IList Mappings
		{
			get	
			{
				return new string[] { "Criteria.Enrolment.hbm.xml"	};
			}
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
			NHAssert.InheritedAreMarkedSerializable(typeof(NHibernate.Expression.ICriterion));
		}

		[Test]
		public void AllProjectionAreSerializable()
		{
			NHAssert.InheritedAreMarkedSerializable(typeof(NHibernate.Expression.IProjection));
		}

		[Test]
		public void AllEmbeddedResultTrasformesHareSerializable()
		{
			NHAssert.InheritedAreMarkedSerializable(typeof(NHibernate.Transform.IResultTransformer));
		}

		[Test]
		public void DetachedCriteriaItSelf()
		{
			DetachedCriteria dc = DetachedCriteria.For(typeof(Student))
				.Add(Expression.Expression.Eq("Name", "Gavin King"));
			SerializeAndList(dc);
		}

		[Test]
		public void BasicCriterions()
		{
			ICriterion c = Expression.Expression.Eq("Name", "Gavin King");
			NHAssert.IsSerializable(c);
			IDictionary nameValue = new Hashtable(1);
			nameValue.Add("Name", "Ralph");
			c = Expression.Expression.AllEq(nameValue);
			NHAssert.IsSerializable(c);
			c = Expression.Expression.Between("Name", "aaaaa", "zzzzz");
			NHAssert.IsSerializable(c);
			c = Expression.Expression.EqProperty("Name", "Name");
			NHAssert.IsSerializable(c);
			c = Expression.Expression.Ge("Name", "a");
			NHAssert.IsSerializable(c);
			c = Expression.Expression.GeProperty("Name", "Name");
			NHAssert.IsSerializable(c);
			c = Expression.Expression.Gt("Name", "z");
			NHAssert.IsSerializable(c);
			c = Expression.Expression.GtProperty("Name", "Name");
			NHAssert.IsSerializable(c);
			c = Expression.Expression.IdEq(1);
			NHAssert.IsSerializable(c);
			c = Expression.Expression.In("Name", new string[] { "Gavin", "Ralph" });
			NHAssert.IsSerializable(c);
			c = Expression.Expression.InsensitiveLike("Name", "GAVIN");
			NHAssert.IsSerializable(c);
			c = Expression.Expression.IsEmpty("Enrolments");
			NHAssert.IsSerializable(c);
			c = Expression.Expression.IsNotEmpty("Enrolments");
			NHAssert.IsSerializable(c);
			c = Expression.Expression.IsNotNull("PreferredCourse");
			NHAssert.IsSerializable(c);
			c = Expression.Expression.IsNull("PreferredCourse");
			NHAssert.IsSerializable(c);
			c = Expression.Expression.Le("Name", "a");
			NHAssert.IsSerializable(c);
			c = Expression.Expression.LeProperty("Name", "Name");
			NHAssert.IsSerializable(c);
			c = Expression.Expression.Lt("Name", "z");
			NHAssert.IsSerializable(c);
			c = Expression.Expression.LtProperty("Name", "Name");
			NHAssert.IsSerializable(c);
			c = Expression.Expression.Like("Name", "G%");
			NHAssert.IsSerializable(c);
			c = Expression.Expression.Not(Expression.Expression.Eq("Name", "Ralph"));
			NHAssert.IsSerializable(c);
			c = Expression.Expression.NotEqProperty("Name", "Name");
			NHAssert.IsSerializable(c);
		}

		[Test]
		public void LikeCriterions()
		{
			ICriterion c = Expression.Expression.Like("Name", "Gavin", MatchMode.Anywhere);
			NHAssert.IsSerializable(c);
			c = Expression.Expression.Like("Name", "Gavin", MatchMode.End);
			NHAssert.IsSerializable(c);
			c = Expression.Expression.Like("Name", "Gavin", MatchMode.Exact);
			NHAssert.IsSerializable(c);
			c = Expression.Expression.Like("Name", "Gavin", MatchMode.Start);
			NHAssert.IsSerializable(c);
		}

		[Test]
		public void LogicalCriterions()
		{
			ICriterion c = Expression.Expression.Or(Expression.Expression.Eq("Name", "Ralph"), Expression.Expression.Eq("Name", "Gavin"));
			NHAssert.IsSerializable(c);
			c = Expression.Expression.And(Expression.Expression.Gt("StudentNumber", 1), Expression.Expression.Lt("StudentNumber", 10));
			NHAssert.IsSerializable(c);
		}

		[Test]
		public void ProjectionsExpressions()
		{
			IProjection p = Expression.Projections.Avg("StudentNumber");
			NHAssert.IsSerializable(p);
			p = Expression.Projections.Count("StudentNumber");
			NHAssert.IsSerializable(p);
			p = Expression.Projections.CountDistinct("StudentNumber");
			NHAssert.IsSerializable(p);
			p = Expression.Projections.GroupProperty("Name");
			NHAssert.IsSerializable(p);
			p = Expression.Projections.Id();
			NHAssert.IsSerializable(p);
			p = Expression.Projections.Max("StudentNumber");
			NHAssert.IsSerializable(p);
			p = Expression.Projections.Min("StudentNumber");
			NHAssert.IsSerializable(p);
			p = Expression.Projections.Property("StudentNumber");
			NHAssert.IsSerializable(p);
			p = Expression.Projections.RowCount();
			NHAssert.IsSerializable(p);
			p = Expression.Projections.Sum("StudentNumber");
			NHAssert.IsSerializable(p);

			p = Expression.Projections.Alias(Expression.Projections.Count("StudentNumber"),"alias");
			NHAssert.IsSerializable(p);
			p = Expression.Projections.Distinct(Expression.Projections.Id());
			NHAssert.IsSerializable(p);
			p = Expression.Projections.ProjectionList().Add(Expression.Projections.Max("StudentNumber"));
			NHAssert.IsSerializable(p);
		}

		[Test]
		public void Junctions()
		{
			ICriterion c = Expression.Expression.Conjunction()
				.Add(Expression.Expression.Eq("Name", "Ralph"))
				.Add(Expression.Expression.Eq("StudentNumber", "1"));
			NHAssert.IsSerializable(c);
			c = Expression.Expression.Disjunction()
				.Add(Expression.Expression.Eq("Name", "Ralph"))
				.Add(Expression.Expression.Eq("Name", "Gavin"));
			NHAssert.IsSerializable(c);
		}

		[Test]
		public void SubqueriesExpressions()
		{
			DetachedCriteria dc = DetachedCriteria.For(typeof(Student))
				.Add(Expression.Expression.Eq("Name", "Gavin King"));
			ICriterion c = Expression.Subqueries.Eq("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.EqAll("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.Exists(dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.Ge("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.GeAll("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.GeSome("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.Gt("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.GtAll("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.GtSome("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.In("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.Le("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.LeAll("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.LeSome("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.Lt("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.LtAll("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.LtSome("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.Ne("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.NotExists(dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.NotIn("Gavin King", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.PropertyEq("Name", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.PropertyEqAll("Name", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.PropertyGe("Name", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.PropertyGeAll("Name", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.PropertyGeSome("Name", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.PropertyGt("Name", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.PropertyGtAll("Name", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.PropertyGtSome("Name", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.PropertyIn("Name", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.PropertyLe("Name", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.PropertyLeAll("Name", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.PropertyLeSome("Name", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.PropertyLt("Name", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.PropertyLtAll("Name", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.PropertyLtSome("Name", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.PropertyNe("Name", dc);
			NHAssert.IsSerializable(c);
			c = Expression.Subqueries.PropertyNotIn("Name", dc);
			NHAssert.IsSerializable(c);
		}

		[Test]
		public void SQLCriterion()
		{
			ICriterion c = Expression.Expression.Sql("SELECT Name FROM Student");
			NHAssert.IsSerializable(c);
		}

		[Test]
		public void SQLProjection()
		{
			IProjection p = Expression.Projections.SqlProjection("COUNT(*)",
				new string[] { "tStudent" }, new NHibernate.Type.IType[] { NHibernateUtil.Int32 });
			NHAssert.IsSerializable(p);
			p = Projections.SqlGroupProjection("COUNT({alias}.studentId), {alias}.preferredCourseCode", "{alias}.preferredCourseCode",
					new string[] { "studentsOfCourse", "CourseCode" },
					new NHibernate.Type.IType[] { NHibernateUtil.Int32, NHibernateUtil.Int32 });
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
				.Add(Expression.Expression.Between("Name", "aaaaa", "zzzzz"))
				.Add(Expression.Expression.EqProperty("Name", "Name"))
				.Add(Expression.Expression.Ge("Name", "a"))
				.Add(Expression.Expression.GeProperty("Name", "Name"))
				.Add(Expression.Expression.Gt("Name", "z"))
				.Add(Expression.Expression.GtProperty("Name", "Name"))
				.Add(Expression.Expression.IdEq(1))
				.Add(Expression.Expression.In("Name", new string[] { "Gavin", "Ralph" }))
				.Add(Expression.Expression.InsensitiveLike("Name", "GAVIN"))
				.Add(Expression.Expression.IsEmpty("Enrolments"))
				.Add(Expression.Expression.IsNotEmpty("Enrolments"))
				.Add(Expression.Expression.IsNotNull("PreferredCourse"))
				.Add(Expression.Expression.IsNull("PreferredCourse"))
				.Add(Expression.Expression.Le("Name", "a"))
				.Add(Expression.Expression.LeProperty("Name", "Name"))
				.Add(Expression.Expression.Lt("Name", "z"))
				.Add(Expression.Expression.LtProperty("Name", "Name"))
				.Add(Expression.Expression.Like("Name", "G%"))
				.Add(Expression.Expression.Not(Expression.Expression.Eq("Name", "Ralph")))
				.Add(Expression.Expression.NotEqProperty("Name", "Name"))
				.AddOrder(Order.Asc("StudentNumber"))
				.SetProjection(Expression.Property.ForName("StudentNumber"));

			SerializeAndList(dc);

			// Like match modes
			dc = DetachedCriteria.For(typeof(Student))
				.Add(Expression.Expression.Like("Name", "Gavin", MatchMode.Anywhere))
				.Add(Expression.Expression.Like("Name", "Gavin", MatchMode.End))
				.Add(Expression.Expression.Like("Name", "Gavin", MatchMode.Exact))
				.Add(Expression.Expression.Like("Name", "Gavin", MatchMode.Start));

			SerializeAndList(dc);

			// Logical Expression
			dc = DetachedCriteria.For(typeof(Student))
				.Add(Expression.Expression.Or(Expression.Expression.Eq("Name", "Ralph"), Expression.Expression.Eq("Name", "Gavin")))
				.Add(Expression.Expression.And(Expression.Expression.Gt("StudentNumber", 1L), Expression.Expression.Lt("StudentNumber", 10L)));

			SerializeAndList(dc);

			// Projections
			dc = DetachedCriteria.For(typeof(Enrolment))
				.SetProjection(Projections.Distinct(Projections.ProjectionList()
					.Add(Projections.Property("StudentNumber"), "stNumber")
					.Add(Projections.Property("CourseCode"), "cCode")))
				.Add(Expression.Expression.Lt("StudentNumber", 668L));
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
				.Add(Expression.Expression.Conjunction()
					.Add(Expression.Expression.Eq("Name", "Ralph"))
					.Add(Expression.Expression.Eq("StudentNumber", 1L)))
				.Add(Expression.Expression.Disjunction()
					.Add(Expression.Expression.Eq("Name", "Ralph"))
					.Add(Expression.Expression.Eq("Name", "Gavin")));
			SerializeAndList(dc);

			// Subquery
			dc = DetachedCriteria.For(typeof(Student))
				.Add(Expression.Property.ForName("StudentNumber").Eq(232L))
				.SetProjection(Expression.Property.ForName("Name"));

			DetachedCriteria dcs = DetachedCriteria.For(typeof(Student))
				.Add(Subqueries.PropertyEqAll("Name", dc));
			SerializeAndList(dc);

			// SQLCriterion
			dc = DetachedCriteria.For(typeof(Student))
				.Add(Expression.Expression.Sql("{alias}.Name = 'Gavin'"));
			SerializeAndList(dc);

			// SQLProjection
			dc = DetachedCriteria.For(typeof(Enrolment))
				.SetProjection(Projections.SqlProjection("1 as constOne, count(*) as countStar",
						new String[] { "constOne", "countStar" },
						new NHibernate.Type.IType[] { NHibernateUtil.Int32, NHibernateUtil.Int32 }));
			SerializeAndList(dc);

			dc = DetachedCriteria.For(typeof(Student))
				.SetProjection(Projections.SqlGroupProjection("COUNT({alias}.studentId), {alias}.preferredCourseCode", "{alias}.preferredCourseCode",
					new string[] { "studentsOfCourse", "CourseCode" },
					new NHibernate.Type.IType[] { NHibernateUtil.Int32, NHibernateUtil.Int32 }));
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
				.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean(typeof(StudentDTO)));
			SerializeAndList(dc);
		}
	}
}
