using System;
using System.Collections;
using System.Reflection;

using NUnit.Framework;
using NHibernate.Expression;
using NHibernate.Util;

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

		private void AssertSerializable(object toSerialize)
		{
			try
			{
				byte[] bytes = SerializationHelper.Serialize(toSerialize);
				object ds = SerializationHelper.Deserialize(bytes);
			}
			catch (System.Runtime.Serialization.SerializationException e)
			{
				Assert.Fail(string.Format("For class {0}: {1}", toSerialize.GetType().Name, e.Message));
			}
		}

		[Test]
		public void AllCriterionHareSerializable()
		{
			Assembly nhbA = Assembly.GetAssembly(typeof(ICriterion));
			IList types = ClassList(nhbA, typeof(ICriterion));
			CheckSerializable(types, "Criterion");
		}

		[Test]
		public void AllProjectionHareSerializable()
		{
			Assembly nhbA = Assembly.GetAssembly(typeof(IProjection));
			IList types = ClassList(nhbA, typeof(IProjection));
			CheckSerializable(types, "Projection");
		}

		private void CheckSerializable(IList types, string context)
		{
			IList noSerializableCriterions = new ArrayList(types.Count);
			foreach (System.Type tp in types)
			{
				object[] atts = tp.GetCustomAttributes(typeof(System.SerializableAttribute), false);
				if (atts.Length == 0)
				{
					noSerializableCriterions.Add(tp.Name);
				}
			}
			if (noSerializableCriterions.Count > 0)
			{
				Console.WriteLine("--------------> No serializable "+ context+":");
				foreach (string name in noSerializableCriterions)
					Console.WriteLine(name);
				Console.WriteLine("<- Make it serializable and add to test ");
				Console.WriteLine("<--------------------------------------");
			}
			Assert.AreEqual(0, noSerializableCriterions.Count, "Some " + context + " are not Serializable.");
		}

		private IList ClassList(Assembly assembly, System.Type interfaceType)
		{
			IList result = new ArrayList();
			if (assembly != null)
			{
				System.Type[] types = assembly.GetTypes();
				foreach (System.Type tp in types)
				{
					if (ImplementInterface(tp, interfaceType))
						result.Add(tp);
				}
			}
			return result;
		}

		private bool ImplementInterface(System.Type objType, System.Type interfaceType)
		{
			if (objType != null)
			{
				TypeFilter intf = new TypeFilter(ImplementInterfaceFilter);
				System.Type[] _AvalInterfaces = objType.FindInterfaces(intf, interfaceType);
				return _AvalInterfaces.Length > 0;
			}
			else
				return false;
		}

		private static bool ImplementInterfaceFilter(System.Type typeObj, object criteria)
		{
			if (criteria != null)
				return typeObj == criteria;
			else
				return false;
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
			AssertSerializable(c);
			IDictionary nameValue = new Hashtable(1);
			nameValue.Add("Name", "Ralph");
			c = Expression.Expression.AllEq(nameValue);
			AssertSerializable(c);
			c = Expression.Expression.Between("Name", "aaaaa", "zzzzz");
			AssertSerializable(c);
			c = Expression.Expression.EqProperty("Name", "Name");
			AssertSerializable(c);
			c = Expression.Expression.Ge("Name", "a");
			AssertSerializable(c);
			c = Expression.Expression.GeProperty("Name", "Name");
			AssertSerializable(c);
			c = Expression.Expression.Gt("Name", "z");
			AssertSerializable(c);
			c = Expression.Expression.GtProperty("Name", "Name");
			AssertSerializable(c);
			c = Expression.Expression.IdEq(1);
			AssertSerializable(c);
			c = Expression.Expression.In("Name", new string[] { "Gavin", "Ralph" });
			AssertSerializable(c);
			c = Expression.Expression.InsensitiveLike("Name", "GAVIN");
			AssertSerializable(c);
			c = Expression.Expression.IsEmpty("Enrolments");
			AssertSerializable(c);
			c = Expression.Expression.IsNotEmpty("Enrolments");
			AssertSerializable(c);
			c = Expression.Expression.IsNotNull("PreferredCourse");
			AssertSerializable(c);
			c = Expression.Expression.IsNull("PreferredCourse");
			AssertSerializable(c);
			c = Expression.Expression.Le("Name", "a");
			AssertSerializable(c);
			c = Expression.Expression.LeProperty("Name", "Name");
			AssertSerializable(c);
			c = Expression.Expression.Lt("Name", "z");
			AssertSerializable(c);
			c = Expression.Expression.LtProperty("Name", "Name");
			AssertSerializable(c);
			c = Expression.Expression.Like("Name", "G%");
			AssertSerializable(c);
			c = Expression.Expression.Not(Expression.Expression.Eq("Name", "Ralph"));
			AssertSerializable(c);
			c = Expression.Expression.NotEqProperty("Name", "Name");
			AssertSerializable(c);
		}

		[Test]
		public void LikeCriterions()
		{
			ICriterion c = Expression.Expression.Like("Name", "Gavin", MatchMode.Anywhere);
			AssertSerializable(c);
			c = Expression.Expression.Like("Name", "Gavin", MatchMode.End);
			AssertSerializable(c);
			c = Expression.Expression.Like("Name", "Gavin", MatchMode.Exact);
			AssertSerializable(c);
			c = Expression.Expression.Like("Name", "Gavin", MatchMode.Start);
			AssertSerializable(c);
		}

		[Test]
		public void LogicalCriterions()
		{
			ICriterion c = Expression.Expression.Or(Expression.Expression.Eq("Name", "Ralph"), Expression.Expression.Eq("Name", "Gavin"));
			AssertSerializable(c);
			c = Expression.Expression.And(Expression.Expression.Gt("StudentNumber", 1), Expression.Expression.Lt("StudentNumber", 10));
			AssertSerializable(c);
		}

		[Test]
		public void ProjectionsExpressions()
		{
			IProjection p = Expression.Projections.Avg("StudentNumber");
			AssertSerializable(p);
			p = Expression.Projections.Count("StudentNumber");
			AssertSerializable(p);
			p = Expression.Projections.CountDistinct("StudentNumber");
			AssertSerializable(p);
			p = Expression.Projections.GroupProperty("Name");
			AssertSerializable(p);
			p = Expression.Projections.Id();
			AssertSerializable(p);
			p = Expression.Projections.Max("StudentNumber");
			AssertSerializable(p);
			p = Expression.Projections.Min("StudentNumber");
			AssertSerializable(p);
			p = Expression.Projections.Property("StudentNumber");
			AssertSerializable(p);
			p = Expression.Projections.RowCount();
			AssertSerializable(p);
			p = Expression.Projections.Sum("StudentNumber");
			AssertSerializable(p);

			p = Expression.Projections.Alias(Expression.Projections.Count("StudentNumber"),"alias");
			AssertSerializable(p);
			p = Expression.Projections.Distinct(Expression.Projections.Id());
			AssertSerializable(p);
			p = Expression.Projections.ProjectionList().Add(Expression.Projections.Max("StudentNumber"));
			AssertSerializable(p);
		}

		[Test]
		public void Junctions()
		{
			ICriterion c = Expression.Expression.Conjunction()
				.Add(Expression.Expression.Eq("Name", "Ralph"))
				.Add(Expression.Expression.Eq("StudentNumber", "1"));
			AssertSerializable(c);
			c = Expression.Expression.Disjunction()
				.Add(Expression.Expression.Eq("Name", "Ralph"))
				.Add(Expression.Expression.Eq("Name", "Gavin"));
			AssertSerializable(c);
		}

		[Test]
		public void SubqueriesExpressions()
		{
			DetachedCriteria dc = DetachedCriteria.For(typeof(Student))
				.Add(Expression.Expression.Eq("Name", "Gavin King"));
			ICriterion c = Expression.Subqueries.Eq("Gavin King", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.EqAll("Gavin King", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.Exists(dc);
			AssertSerializable(c);
			c = Expression.Subqueries.Ge("Gavin King", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.GeAll("Gavin King", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.GeSome("Gavin King", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.Gt("Gavin King", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.GtAll("Gavin King", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.GtSome("Gavin King", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.In("Gavin King", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.Le("Gavin King", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.LeAll("Gavin King", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.LeSome("Gavin King", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.Lt("Gavin King", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.LtAll("Gavin King", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.LtSome("Gavin King", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.Ne("Gavin King", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.NotExists(dc);
			AssertSerializable(c);
			c = Expression.Subqueries.NotIn("Gavin King", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.PropertyEq("Name", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.PropertyEqAll("Name", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.PropertyGe("Name", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.PropertyGeAll("Name", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.PropertyGeSome("Name", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.PropertyGt("Name", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.PropertyGtAll("Name", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.PropertyGtSome("Name", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.PropertyIn("Name", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.PropertyLe("Name", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.PropertyLeAll("Name", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.PropertyLeSome("Name", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.PropertyLt("Name", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.PropertyLtAll("Name", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.PropertyLtSome("Name", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.PropertyNe("Name", dc);
			AssertSerializable(c);
			c = Expression.Subqueries.PropertyNotIn("Name", dc);
			AssertSerializable(c);
		}

		[Test]
		public void SQLCriterion()
		{
			ICriterion c = Expression.Expression.Sql("SELECT Name FROM Student");
			AssertSerializable(c);
		}

		[Test]
		public void SQLProjection()
		{
			IProjection p = Expression.Projections.SqlProjection("COUNT(*)",
				new string[] { "tStudent" }, new NHibernate.Type.IType[] { NHibernateUtil.Int32 });
			AssertSerializable(p);
			p = Projections.SqlGroupProjection("COUNT({alias}.studentId), {alias}.preferredCourseCode", "{alias}.preferredCourseCode",
					new string[] { "studentsOfCourse", "CourseCode" },
					new NHibernate.Type.IType[] { NHibernateUtil.Int32, NHibernateUtil.Int32 });
			AssertSerializable(p);
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
				.Add(Expression.Expression.And(Expression.Expression.Gt("StudentNumber", 1), Expression.Expression.Lt("StudentNumber", 10)));

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
					.Add(Expression.Expression.Eq("StudentNumber", "1")))
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
				.Add(Expression.Expression.Sql("{alias}.Name"));
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
		}
	}
}
