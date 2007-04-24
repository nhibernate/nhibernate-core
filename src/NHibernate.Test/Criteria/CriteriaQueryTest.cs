using System;
using System.Collections;
using NHibernate.Expression;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.Criteria
{
	[TestFixture]
	public class CriteriaQueryTest : TestCase
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

		[Test, Ignore("EscapeCharacter not implemented yet in NHibernate.Expression.LikeExpression")]
		public void EscapeCharacter()
		{
			ISession session = OpenSession();
			ITransaction t = session.BeginTransaction();
			Course c1 = new Course();
			c1.CourseCode = "course-1";
			c1.Description = "%1";
			Course c2 = new Course();
			c2.CourseCode = "course-2";
			c2.Description = "%2";
			Course c3 = new Course();
			c3.CourseCode = "course-3";
			c3.Description = "control";
			session.Save(c1);
			session.Save(c2);
			session.Save(c3);
			session.Flush();
			session.Clear();

			// finds all courses which have a description equal to '%1'
			Course example = new Course();
			example.Description = "&%1";
			//IList result = session.CreateCriteria(typeof(Course))
			//    .Add(Example.Create(example).IgnoreCase().EnableLike().EscapeCharacter('&'))
			//    .List();
			//Assert.AreEqual(1, result.Count);
			// finds all courses which contain '%' as the first char in the description 
			example.Description = "&%%";
			//result = session.CreateCriteria(typeof(Course))
			//    .Add(Example.Create(example).IgnoreCase().EnableLike().EscapeCharacter('&'))
			//    .List();
			//Assert.AreEqual(2, result.Count);

			session.Delete(typeof(Course));
			t.Commit();
			session.Close();
		}

		[Test, Ignore("ScrollableResults not implemented")]
		public void ScrollCriteria()
		{
			ISession session = OpenSession();
			ITransaction t = session.BeginTransaction();

			Course course = new Course();
			course.CourseCode = "HIB";
			course.Description = "Hibernate Training";
			session.Save(course);
			session.Flush();
			session.Clear();
			//IScrollableResults sr = session.CreateCriteria(typeof(Course)).Scroll();
			//Assert.IsTrue( sr.Next() );
			//course = (Course) sr[0];
			Assert.IsNotNull(course);
			//sr.Close();
			session.Delete(course);

			t.Commit();
			session.Close();
		}

		[Test]
		public void Subselect()
		{
			ISession session = OpenSession();
			ITransaction t = session.BeginTransaction();

			Course course = new Course();
			course.CourseCode = "HIB";
			course.Description = "Hibernate Training";
			session.Save(course);

			Student gavin = new Student();
			gavin.Name = "Gavin King";
			gavin.StudentNumber = 232;
			session.Save(gavin);

			Enrolment enrolment2 = new Enrolment();
			enrolment2.Course = course;
			enrolment2.CourseCode = course.CourseCode;
			enrolment2.Semester = 3;
			enrolment2.Year = 1998;
			enrolment2.Student = gavin;
			enrolment2.StudentNumber = gavin.StudentNumber;
			gavin.Enrolments.Add(enrolment2);
			session.Save(enrolment2);

			DetachedCriteria dc = DetachedCriteria.For(typeof(Student))
				.Add(Expression.Property.ForName("StudentNumber").Eq(232L))
				.SetProjection(Expression.Property.ForName("Name"));

			session.CreateCriteria(typeof(Student))
				.Add(Subqueries.PropertyEqAll("Name", dc))
				.List();

			session.CreateCriteria(typeof(Student))
				.Add(Subqueries.Exists(dc))
				.List();

			session.CreateCriteria(typeof(Student))
				.Add(Expression.Property.ForName("Name").EqAll(dc))
				.List();

			session.CreateCriteria(typeof(Student))
				.Add(Subqueries.In("Gavin King", dc))
				.List();

			DetachedCriteria dc2 = DetachedCriteria.For(typeof(Student), "st")
				.Add(Expression.Property.ForName("st.StudentNumber").EqProperty("e.StudentNumber"))
				.SetProjection(Expression.Property.ForName("Name"));

			session.CreateCriteria(typeof(Enrolment), "e")
				.Add(Subqueries.Eq("Gavin King", dc2))
				.List();

			DetachedCriteria dc3 = DetachedCriteria.For(typeof(Student), "st")
				.CreateCriteria("Enrolments")
				.CreateCriteria("Course")
				.Add(Expression.Property.ForName("Description").Eq("Hibernate Training"))
				.SetProjection(Expression.Property.ForName("st.Name"));

			session.CreateCriteria(typeof(Enrolment), "e")
				.Add(Subqueries.Eq("Gavin King", dc3))
				.List();

			DetachedCriteria courseCriteria = DetachedCriteria.For(typeof(Course))
				.Add(Expression.Property.ForName("Description").Eq("Hibernate Training"))
				.SetProjection(Projections.Property("CourseCode"));

			DetachedCriteria enrolmentCriteria = DetachedCriteria.For(typeof(Enrolment))
				.Add(Expression.Property.ForName("CourseCode").Eq(courseCriteria))
				.SetProjection(Projections.Property("CourseCode"));

			DetachedCriteria studentCriteria = DetachedCriteria.For(typeof(Student))
				.Add(Subqueries.Exists(enrolmentCriteria));

			object result = studentCriteria.GetExecutableCriteria(session).UniqueResult();
			Assert.AreSame(gavin, result);

			session.Delete(enrolment2);
			session.Delete(gavin);
			session.Delete(course);
			t.Commit();
			session.Close();
		}

		[Test]
		public void CloningCriteria()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			// HQL: from Animal a where a.mother.class = Reptile
			ICriteria c = s.CreateCriteria(typeof(Animal), "a")
				.CreateAlias("mother", "m")
				.Add(Expression.Property.ForName("m.class").Eq(typeof(Reptile)));
			ICriteria cloned = c.Clone();
			cloned.List();
			t.Rollback();
			s.Close();
		}

		[Test]
		public void DetachedCriteriaTest()
		{
			DetachedCriteria dc = DetachedCriteria.For(typeof(Student))
				.Add(Expression.Property.ForName("Name").Eq("Gavin King"))
				.AddOrder(Order.Asc("StudentNumber"))
				.SetProjection(Expression.Property.ForName("StudentNumber"));

			byte[] bytes = SerializationHelper.Serialize(dc);

			dc = (DetachedCriteria) SerializationHelper.Deserialize(bytes);

			ISession session = OpenSession();
			ITransaction t = session.BeginTransaction();

			Student gavin = new Student();
			gavin.Name = "Gavin King";
			gavin.StudentNumber = 232;
			Student bizarroGavin = new Student();
			bizarroGavin.Name = "Gavin King";
			bizarroGavin.StudentNumber = 666;
			session.Save(bizarroGavin);
			session.Save(gavin);

			IList result = dc.GetExecutableCriteria(session)
				.SetMaxResults(3)
				.List();

			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(232L, result[0]);
			Assert.AreEqual(666L, result[1]);

			session.Delete(gavin);
			session.Delete(bizarroGavin);
			t.Commit();
			session.Close();
		}

		[Test]
		public void CloningDetachedCriteriaTest()
		{
			DetachedCriteria dc = DetachedCriteria.For(typeof(Student))
				.Add(Expression.Property.ForName("Name").Eq("Gavin King"))
				.SetProjection(Expression.Property.ForName("StudentNumber"));

			ISession session = OpenSession();
			ITransaction t = session.BeginTransaction();

			Student gavin = new Student();
			gavin.Name = "Gavin King";
			gavin.StudentNumber = 232;
			Student bizarroGavin = new Student();
			bizarroGavin.Name = "Gavin King";
			bizarroGavin.StudentNumber = 666;
			session.Save(bizarroGavin);
			session.Save(gavin);

			IList result = dc
				.Clone()
				.AddOrder(Order.Asc("StudentNumber"))
				.GetExecutableCriteria(session)
				.SetMaxResults(3)
				.List();

			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(232L, result[0]);
			Assert.AreEqual(666L, result[1]);

			int count = (int)dc
				.Clone()
				.SetProjection(Projections.RowCount())
				.GetExecutableCriteria(session)
				.UniqueResult();
			Assert.AreEqual(2, count );

			session.Delete(gavin);
			session.Delete(bizarroGavin);
			t.Commit();
			session.Close();
		}

		[Test]
		public void ProjectionCache()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			Course course = new Course();
			course.CourseCode = "HIB";
			course.Description = "Hibernate Training";
			s.Save(course);

			Student gavin = new Student();
			gavin.Name = "Gavin King";
			gavin.StudentNumber = 666;
			s.Save(gavin);

			Student xam = new Student();
			xam.Name = "Max Rydahl Andersen";
			xam.StudentNumber = 101;
			s.Save(xam);

			Enrolment enrolment1 = new Enrolment();
			enrolment1.Course = course;
			enrolment1.CourseCode = course.CourseCode;
			enrolment1.Semester = 1;
			enrolment1.Year = 1999;
			enrolment1.Student = xam;
			enrolment1.StudentNumber = xam.StudentNumber;
			xam.Enrolments.Add(enrolment1);
			s.Save(enrolment1);

			Enrolment enrolment2 = new Enrolment();
			enrolment2.Course = course;
			enrolment2.CourseCode = course.CourseCode;
			enrolment2.Semester = 3;
			enrolment2.Year = 1998;
			enrolment2.Student = gavin;
			enrolment2.StudentNumber = gavin.StudentNumber;
			gavin.Enrolments.Add(enrolment2);
			s.Save(enrolment2);

			IList list = s.CreateCriteria(typeof(Enrolment))
				.CreateAlias("Student", "s")
				.CreateAlias("Course", "c")
				.Add(Expression.Expression.IsNotEmpty("s.Enrolments"))
				.SetProjection(Projections.ProjectionList()
				               	.Add(Projections.Property("s.Name"))
				               	.Add(Projections.Property("c.Description")))
				.SetCacheable(true)
				.List();

			Assert.AreEqual(2, list.Count);
			Assert.AreEqual(2, ((object[]) list[0]).Length);
			Assert.AreEqual(2, ((object[]) list[1]).Length);

			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			s.CreateCriteria(typeof(Enrolment))
				.CreateAlias("Student", "s")
				.CreateAlias("Course", "c")
				.Add(Expression.Expression.IsNotEmpty("s.Enrolments"))
				.SetProjection(Projections.ProjectionList()
				               	.Add(Projections.Property("s.Name"))
				               	.Add(Projections.Property("c.Description")))
				.SetCacheable(true)
				.List();

			Assert.AreEqual(2, list.Count);
			Assert.AreEqual(2, ((object[]) list[0]).Length);
			Assert.AreEqual(2, ((object[]) list[1]).Length);

			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			s.CreateCriteria(typeof(Enrolment))
				.CreateAlias("Student", "s")
				.CreateAlias("Course", "c")
				.Add(Expression.Expression.IsNotEmpty("s.Enrolments"))
				.SetProjection(Projections.ProjectionList()
				               	.Add(Projections.Property("s.Name"))
				               	.Add(Projections.Property("c.Description")))
				.SetCacheable(true)
				.List();

			Assert.AreEqual(2, list.Count);
			Assert.AreEqual(2, ((object[]) list[0]).Length);
			Assert.AreEqual(2, ((object[]) list[1]).Length);

			s.Delete(enrolment1);
			s.Delete(enrolment2);
			s.Delete(course);
			s.Delete(gavin);
			s.Delete(xam);

			t.Commit();
			s.Close();
		}

		[Test]
		public void ProjectionsTest()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			Course course = new Course();
			course.CourseCode = "HIB";
			course.Description = "Hibernate Training";
			s.Save(course);

			Student gavin = new Student();
			gavin.Name = "Gavin King";
			gavin.StudentNumber = 667;
			s.Save(gavin);

			Student xam = new Student();
			xam.Name = "Max Rydahl Andersen";
			xam.StudentNumber = 101;
			s.Save(xam);

			Enrolment enrolment = new Enrolment();
			enrolment.Course = course;
			enrolment.CourseCode = course.CourseCode;
			enrolment.Semester = 1;
			enrolment.Year = 1999;
			enrolment.Student = xam;
			enrolment.StudentNumber = xam.StudentNumber;
			xam.Enrolments.Add(enrolment);
			s.Save(enrolment);

			enrolment = new Enrolment();
			enrolment.Course = course;
			enrolment.CourseCode = course.CourseCode;
			enrolment.Semester = 3;
			enrolment.Year = 1998;
			enrolment.Student = gavin;
			enrolment.StudentNumber = gavin.StudentNumber;
			gavin.Enrolments.Add(enrolment);
			s.Save(enrolment);

			//s.flush();

			int count = (int) s.CreateCriteria(typeof(Enrolment))
			                  	.SetProjection(Projections.Count("StudentNumber").SetDistinct())
			                  	.UniqueResult();
			Assert.AreEqual(2, count);

			object obj = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Projections.ProjectionList()
				               	.Add(Projections.Count("StudentNumber"))
				               	.Add(Projections.Max("StudentNumber"))
				               	.Add(Projections.Min("StudentNumber"))
				               	.Add(Projections.Avg("StudentNumber"))
				)
				.UniqueResult();
			object[] result = (object[]) obj;

			Assert.AreEqual(2, result[0]);
			Assert.AreEqual(667L, result[1]);
			Assert.AreEqual(101L, result[2]);
			Assert.AreEqual(384.0D, (Double) result[3], 0.01D);


			IList resultWithMaps = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Projections.Distinct(Projections.ProjectionList()
				                                    	.Add(Projections.Property("StudentNumber"), "stNumber")
				                                    	.Add(Projections.Property("CourseCode"), "cCode"))
				)
				.Add(Expression.Expression.Gt("StudentNumber", 665L))
				.Add(Expression.Expression.Lt("StudentNumber", 668L))
				.AddOrder(Order.Asc("stNumber"))
				.SetResultTransformer(CriteriaUtil.AliasToEntityMap)
				.List();

			Assert.AreEqual(1, resultWithMaps.Count);
			IDictionary m1 = (IDictionary) resultWithMaps[0];

			Assert.AreEqual(667L, m1["stNumber"]);
			Assert.AreEqual(course.CourseCode, m1["cCode"]);

			resultWithMaps = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Projections.Property("StudentNumber").As("stNumber"))
				.AddOrder(Order.Desc("stNumber"))
				.SetResultTransformer(CriteriaUtil.AliasToEntityMap)
				.List();

			Assert.AreEqual(2, resultWithMaps.Count);
			IDictionary m0 = (IDictionary) resultWithMaps[0];
			m1 = (IDictionary) resultWithMaps[1];

			Assert.AreEqual(101L, m1["stNumber"]);
			Assert.AreEqual(667L, m0["stNumber"]);


			IList resultWithAliasedBean = s.CreateCriteria(typeof(Enrolment))
				.CreateAlias("Student", "st")
				.CreateAlias("Course", "co")
				.SetProjection(Projections.ProjectionList()
				               	.Add(Projections.Property("st.Name"), "studentName")
				               	.Add(Projections.Property("co.Description"), "courseDescription")
				)
				.AddOrder(Order.Desc("studentName"))
				.SetResultTransformer(Transformers.AliasToBean(typeof(StudentDTO)))
				.List();

			Assert.AreEqual(2, resultWithAliasedBean.Count);

			StudentDTO dto = (StudentDTO) resultWithAliasedBean[0];
			Assert.IsNotNull(dto.Description);
			Assert.IsNotNull(dto.Name);

			s.CreateCriteria(typeof(Student))
				.Add(Expression.Expression.Like("Name", "Gavin", MatchMode.Start))
				.AddOrder(Order.Asc("Name"))
				.CreateCriteria("Enrolments", "e")
				.AddOrder(Order.Desc("Year"))
				.AddOrder(Order.Desc("Semester"))
				.CreateCriteria("Course", "c")
				.AddOrder(Order.Asc("Description"))
				.SetProjection(Projections.ProjectionList()
				               	.Add(Projections.Property("this.Name"))
				               	.Add(Projections.Property("e.Year"))
				               	.Add(Projections.Property("e.Semester"))
				               	.Add(Projections.Property("c.CourseCode"))
				               	.Add(Projections.Property("c.Description"))
				)
				.UniqueResult();

			ProjectionList p1 = Projections.ProjectionList()
				.Add(Projections.Count("StudentNumber"))
				.Add(Projections.Max("StudentNumber"))
				.Add(Projections.RowCount());

			ProjectionList p2 = Projections.ProjectionList()
				.Add(Projections.Min("StudentNumber"))
				.Add(Projections.Avg("StudentNumber"))
				.Add(Projections.SqlProjection(
				     	"1 as constOne, count(*) as countStar",
				     	new String[] {"constOne", "countStar"},
				     	new IType[] {NHibernateUtil.Int32, NHibernateUtil.Int32}
				     	));

			object[] array = (object[]) s.CreateCriteria(typeof(Enrolment))
			                            	.SetProjection(Projections.ProjectionList().Add(p1).Add(p2))
			                            	.UniqueResult();

			Assert.AreEqual(7, array.Length);

			IList list = s.CreateCriteria(typeof(Enrolment))
				.CreateAlias("Student", "st")
				.CreateAlias("Course", "co")
				.SetProjection(Projections.ProjectionList()
				               	.Add(Projections.GroupProperty("co.CourseCode"))
				               	.Add(Projections.Count("st.StudentNumber").SetDistinct())
				               	.Add(Projections.GroupProperty("Year"))
				)
				.List();

			Assert.AreEqual(2, list.Count);

			object g = s.CreateCriteria(typeof(Student))
				.Add(Expression.Expression.IdEq(667L))
				.SetFetchMode("enrolments", FetchMode.Join)
				//.setFetchMode("enrolments.course", FetchMode.JOIN) //TODO: would love to make that work...
				.UniqueResult();
			Assert.AreSame(gavin, g);

			s.Delete(gavin);
			s.Delete(xam);
			s.Delete(course);

			t.Commit();
			s.Close();
		}
		
		[Test]
		public void CloningProjectionsTest()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			Course course = new Course();
			course.CourseCode = "HIB";
			course.Description = "Hibernate Training";
			s.Save(course);

			Student gavin = new Student();
			gavin.Name = "Gavin King";
			gavin.StudentNumber = 667;
			s.Save(gavin);

			Student xam = new Student();
			xam.Name = "Max Rydahl Andersen";
			xam.StudentNumber = 101;
			s.Save(xam);

			Enrolment enrolment = new Enrolment();
			enrolment.Course = course;
			enrolment.CourseCode = course.CourseCode;
			enrolment.Semester = 1;
			enrolment.Year = 1999;
			enrolment.Student = xam;
			enrolment.StudentNumber = xam.StudentNumber;
			xam.Enrolments.Add(enrolment);
			s.Save(enrolment);

			enrolment = new Enrolment();
			enrolment.Course = course;
			enrolment.CourseCode = course.CourseCode;
			enrolment.Semester = 3;
			enrolment.Year = 1998;
			enrolment.Student = gavin;
			enrolment.StudentNumber = gavin.StudentNumber;
			gavin.Enrolments.Add(enrolment);
			s.Save(enrolment);

			//s.flush();

			int count = (int) s.CreateCriteria(typeof(Enrolment))
			                  	.SetProjection(Projections.Count("StudentNumber").SetDistinct())
			                  	.Clone()
								.UniqueResult();
			Assert.AreEqual(2, count);

			object obj = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Projections.ProjectionList()
				               	.Add(Projections.Count("StudentNumber"))
				               	.Add(Projections.Max("StudentNumber"))
				               	.Add(Projections.Min("StudentNumber"))
				               	.Add(Projections.Avg("StudentNumber"))
				)
				.Clone()
				.UniqueResult();
			object[] result = (object[]) obj;

			Assert.AreEqual(2, result[0]);
			Assert.AreEqual(667L, result[1]);
			Assert.AreEqual(101L, result[2]);
			Assert.AreEqual(384.0D, (Double) result[3], 0.01D);


			IList resultWithMaps = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Projections.Distinct(Projections.ProjectionList()
				                                    	.Add(Projections.Property("StudentNumber"), "stNumber")
				                                    	.Add(Projections.Property("CourseCode"), "cCode"))
				)
				.Add(Expression.Expression.Gt("StudentNumber", 665L))
				.Add(Expression.Expression.Lt("StudentNumber", 668L))
				.AddOrder(Order.Asc("stNumber"))
				.SetResultTransformer(CriteriaUtil.AliasToEntityMap)
				.Clone()
				.List();

			Assert.AreEqual(1, resultWithMaps.Count);
			IDictionary m1 = (IDictionary) resultWithMaps[0];

			Assert.AreEqual(667L, m1["stNumber"]);
			Assert.AreEqual(course.CourseCode, m1["cCode"]);

			resultWithMaps = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Projections.Property("StudentNumber").As("stNumber"))
				.AddOrder(Order.Desc("stNumber"))
				.SetResultTransformer(CriteriaUtil.AliasToEntityMap)
				.Clone()
				.List();

			Assert.AreEqual(2, resultWithMaps.Count);
			IDictionary m0 = (IDictionary) resultWithMaps[0];
			m1 = (IDictionary) resultWithMaps[1];

			Assert.AreEqual(101L, m1["stNumber"]);
			Assert.AreEqual(667L, m0["stNumber"]);


			IList resultWithAliasedBean = s.CreateCriteria(typeof(Enrolment))
				.CreateAlias("Student", "st")
				.CreateAlias("Course", "co")
				.SetProjection(Projections.ProjectionList()
				               	.Add(Projections.Property("st.Name"), "studentName")
				               	.Add(Projections.Property("co.Description"), "courseDescription")
				)
				.AddOrder(Order.Desc("studentName"))
				.SetResultTransformer(Transformers.AliasToBean(typeof(StudentDTO)))
				.Clone()
				.List();

			Assert.AreEqual(2, resultWithAliasedBean.Count);

			StudentDTO dto = (StudentDTO) resultWithAliasedBean[0];
			Assert.IsNotNull(dto.Description);
			Assert.IsNotNull(dto.Name);

			s.CreateCriteria(typeof(Student))
				.Add(Expression.Expression.Like("Name", "Gavin", MatchMode.Start))
				.AddOrder(Order.Asc("Name"))
				.CreateCriteria("Enrolments", "e")
				.AddOrder(Order.Desc("Year"))
				.AddOrder(Order.Desc("Semester"))
				.CreateCriteria("Course", "c")
				.AddOrder(Order.Asc("Description"))
				.SetProjection(Projections.ProjectionList()
				               	.Add(Projections.Property("this.Name"))
				               	.Add(Projections.Property("e.Year"))
				               	.Add(Projections.Property("e.Semester"))
				               	.Add(Projections.Property("c.CourseCode"))
				               	.Add(Projections.Property("c.Description"))
				)
				.Clone()
				.UniqueResult();

			ProjectionList p1 = Projections.ProjectionList()
				.Add(Projections.Count("StudentNumber"))
				.Add(Projections.Max("StudentNumber"))
				.Add(Projections.RowCount());

			ProjectionList p2 = Projections.ProjectionList()
				.Add(Projections.Min("StudentNumber"))
				.Add(Projections.Avg("StudentNumber"))
				.Add(Projections.SqlProjection(
				     	"1 as constOne, count(*) as countStar",
				     	new String[] {"constOne", "countStar"},
				     	new IType[] {NHibernateUtil.Int32, NHibernateUtil.Int32}
				     	));

			object[] array = (object[]) s.CreateCriteria(typeof(Enrolment))
			                            	.SetProjection(Projections.ProjectionList().Add(p1).Add(p2))
			                            	.Clone()
											.UniqueResult();

			Assert.AreEqual(7, array.Length);

			IList list = s.CreateCriteria(typeof(Enrolment))
				.CreateAlias("Student", "st")
				.CreateAlias("Course", "co")
				.SetProjection(Projections.ProjectionList()
				               	.Add(Projections.GroupProperty("co.CourseCode"))
				               	.Add(Projections.Count("st.StudentNumber").SetDistinct())
				               	.Add(Projections.GroupProperty("Year"))
				)
				.Clone()
				.List();

			Assert.AreEqual(2, list.Count);

			object g = s.CreateCriteria(typeof(Student))
				.Add(Expression.Expression.IdEq(667L))
				.SetFetchMode("enrolments", FetchMode.Join)
				//.setFetchMode("enrolments.course", FetchMode.JOIN) //TODO: would love to make that work...
				.Clone()
				.UniqueResult();
			Assert.AreSame(gavin, g);

			s.Delete(gavin);
			s.Delete(xam);
			s.Delete(course);

			t.Commit();
			s.Close();
		}

		[Test]
		public void ProjectionsUsingProperty()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			Course course = new Course();
			course.CourseCode = "HIB";
			course.Description = "Hibernate Training";
			s.Save(course);

			Student gavin = new Student();
			gavin.Name = "Gavin King";
			gavin.StudentNumber = 667;
			s.Save(gavin);

			Student xam = new Student();
			xam.Name = "Max Rydahl Andersen";
			xam.StudentNumber = 101;
			s.Save(xam);

			Enrolment enrolment = new Enrolment();
			enrolment.Course = course;
			enrolment.CourseCode = course.CourseCode;
			enrolment.Semester = 1;
			enrolment.Year = 1999;
			enrolment.Student = xam;
			enrolment.StudentNumber = xam.StudentNumber;
			xam.Enrolments.Add(enrolment);
			s.Save(enrolment);

			enrolment = new Enrolment();
			enrolment.Course = course;
			enrolment.CourseCode = course.CourseCode;
			enrolment.Semester = 3;
			enrolment.Year = 1998;
			enrolment.Student = gavin;
			enrolment.StudentNumber = gavin.StudentNumber;
			gavin.Enrolments.Add(enrolment);
			s.Save(enrolment);
			s.Flush();

			int count = (int) s.CreateCriteria(typeof(Enrolment))
			                  	.SetProjection(Expression.Property.ForName("StudentNumber").Count().SetDistinct())
			                  	.UniqueResult();
			Assert.AreEqual(2, count);

			object obj = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Projections.ProjectionList()
				               	.Add(Expression.Property.ForName("StudentNumber").Count())
				               	.Add(Expression.Property.ForName("StudentNumber").Max())
				               	.Add(Expression.Property.ForName("StudentNumber").Min())
				               	.Add(Expression.Property.ForName("StudentNumber").Avg())
				)
				.UniqueResult();
			object[] result = (object[]) obj;

			Assert.AreEqual(2, result[0]);
			Assert.AreEqual(667L, result[1]);
			Assert.AreEqual(101L, result[2]);
			Assert.AreEqual(384.0D, (double) result[3], 0.01D);


			s.CreateCriteria(typeof(Enrolment))
				.Add(Expression.Property.ForName("StudentNumber").Gt(665L))
				.Add(Expression.Property.ForName("StudentNumber").Lt(668L))
				.Add(Expression.Property.ForName("CourseCode").Like("HIB", MatchMode.Start))
				.Add(Expression.Property.ForName("Year").Eq((short) 1999))
				.AddOrder(Expression.Property.ForName("StudentNumber").Asc())
				.UniqueResult();

			IList resultWithMaps = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Projections.ProjectionList()
				               	.Add(Expression.Property.ForName("StudentNumber").As("stNumber"))
				               	.Add(Expression.Property.ForName("CourseCode").As("cCode"))
				)
				.Add(Expression.Property.ForName("StudentNumber").Gt(665L))
				.Add(Expression.Property.ForName("StudentNumber").Lt(668L))
				.AddOrder(Expression.Property.ForName("StudentNumber").Asc())
				.SetResultTransformer(CriteriaUtil.AliasToEntityMap)
				.List();

			Assert.AreEqual(1, resultWithMaps.Count);
			IDictionary m1 = (IDictionary) resultWithMaps[0];

			Assert.AreEqual(667L, m1["stNumber"]);
			Assert.AreEqual(course.CourseCode, m1["cCode"]);

			resultWithMaps = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Expression.Property.ForName("StudentNumber").As("stNumber"))
				.AddOrder(Order.Desc("stNumber"))
				.SetResultTransformer(CriteriaUtil.AliasToEntityMap)
				.List();

			Assert.AreEqual(2, resultWithMaps.Count);
			IDictionary m0 = (IDictionary) resultWithMaps[0];
			m1 = (IDictionary) resultWithMaps[1];

			Assert.AreEqual(101L, m1["stNumber"]);
			Assert.AreEqual(667L, m0["stNumber"]);


			IList resultWithAliasedBean = s.CreateCriteria(typeof(Enrolment))
				.CreateAlias("Student", "st")
				.CreateAlias("Course", "co")
				.SetProjection(Projections.ProjectionList()
				               	.Add(Expression.Property.ForName("st.Name").As("studentName"))
				               	.Add(Expression.Property.ForName("co.Description").As("courseDescription"))
				)
				.AddOrder(Order.Desc("studentName"))
				.SetResultTransformer(Transformers.AliasToBean(typeof(StudentDTO)))
				.List();

			Assert.AreEqual(2, resultWithAliasedBean.Count);

			StudentDTO dto = (StudentDTO) resultWithAliasedBean[0];
			Assert.IsNotNull(dto.Description);
			Assert.IsNotNull(dto.Name);

			s.CreateCriteria(typeof(Student))
				.Add(Expression.Expression.Like("Name", "Gavin", MatchMode.Start))
				.AddOrder(Order.Asc("Name"))
				.CreateCriteria("Enrolments", "e")
				.AddOrder(Order.Desc("Year"))
				.AddOrder(Order.Desc("Semester"))
				.CreateCriteria("Course", "c")
				.AddOrder(Order.Asc("Description"))
				.SetProjection(Projections.ProjectionList()
				               	.Add(Expression.Property.ForName("this.Name"))
				               	.Add(Expression.Property.ForName("e.Year"))
				               	.Add(Expression.Property.ForName("e.Semester"))
				               	.Add(Expression.Property.ForName("c.CourseCode"))
				               	.Add(Expression.Property.ForName("c.Description"))
				)
				.UniqueResult();

			ProjectionList p1 = Projections.ProjectionList()
				.Add(Expression.Property.ForName("StudentNumber").Count())
				.Add(Expression.Property.ForName("StudentNumber").Max())
				.Add(Projections.RowCount());

			ProjectionList p2 = Projections.ProjectionList()
				.Add(Expression.Property.ForName("StudentNumber").Min())
				.Add(Expression.Property.ForName("StudentNumber").Avg())
				.Add(Projections.SqlProjection(
				     	"1 as constOne, count(*) as countStar",
				     	new String[] {"constOne", "countStar"},
				     	new IType[] {NHibernateUtil.Int32, NHibernateUtil.Int32}
				     	));

			object[] array = (object[]) s.CreateCriteria(typeof(Enrolment))
			                            	.SetProjection(Projections.ProjectionList().Add(p1).Add(p2))
			                            	.UniqueResult();
			Assert.AreEqual(7, array.Length);

			IList list = s.CreateCriteria(typeof(Enrolment))
				.CreateAlias("Student", "st")
				.CreateAlias("Course", "co")
				.SetProjection(Projections.ProjectionList()
				               	.Add(Expression.Property.ForName("co.CourseCode").Group())
				               	.Add(Expression.Property.ForName("st.StudentNumber").Count().SetDistinct())
				               	.Add(Expression.Property.ForName("Year").Group())
				)
				.List();

			Assert.AreEqual(2, list.Count);

			s.Delete(gavin);
			s.Delete(xam);
			s.Delete(course);

			t.Commit();
			s.Close();
		}

		[Test]
		public void CloningProjectionsUsingProperty()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			Course course = new Course();
			course.CourseCode = "HIB";
			course.Description = "Hibernate Training";
			s.Save(course);

			Student gavin = new Student();
			gavin.Name = "Gavin King";
			gavin.StudentNumber = 667;
			s.Save(gavin);

			Student xam = new Student();
			xam.Name = "Max Rydahl Andersen";
			xam.StudentNumber = 101;
			s.Save(xam);

			Enrolment enrolment = new Enrolment();
			enrolment.Course = course;
			enrolment.CourseCode = course.CourseCode;
			enrolment.Semester = 1;
			enrolment.Year = 1999;
			enrolment.Student = xam;
			enrolment.StudentNumber = xam.StudentNumber;
			xam.Enrolments.Add(enrolment);
			s.Save(enrolment);

			enrolment = new Enrolment();
			enrolment.Course = course;
			enrolment.CourseCode = course.CourseCode;
			enrolment.Semester = 3;
			enrolment.Year = 1998;
			enrolment.Student = gavin;
			enrolment.StudentNumber = gavin.StudentNumber;
			gavin.Enrolments.Add(enrolment);
			s.Save(enrolment);
			s.Flush();

			int count = (int) s.CreateCriteria(typeof(Enrolment))
			                  	.SetProjection(Expression.Property.ForName("StudentNumber").Count().SetDistinct())
								.Clone()
			                  	.UniqueResult();
			Assert.AreEqual(2, count);

			object obj = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Projections.ProjectionList()
				               	.Add(Expression.Property.ForName("StudentNumber").Count())
				               	.Add(Expression.Property.ForName("StudentNumber").Max())
				               	.Add(Expression.Property.ForName("StudentNumber").Min())
				               	.Add(Expression.Property.ForName("StudentNumber").Avg())
				)
				.Clone()
				.UniqueResult();
			object[] result = (object[]) obj;

			Assert.AreEqual(2, result[0]);
			Assert.AreEqual(667L, result[1]);
			Assert.AreEqual(101L, result[2]);
			Assert.AreEqual(384.0D, (double) result[3], 0.01D);


			s.CreateCriteria(typeof(Enrolment))
				.Add(Expression.Property.ForName("StudentNumber").Gt(665L))
				.Add(Expression.Property.ForName("StudentNumber").Lt(668L))
				.Add(Expression.Property.ForName("CourseCode").Like("HIB", MatchMode.Start))
				.Add(Expression.Property.ForName("Year").Eq((short) 1999))
				.AddOrder(Expression.Property.ForName("StudentNumber").Asc())
				.Clone()
				.UniqueResult();

			IList resultWithMaps = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Projections.ProjectionList()
				               	.Add(Expression.Property.ForName("StudentNumber").As("stNumber"))
				               	.Add(Expression.Property.ForName("CourseCode").As("cCode"))
				)
				.Add(Expression.Property.ForName("StudentNumber").Gt(665L))
				.Add(Expression.Property.ForName("StudentNumber").Lt(668L))
				.AddOrder(Expression.Property.ForName("StudentNumber").Asc())
				.SetResultTransformer(CriteriaUtil.AliasToEntityMap)
				.Clone()
				.List();

			Assert.AreEqual(1, resultWithMaps.Count);
			IDictionary m1 = (IDictionary) resultWithMaps[0];

			Assert.AreEqual(667L, m1["stNumber"]);
			Assert.AreEqual(course.CourseCode, m1["cCode"]);

			resultWithMaps = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Expression.Property.ForName("StudentNumber").As("stNumber"))
				.AddOrder(Order.Desc("stNumber"))
				.SetResultTransformer(CriteriaUtil.AliasToEntityMap)
				.Clone()
				.List();

			Assert.AreEqual(2, resultWithMaps.Count);
			IDictionary m0 = (IDictionary) resultWithMaps[0];
			m1 = (IDictionary) resultWithMaps[1];

			Assert.AreEqual(101L, m1["stNumber"]);
			Assert.AreEqual(667L, m0["stNumber"]);


			IList resultWithAliasedBean = s.CreateCriteria(typeof(Enrolment))
				.CreateAlias("Student", "st")
				.CreateAlias("Course", "co")
				.SetProjection(Projections.ProjectionList()
				               	.Add(Expression.Property.ForName("st.Name").As("studentName"))
				               	.Add(Expression.Property.ForName("co.Description").As("courseDescription"))
				)
				.AddOrder(Order.Desc("studentName"))
				.SetResultTransformer(Transformers.AliasToBean(typeof(StudentDTO)))
				.Clone()
				.List();

			Assert.AreEqual(2, resultWithAliasedBean.Count);

			StudentDTO dto = (StudentDTO) resultWithAliasedBean[0];
			Assert.IsNotNull(dto.Description);
			Assert.IsNotNull(dto.Name);

			s.CreateCriteria(typeof(Student))
				.Add(Expression.Expression.Like("Name", "Gavin", MatchMode.Start))
				.AddOrder(Order.Asc("Name"))
				.CreateCriteria("Enrolments", "e")
				.AddOrder(Order.Desc("Year"))
				.AddOrder(Order.Desc("Semester"))
				.CreateCriteria("Course", "c")
				.AddOrder(Order.Asc("Description"))
				.SetProjection(Projections.ProjectionList()
				               	.Add(Expression.Property.ForName("this.Name"))
				               	.Add(Expression.Property.ForName("e.Year"))
				               	.Add(Expression.Property.ForName("e.Semester"))
				               	.Add(Expression.Property.ForName("c.CourseCode"))
				               	.Add(Expression.Property.ForName("c.Description"))
				)
				.Clone()
				.UniqueResult();

			ProjectionList p1 = Projections.ProjectionList()
				.Add(Expression.Property.ForName("StudentNumber").Count())
				.Add(Expression.Property.ForName("StudentNumber").Max())
				.Add(Projections.RowCount());

			ProjectionList p2 = Projections.ProjectionList()
				.Add(Expression.Property.ForName("StudentNumber").Min())
				.Add(Expression.Property.ForName("StudentNumber").Avg())
				.Add(Projections.SqlProjection(
				     	"1 as constOne, count(*) as countStar",
				     	new String[] {"constOne", "countStar"},
				     	new IType[] {NHibernateUtil.Int32, NHibernateUtil.Int32}
				     	));

			object[] array = (object[]) s.CreateCriteria(typeof(Enrolment))
			                            	.SetProjection(Projections.ProjectionList().Add(p1).Add(p2))
			                            	.Clone()
											.UniqueResult();
			Assert.AreEqual(7, array.Length);

			IList list = s.CreateCriteria(typeof(Enrolment))
				.CreateAlias("Student", "st")
				.CreateAlias("Course", "co")
				.SetProjection(Projections.ProjectionList()
				               	.Add(Expression.Property.ForName("co.CourseCode").Group())
				               	.Add(Expression.Property.ForName("st.StudentNumber").Count().SetDistinct())
				               	.Add(Expression.Property.ForName("Year").Group())
				)
				.Clone()
				.List();

			Assert.AreEqual(2, list.Count);

			s.Delete(gavin);
			s.Delete(xam);
			s.Delete(course);

			t.Commit();
			s.Close();
		}

		[Test]
		public void RestrictionOnSubclassCollection()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			s.CreateCriteria(typeof(Reptile))
				.Add(Expression.Expression.IsEmpty("offspring"))
				.List();

			s.CreateCriteria(typeof(Reptile))
				.Add(Expression.Expression.IsNotEmpty("offspring"))
				.List();

			t.Rollback();
			s.Close();
		}

		[Test]
		public void ClassProperty()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			// HQL: from Animal a where a.mother.class = Reptile
			ICriteria c = s.CreateCriteria(typeof(Animal), "a")
				.CreateAlias("mother", "m")
				.Add(Expression.Property.ForName("m.class").Eq(typeof(Reptile)));
			c.List();
			t.Rollback();
			s.Close();
		}

		[Test]
		public void ProjectedId()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.CreateCriteria(typeof(Course)).SetProjection(Projections.Property("CourseCode")).List();
			s.CreateCriteria(typeof(Course)).SetProjection(Projections.Id()).List();
			t.Rollback();
			s.Close();
		}

		[Test]
		public void CloningProjectedId()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.CreateCriteria(typeof(Course)).SetProjection(Projections.Property("CourseCode")).List();
			s.CreateCriteria(typeof(Course)).SetProjection(Projections.Id()).Clone().List();
			t.Rollback();
			s.Close();
		}

		[Test]
		public void CloningSubcriteriaJoinTypes()
		{
			ISession session = OpenSession();
			ITransaction t = session.BeginTransaction();

			Course courseA = new Course();
			courseA.CourseCode = "HIB-A";
			courseA.Description = "Hibernate Training A";
			session.Save(courseA);

			Course courseB = new Course();
			courseB.CourseCode = "HIB-B";
			courseB.Description = "Hibernate Training B";
			session.Save(courseB);

			Student gavin = new Student();
			gavin.Name = "Gavin King";
			gavin.StudentNumber = 232;
			gavin.PreferredCourse = courseA;
			session.Save(gavin);

			Student leonardo = new Student();
			leonardo.Name = "Leonardo Quijano";
			leonardo.StudentNumber = 233;
			leonardo.PreferredCourse = courseB;
			session.Save(leonardo);

			Student johnDoe = new Student();
			johnDoe.Name = "John Doe";
			johnDoe.StudentNumber = 235;
			johnDoe.PreferredCourse = null;
			session.Save(johnDoe);

			ICriteria criteria = session.CreateCriteria(typeof(Student))
				.SetProjection(Expression.Property.ForName("PreferredCourse.CourseCode"))
				.CreateCriteria("PreferredCourse", JoinType.LeftOuterJoin)
				.AddOrder(Order.Asc("CourseCode"));

			IList result = criteria.Clone().List();

			Assert.AreEqual(3, result.Count);

			t.Rollback();
			session.Dispose();
		}

		[Test]
		public void SubcriteriaJoinTypes()
		{
			ISession session = OpenSession();
			ITransaction t = session.BeginTransaction();

			Course courseA = new Course();
			courseA.CourseCode = "HIB-A";
			courseA.Description = "Hibernate Training A";
			session.Save(courseA);

			Course courseB = new Course();
			courseB.CourseCode = "HIB-B";
			courseB.Description = "Hibernate Training B";
			session.Save(courseB);

			Student gavin = new Student();
			gavin.Name = "Gavin King";
			gavin.StudentNumber = 232;
			gavin.PreferredCourse = courseA;
			session.Save(gavin);

			Student leonardo = new Student();
			leonardo.Name = "Leonardo Quijano";
			leonardo.StudentNumber = 233;
			leonardo.PreferredCourse = courseB;
			session.Save(leonardo);

			Student johnDoe = new Student();
			johnDoe.Name = "John Doe";
			johnDoe.StudentNumber = 235;
			johnDoe.PreferredCourse = null;
			session.Save(johnDoe);

			IList result = session.CreateCriteria(typeof(Student))
				.SetProjection(Expression.Property.ForName("PreferredCourse.CourseCode"))
				.CreateCriteria("PreferredCourse", JoinType.LeftOuterJoin)
				.AddOrder(Order.Asc("CourseCode"))
				.List();
			Assert.AreEqual(3, result.Count);
			// can't be sure of NULL comparison ordering aside from they should
			// either come first or last
			if (result[0] == null)
			{
				Assert.AreEqual("HIB-A", result[1]);
				Assert.AreEqual("HIB-B", result[2]);
			}
			else
			{
				Assert.IsNull(result[2]);
				Assert.AreEqual("HIB-A", result[0]);
				Assert.AreEqual("HIB-B", result[1]);
			}

			result = session.CreateCriteria(typeof(Student))
				.SetFetchMode("PreferredCourse", FetchMode.Join)
				.CreateCriteria("PreferredCourse", JoinType.LeftOuterJoin)
				.AddOrder(Order.Asc("CourseCode"))
				.List();
			Assert.AreEqual(3, result.Count);
			Assert.IsNotNull(result[0]);
			Assert.IsNotNull(result[1]);
			Assert.IsNotNull(result[2]);

			result = session.CreateCriteria(typeof(Student))
				.SetFetchMode("PreferredCourse", FetchMode.Join)
				.CreateAlias("PreferredCourse", "pc", JoinType.LeftOuterJoin)
				.AddOrder(Order.Asc("pc.CourseCode"))
				.List();
			Assert.AreEqual(3, result.Count);
			Assert.IsNotNull(result[0]);
			Assert.IsNotNull(result[1]);
			Assert.IsNotNull(result[2]);

			session.Delete(gavin);
			session.Delete(leonardo);
			session.Delete(johnDoe);
			session.Delete(courseA);
			session.Delete(courseB);
			t.Commit();
			session.Close();
		}

		[Test, ExpectedException(typeof(QueryException))]
		public void TypeMismatch()
		{
			using (ISession session = OpenSession())
			{
				session.CreateCriteria(typeof(Enrolment))
					.Add(Expression.Expression.Eq("Student", 10)) // Type mismatch!
					.List();
			}
		}

		[Test]
		public void PropertySubClassDiscriminator()
		{
			using (ISession s = OpenSession())
			{
				MaterialUnitable bo1 = new MaterialUnitable();
				bo1.Description = "Seal";
				MaterialUnitable bo2 = new MaterialUnitable();
				bo2.Description = "Meter";
				MaterialUnitable dv = new DeviceDef();
				dv.Description = "Printer";
				s.Save(bo1);
				s.Save(bo2);
				s.Save(dv);
				s.Flush();

				MaterialUnit mu = new MaterialUnit(bo1, "S1");
				s.Save(mu);
				mu = new MaterialUnit(bo1, "S2");
				s.Save(mu);
				mu = new MaterialUnit(bo2, "M1");
				s.Save(mu);
				mu = new MaterialUnit(dv, "D1");
				s.Save(mu);
				s.Flush();
			}

			using (ISession session = OpenSession())
			{
				IList l = session.CreateCriteria(typeof(MaterialUnit), "mu")
					.CreateAlias("mu.Material", "ma")
					.Add(Expression.Property.ForName("ma.class").Eq(typeof(MaterialUnitable)))
					.List();
				Assert.AreEqual(3, l.Count);
			}

			using (ISession s = OpenSession())
			{
				s.Delete("from MaterialUnit");
				s.Delete("from MaterialResource");
				s.Flush();
			}
		}
	}
}