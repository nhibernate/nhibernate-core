using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Dialect;
using NHibernate.Criterion;
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

		[Test]
		public void EscapeCharacter()
		{
			Course c1 = new Course();
			c1.CourseCode = "course-1";
			c1.Description = "%1";
			Course c2 = new Course();
			c2.CourseCode = "course-2";
			c2.Description = "%2";
			Course c3 = new Course();
			c3.CourseCode = "course-3";
			c3.Description = "control";

			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				session.Save(c1);
				session.Save(c2);
				session.Save(c3);
				t.Commit();
			}

			using (ISession session = OpenSession())
			{
				// finds all courses which have a description equal to '%1'
				Course example = new Course();
				example.Description = "&%1";
				IList result =
					session.CreateCriteria(typeof(Course)).Add(
						Example.Create(example).IgnoreCase().EnableLike().SetEscapeCharacter('&')).List();
				Assert.AreEqual(1, result.Count);
			}

			using (ISession session = OpenSession())
			{
				// finds all courses which contain '%' as the first char in the description
				Course example = new Course();
				example.Description = "&%%";
				IList result =
					session.CreateCriteria(typeof(Course)).Add(
						Example.Create(example).IgnoreCase().EnableLike().SetEscapeCharacter('&')).List();
				Assert.AreEqual(2, result.Count);
			}

			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				session.Delete(c1);
				session.Delete(c2);
				session.Delete(c3);
				t.Commit();
			}
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
		public void AllowToSetLimitOnSubqueries()
		{
			using (ISession session = OpenSession())
			{
				DetachedCriteria dc = DetachedCriteria.For(typeof(Student))
					.Add(Property.ForName("StudentNumber").Eq(232L))
					.SetMaxResults(1)
					.AddOrder(Order.Asc("Name"))
					.SetProjection(Property.ForName("Name"));

				session.CreateCriteria(typeof(Student))
					.Add(Subqueries.PropertyEq("Name", dc))
					.List();
			}
		}

		[Test]
		public void TestSubcriteriaBeingNull()
		{
			ISession session = OpenSession();
			ITransaction t = session.BeginTransaction();

			Course hibernateCourse = new Course();
			hibernateCourse.CourseCode = "HIB";
			hibernateCourse.Description = "Hibernate Training";
			session.Save(hibernateCourse);

			DetachedCriteria subcriteria = DetachedCriteria.For<Enrolment>("e");
			subcriteria.Add(Expression.EqProperty("e.CourseCode", "c.CourseCode"));
			subcriteria.SetProjection(Projections.Avg("Semester"));

			DetachedCriteria criteria = DetachedCriteria.For<Course>("c");
			criteria.SetProjection(Projections.Count("id"));
			criteria.Add(Expression.Or(Subqueries.Le(5, subcriteria), Subqueries.IsNull(subcriteria)));

			object o = criteria.GetExecutableCriteria(session).UniqueResult();
			Assert.AreEqual(1, o);

			session.Delete(hibernateCourse);
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
				.Add(Property.ForName("StudentNumber").Eq(232L))
				.SetProjection(Property.ForName("Name"));

			if (TestDialect.SupportsOperatorAll)
			{
				session.CreateCriteria(typeof (Student))
					.Add(Subqueries.PropertyEqAll("Name", dc))
					.List();
			}

			session.CreateCriteria(typeof(Student))
				.Add(Subqueries.Exists(dc))
				.List();

			if (TestDialect.SupportsOperatorAll)
			{
				session.CreateCriteria(typeof (Student))
					.Add(Property.ForName("Name").EqAll(dc))
					.List();
			}

			session.CreateCriteria(typeof(Student))
				.Add(Subqueries.In("Gavin King", dc))
				.List();

			DetachedCriteria dc2 = DetachedCriteria.For(typeof(Student), "st")
				.Add(Property.ForName("st.StudentNumber").EqProperty("e.StudentNumber"))
				.SetProjection(Property.ForName("Name"));

			session.CreateCriteria(typeof(Enrolment), "e")
				.Add(Subqueries.Eq("Gavin King", dc2))
				.List();

			DetachedCriteria dc3 = DetachedCriteria.For(typeof(Student), "st")
				.CreateCriteria("Enrolments")
				.CreateCriteria("Course")
				.Add(Property.ForName("Description").Eq("Hibernate Training"))
				.SetProjection(Property.ForName("st.Name"));

			session.CreateCriteria(typeof(Enrolment), "e")
				.Add(Subqueries.Eq("Gavin King", dc3))
				.List();

			DetachedCriteria courseCriteria = DetachedCriteria.For(typeof(Course))
				.Add(Property.ForName("Description").Eq("Hibernate Training"))
				.SetProjection(Projections.Property("CourseCode"));

			DetachedCriteria enrolmentCriteria = DetachedCriteria.For(typeof(Enrolment))
				.Add(Property.ForName("CourseCode").Eq(courseCriteria))
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
		public void SubselectWithComponent()
		{
			Course course = null;
			Student gavin = null;
			DetachedCriteria dc = null;
			CityState odessaWa = null;
			Enrolment enrolment2 = null;
			
			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				course = new Course();
				course.CourseCode = "HIB";
				course.Description = "Hibernate Training";
				session.Save(course);
				
				odessaWa = new CityState("Odessa", "WA");
	
				gavin = new Student();
				gavin.Name = "Gavin King";
				gavin.StudentNumber = 232;
				gavin.CityState = odessaWa;
				session.Save(gavin);
	
				enrolment2 = new Enrolment();
				enrolment2.Course = course;
				enrolment2.CourseCode = course.CourseCode;
				enrolment2.Semester = 3;
				enrolment2.Year = 1998;
				enrolment2.Student = gavin;
				enrolment2.StudentNumber = gavin.StudentNumber;
				gavin.Enrolments.Add(enrolment2);
				
				session.Persist(enrolment2);
	
				dc = DetachedCriteria.For<Student>()
					.Add(Property.ForName("CityState").Eq(odessaWa))
					.SetProjection(Property.ForName("CityState"));
		
				session.CreateCriteria<Student>()
					.Add(Subqueries.Exists(dc))
					.List();
				
				t.Commit();
			}

			if (TestDialect.SupportsOperatorAll)
			{
				using (ISession session = OpenSession())
				using (ITransaction t = session.BeginTransaction())
				{
					try
					{
						session.CreateCriteria<Student>()
							.Add(Subqueries.PropertyEqAll("CityState", dc))
							.List();

						Assert.Fail("should have failed because cannot compare subquery results with multiple columns");
					}
					catch (QueryException)
					{
						// expected
					}
					t.Rollback();
				}
			}

			if (TestDialect.SupportsOperatorAll)
			{
				using (ISession session = OpenSession())
				using (ITransaction t = session.BeginTransaction())
				{
					try
					{
						session.CreateCriteria<Student>()
							.Add(Property.ForName("CityState").EqAll(dc))
							.List();

						Assert.Fail("should have failed because cannot compare subquery results with multiple columns");
					}
					catch (QueryException)
					{
						// expected
					}
					finally
					{
						t.Rollback();
					}
				}
			}

			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				try
				{
					session.CreateCriteria<Student>()
						.Add(Subqueries.In(odessaWa, dc))
						.List();
					
					Assert.Fail("should have failed because cannot compare subquery results with multiple columns");
				}
				catch (NHibernate.Exceptions.GenericADOException)
				{
					// expected
				}
				finally
				{
					t.Rollback();
				}
			}
	
			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				DetachedCriteria dc2 = DetachedCriteria.For<Student>("st1")
					.Add(Property.ForName("st1.CityState").EqProperty("st2.CityState"))
					.SetProjection(Property.ForName("CityState"));
				
				try 
				{
					session.CreateCriteria<Student>("st2")
						.Add( Subqueries.Eq(odessaWa, dc2))
						.List();
					Assert.Fail("should have failed because cannot compare subquery results with multiple columns");
				}
				catch (NHibernate.Exceptions.GenericADOException)
				{
					// expected
				}
				finally
				{
					t.Rollback();
				}
			}
	
			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				DetachedCriteria dc3 = DetachedCriteria.For<Student>("st")
					.CreateCriteria("Enrolments")
						.CreateCriteria("Course")
							.Add(Property.ForName("Description").Eq("Hibernate Training"))
							.SetProjection(Property.ForName("st.CityState"));
				try
				{
					session.CreateCriteria<Enrolment>("e")
						.Add(Subqueries.Eq(odessaWa, dc3))
						.List();
					
					Assert.Fail("should have failed because cannot compare subquery results with multiple columns");
				}
				catch (NHibernate.Exceptions.GenericADOException)
				{
					// expected
				}
				finally
				{
					t.Rollback();
				}
			}
	
			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				session.Delete(enrolment2);
				session.Delete(gavin);
				session.Delete(course);
				t.Commit();
			}
		}
		
		[Test]
		public void CloningCriteria()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			// HQL: from Animal a where a.mother.class = Reptile
			ICriteria c = s.CreateCriteria(typeof(Animal), "a")
				.CreateAlias("mother", "m")
				.Add(Property.ForName("m.class").Eq(typeof(Reptile)));
			ICriteria cloned = CriteriaTransformer.Clone(c);
			cloned.List();
			t.Rollback();
			s.Close();
		}

		[Test]
		public void CloningCriteria_AddCount_RemoveOrdering()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			// HQL: from Animal a where a.mother.class = Reptile
			ICriteria c = s.CreateCriteria(typeof(Animal), "a")
				.CreateAlias("mother", "m")
				.Add(Property.ForName("m.class").Eq(typeof(Reptile)))
				.AddOrder(Order.Asc("a.bodyWeight"));
			ICriteria cloned = CriteriaTransformer.TransformToRowCount(c);


			cloned.List();
			t.Rollback();
			s.Close();
		}

		[Test]
		public void DetachedCriteriaTest()
		{
			DetachedCriteria dc = DetachedCriteria.For(typeof(Student))
				.Add(Property.ForName("Name").Eq("Gavin King"))
				.AddOrder(Order.Asc("StudentNumber"))
				.SetProjection(Property.ForName("StudentNumber"));

			byte[] bytes = SerializationHelper.Serialize(dc);

			dc = (DetachedCriteria)SerializationHelper.Deserialize(bytes);

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
		public void SubqueryPaginationOnlyWithFirst()
		{
			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				session.Save(new Student { Name = "Mengano", StudentNumber = 232 });
				session.Save(new Student { Name = "Ayende", StudentNumber = 999 });
				session.Save(new Student { Name = "Fabio", StudentNumber = 123 });
				session.Save(new Student { Name = "Merlo", StudentNumber = 456 });
				session.Save(new Student { Name = "Fulano", StudentNumber = 0 });

				t.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				DetachedCriteria dc = DetachedCriteria.For(typeof(Student))
					.Add(Property.ForName("StudentNumber").Gt(0L))
					.SetFirstResult(1)
					.AddOrder(Order.Asc("StudentNumber"))
					.SetProjection(Property.ForName("Name"));

				var result = session.CreateCriteria(typeof(Student))
					.Add(Subqueries.PropertyIn("Name", dc))
					.List<Student>();

				Assert.That(result.Count, Is.EqualTo(3));
				t.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				session.CreateQuery("delete from Student").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void SubqueryPagination()
		{
			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				session.Save(new Student { Name = "Mengano", StudentNumber = 232 });
				session.Save(new Student { Name = "Ayende", StudentNumber = 999 });
				session.Save(new Student { Name = "Fabio", StudentNumber = 123 });
				session.Save(new Student { Name = "Merlo", StudentNumber = 456 });
				session.Save(new Student { Name = "Fulano", StudentNumber = 0 });

				t.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				DetachedCriteria dc = DetachedCriteria.For(typeof (Student))
					.Add(Property.ForName("StudentNumber").Gt(200L))
					.SetMaxResults(2)
					.SetFirstResult(1)
					.AddOrder(Order.Asc("StudentNumber"))
					.SetProjection(Property.ForName("Name"));

				var result = session.CreateCriteria(typeof(Student))
					.Add(Subqueries.PropertyIn("Name", dc))
					.AddOrder(Order.Asc("StudentNumber"))
					.List<Student>();

				Assert.That(result.Count, Is.EqualTo(2));
				Assert.That(result[0].StudentNumber, Is.EqualTo(456));
				Assert.That(result[1].StudentNumber, Is.EqualTo(999));

				t.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				session.CreateQuery("delete from Student").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void SimplePagination()
		{
			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				session.Save(new Student {Name = "Mengano", StudentNumber = 232});
				session.Save(new Student {Name = "Ayende", StudentNumber = 999});
				session.Save(new Student {Name = "Fabio", StudentNumber = 123});
				session.Save(new Student {Name = "Merlo", StudentNumber = 456});
				session.Save(new Student {Name = "Fulano", StudentNumber = 0});

				t.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				var result = session.CreateCriteria<Student>()
					.Add(Restrictions.Gt("StudentNumber", 0L))
					.AddOrder(Order.Asc("StudentNumber"))
					.SetFirstResult(1).SetMaxResults(2)
					.List<Student>();
				Assert.That(result.Count, Is.EqualTo(2));
				Assert.That(result[0].StudentNumber, Is.EqualTo(232));
				Assert.That(result[1].StudentNumber, Is.EqualTo(456));

				t.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				session.CreateQuery("delete from Student").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void SimplePaginationOnlyWithFirst()
		{
			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				session.Save(new Student {Name = "Mengano", StudentNumber = 232});
				session.Save(new Student {Name = "Ayende", StudentNumber = 999});
				session.Save(new Student {Name = "Fabio", StudentNumber = 123});
				session.Save(new Student {Name = "Merlo", StudentNumber = 456});
				session.Save(new Student {Name = "Fulano", StudentNumber = 0});

				t.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				var result = session.CreateCriteria<Student>()
					.Add(Restrictions.Gt("StudentNumber", 0L))
					.AddOrder(Order.Asc("StudentNumber"))
					.SetFirstResult(1)
					.List<Student>();

				Assert.That(result.Count, Is.EqualTo(3));
				Assert.That(result[0].StudentNumber, Is.EqualTo(232));
				Assert.That(result[1].StudentNumber, Is.EqualTo(456));
				Assert.That(result[2].StudentNumber, Is.EqualTo(999));

				t.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				session.CreateQuery("delete from Student").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void CloningDetachedCriteriaTest()
		{
			DetachedCriteria dc = DetachedCriteria.For(typeof(Student))
				.Add(Property.ForName("Name").Eq("Gavin King"))
				.SetProjection(Property.ForName("StudentNumber"));

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

			IList result = CriteriaTransformer.Clone(dc)
				.AddOrder(Order.Asc("StudentNumber"))
				.GetExecutableCriteria(session)
				.SetMaxResults(3)
				.List();

			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(232L, result[0]);
			Assert.AreEqual(666L, result[1]);

			int count = (int)CriteriaTransformer.Clone(dc)
								.SetProjection(Projections.RowCount())
								.GetExecutableCriteria(session)
								.UniqueResult();
			Assert.AreEqual(2, count);

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
				.Add(Expression.IsNotEmpty("s.Enrolments"))
				.SetProjection(Projections.ProjectionList()
								.Add(Projections.Property("s.Name"))
								.Add(Projections.Property("c.Description")))
				.SetCacheable(true)
				.List();

			Assert.AreEqual(2, list.Count);
			Assert.AreEqual(2, ((object[])list[0]).Length);
			Assert.AreEqual(2, ((object[])list[1]).Length);

			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			s.CreateCriteria(typeof(Enrolment))
				.CreateAlias("Student", "s")
				.CreateAlias("Course", "c")
				.Add(Expression.IsNotEmpty("s.Enrolments"))
				.SetProjection(Projections.ProjectionList()
								.Add(Projections.Property("s.Name"))
								.Add(Projections.Property("c.Description")))
				.SetCacheable(true)
				.List();

			Assert.AreEqual(2, list.Count);
			Assert.AreEqual(2, ((object[])list[0]).Length);
			Assert.AreEqual(2, ((object[])list[1]).Length);

			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			s.CreateCriteria(typeof(Enrolment))
				.CreateAlias("Student", "s")
				.CreateAlias("Course", "c")
				.Add(Expression.IsNotEmpty("s.Enrolments"))
				.SetProjection(Projections.ProjectionList()
								.Add(Projections.Property("s.Name"))
								.Add(Projections.Property("c.Description")))
				.SetCacheable(true)
				.List();

			Assert.AreEqual(2, list.Count);
			Assert.AreEqual(2, ((object[])list[0]).Length);
			Assert.AreEqual(2, ((object[])list[1]).Length);

			s.Delete(enrolment1);
			s.Delete(enrolment2);
			s.Delete(course);
			s.Delete(gavin);
			s.Delete(xam);

			t.Commit();
			s.Close();
		}

		[Test, Ignore("Not supported.")]
		public void NH_1155_ShouldNotLoadAllChildrenInPagedSubSelect()
		{
			if (this.Dialect.GetType().Equals((typeof(MsSql2000Dialect))))
				Assert.Ignore("This is not fixed for SQL 2000 Dialect");

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Course course = new Course();
				course.CourseCode = "HIB";
				course.Description = "Hibernate Training";
				s.Save(course);


				Student gavin = new Student();
				gavin.Name = "Gavin King";
				gavin.StudentNumber = 667;
				s.Save(gavin);

				Student ayende = new Student();
				ayende.Name = "Ayende Rahien";
				ayende.StudentNumber = 1337;
				s.Save(ayende);


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
				enrolment.Student = ayende;
				enrolment.StudentNumber = ayende.StudentNumber;
				ayende.Enrolments.Add(enrolment);
				s.Save(enrolment);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				IList<Student> list = s.CreateCriteria(typeof(Student))
					.SetFirstResult(1)
					.SetMaxResults(10)
					.AddOrder(Order.Asc("StudentNumber"))
					.List<Student>();
				foreach (Student student in list)
				{
					foreach (Enrolment enrolment in student.Enrolments)
					{
						NHibernateUtil.Initialize(enrolment);
					}
				}

				Enrolment key = new Enrolment();
				key.CourseCode = "HIB";
				key.StudentNumber = 101;// xam
				//since we didn't load xam's entrollments before (skipped by orderring)
				//it should not be already loaded
				Enrolment shouldNotBeLoaded = (Enrolment)s.Load(typeof(Enrolment), key);
				Assert.IsFalse(NHibernateUtil.IsInitialized(shouldNotBeLoaded));
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Enrolment");
				s.Delete("from Student");
				s.Delete("from Course");
				tx.Commit();
			}
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

			int count = (int)s.CreateCriteria(typeof(Enrolment))
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
			object[] result = (object[])obj;

			Assert.AreEqual(2, result[0]);
			Assert.AreEqual(667L, result[1]);
			Assert.AreEqual(101L, result[2]);
			Assert.AreEqual(384.0D, (Double)result[3], 0.01D);


			IList resultWithMaps = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Projections.Distinct(Projections.ProjectionList()
														.Add(Projections.Property("StudentNumber"), "stNumber")
														.Add(Projections.Property("CourseCode"), "cCode"))
				)
				.Add(Expression.Gt("StudentNumber", 665L))
				.Add(Expression.Lt("StudentNumber", 668L))
				.AddOrder(Order.Asc("stNumber"))
				.SetResultTransformer(CriteriaSpecification.AliasToEntityMap)
				.List();

			Assert.AreEqual(1, resultWithMaps.Count);
			IDictionary m1 = (IDictionary)resultWithMaps[0];

			Assert.AreEqual(667L, m1["stNumber"]);
			Assert.AreEqual(course.CourseCode, m1["cCode"]);

			resultWithMaps = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Projections.Property("StudentNumber").As("stNumber"))
				.AddOrder(Order.Desc("stNumber"))
				.SetResultTransformer(CriteriaSpecification.AliasToEntityMap)
				.List();

			Assert.AreEqual(2, resultWithMaps.Count);
			IDictionary m0 = (IDictionary)resultWithMaps[0];
			m1 = (IDictionary)resultWithMaps[1];

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

			StudentDTO dto = (StudentDTO)resultWithAliasedBean[0];
			Assert.IsNotNull(dto.Description);
			Assert.IsNotNull(dto.Name);

			s.CreateCriteria(typeof(Student))
				.Add(Expression.Like("Name", "Gavin", MatchMode.Start))
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
						new String[] { "constOne", "countStar" },
						new IType[] { NHibernateUtil.Int32, NHibernateUtil.Int32 }
						));

			object[] array = (object[])s.CreateCriteria(typeof(Enrolment))
											.SetProjection(Projections.ProjectionList().Add(p1).Add(p2))
											.UniqueResult();

			Assert.AreEqual(7, array.Length);

			ProjectionList pp1 = Projections.ProjectionList().Add(Projections.RowCountInt64());


			object r = s.CreateCriteria(typeof(Enrolment))
											.SetProjection(pp1)
											.UniqueResult();
			Assert.AreEqual(typeof(Int64), r.GetType());

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
				.Add(Expression.IdEq(667L))
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

			ICriteria criteriaToBeCloned = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Projections.Count("StudentNumber").SetDistinct());
			int count = (int)CriteriaTransformer.Clone(criteriaToBeCloned)
								.UniqueResult();
			Assert.AreEqual(2, count);

			ICriteria criteriaToClone = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Projections.ProjectionList()
								.Add(Projections.Count("StudentNumber"))
								.Add(Projections.Max("StudentNumber"))
								.Add(Projections.Min("StudentNumber"))
								.Add(Projections.Avg("StudentNumber"))
				);
			object obj = CriteriaTransformer.Clone(criteriaToClone)
				.UniqueResult();
			object[] result = (object[])obj;

			Assert.AreEqual(2, result[0]);
			Assert.AreEqual(667L, result[1]);
			Assert.AreEqual(101L, result[2]);
			Assert.AreEqual(384.0D, (Double)result[3], 0.01D);


			ICriteria criteriaToClone2 = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Projections.Distinct(Projections.ProjectionList()
														.Add(Projections.Property("StudentNumber"), "stNumber")
														.Add(Projections.Property("CourseCode"), "cCode"))
				)
				.Add(Expression.Gt("StudentNumber", 665L))
				.Add(Expression.Lt("StudentNumber", 668L))
				.AddOrder(Order.Asc("stNumber"))
				.SetResultTransformer(CriteriaSpecification.AliasToEntityMap);
			IList resultWithMaps = CriteriaTransformer.Clone(criteriaToClone2)
				.List();

			Assert.AreEqual(1, resultWithMaps.Count);
			IDictionary m1 = (IDictionary)resultWithMaps[0];

			Assert.AreEqual(667L, m1["stNumber"]);
			Assert.AreEqual(course.CourseCode, m1["cCode"]);

			ICriteria criteria = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Projections.Property("StudentNumber").As("stNumber"))
				.AddOrder(Order.Desc("stNumber"))
				.SetResultTransformer(CriteriaSpecification.AliasToEntityMap);
			resultWithMaps = CriteriaTransformer.Clone(criteria)
				.List();

			Assert.AreEqual(2, resultWithMaps.Count);
			IDictionary m0 = (IDictionary)resultWithMaps[0];
			m1 = (IDictionary)resultWithMaps[1];

			Assert.AreEqual(101L, m1["stNumber"]);
			Assert.AreEqual(667L, m0["stNumber"]);


			ICriteria criteriaToClone3 = s.CreateCriteria(typeof(Enrolment))
				.CreateAlias("Student", "st")
				.CreateAlias("Course", "co")
				.SetProjection(Projections.ProjectionList()
								.Add(Projections.Property("st.Name"), "studentName")
								.Add(Projections.Property("co.Description"), "courseDescription")
				)
				.AddOrder(Order.Desc("studentName"))
				.SetResultTransformer(Transformers.AliasToBean(typeof(StudentDTO)));
			IList resultWithAliasedBean = CriteriaTransformer.Clone(criteriaToClone3)
				.List();

			Assert.AreEqual(2, resultWithAliasedBean.Count);

			StudentDTO dto = (StudentDTO)resultWithAliasedBean[0];
			Assert.IsNotNull(dto.Description);
			Assert.IsNotNull(dto.Name);

			ICriteria complexCriteriaToBeCloned = s.CreateCriteria(typeof(Student))
				.Add(Expression.Like("Name", "Gavin", MatchMode.Start))
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
				);
			CriteriaTransformer.Clone(complexCriteriaToBeCloned)
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
						new String[] { "constOne", "countStar" },
						new IType[] { NHibernateUtil.Int32, NHibernateUtil.Int32 }
						));

			object[] array = (object[])CriteriaTransformer.Clone(
											s.CreateCriteria(typeof(Enrolment))
												.SetProjection(Projections.ProjectionList().Add(p1).Add(p2))
											).UniqueResult();

			Assert.AreEqual(7, array.Length);

			ICriteria criteriaToClone5 = s.CreateCriteria(typeof(Enrolment))
				.CreateAlias("Student", "st")
				.CreateAlias("Course", "co")
				.SetProjection(Projections.ProjectionList()
								.Add(Projections.GroupProperty("co.CourseCode"))
								.Add(Projections.Count("st.StudentNumber").SetDistinct())
								.Add(Projections.GroupProperty("Year"))
				);
			IList list = CriteriaTransformer.Clone(criteriaToClone5)
				.List();

			Assert.AreEqual(2, list.Count);

			ICriteria criteriaToClone6 = s.CreateCriteria(typeof(Student))
				.Add(Expression.IdEq(667L))
				.SetFetchMode("enrolments", FetchMode.Join);
			object g = CriteriaTransformer.Clone(criteriaToClone6)
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
			course.CourseMeetings.Add(new CourseMeeting(course, "Monday", 1, "1313 Mockingbird Lane"));
			s.Save(course);

			Student gavin = new Student();
			gavin.Name = "Gavin King";
			gavin.StudentNumber = 667;
			CityState odessaWa = new CityState("Odessa", "WA");
			gavin.CityState = odessaWa;
			gavin.PreferredCourse = course;
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

			// Subtest #1
			IList resultList = s.CreateCriteria<Enrolment>()
				.SetProjection(Projections.ProjectionList()
					.Add(Property.ForName("Student"), "student")
					.Add(Property.ForName("Course"), "course")
					.Add(Property.ForName("Semester"), "semester")
					.Add(Property.ForName("Year"), "year")
				).List();

			Assert.That(resultList.Count, Is.EqualTo(2));
			
			foreach (object[] objects in resultList)
			{
				Assert.That(objects.Length, Is.EqualTo(4));
				Assert.That(objects[0], Is.InstanceOf<Student>());
				Assert.That(objects[1], Is.InstanceOf<Course>());
				Assert.That(objects[2], Is.InstanceOf<short>());
				Assert.That(objects[3], Is.InstanceOf<short>());
			}

			// Subtest #2
			resultList = s.CreateCriteria<Student>()
				.SetProjection(Projections.ProjectionList()
					.Add(Projections.Id().As("StudentNumber"))
					.Add(Property.ForName("Name"), "name")
					.Add(Property.ForName("CityState"), "cityState")
					.Add(Property.ForName("PreferredCourse"), "preferredCourse")
				).List();
			
			Assert.That(resultList.Count, Is.EqualTo(2));
			
			foreach(object[] objects in resultList)
			{
				Assert.That(objects.Length, Is.EqualTo(4));
				Assert.That(objects[0], Is.InstanceOf<long>());
				Assert.That(objects[1], Is.InstanceOf<string>());
				
				if ("Gavin King".Equals(objects[1]))
				{
					Assert.That(objects[2], Is.InstanceOf<CityState>());
					Assert.That(objects[3], Is.InstanceOf<Course>());
				}
				else
				{
					Assert.That(objects[2], Is.Null);
					Assert.That(objects[3], Is.Null);
				}
			}
			
			// Subtest #3
			resultList = s.CreateCriteria<Student>()
				.Add(Restrictions.Eq("Name", "Gavin King"))
				.SetProjection(Projections.ProjectionList()
					.Add(Projections.Id().As("StudentNumber"))
					.Add(Property.ForName("Name"), "name")
					.Add(Property.ForName("CityState"), "cityState")
					.Add(Property.ForName("PreferredCourse"), "preferredCourse")
				).List();
			
			Assert.That(resultList.Count, Is.EqualTo(1));

			// Subtest #4
			object[] aResult = (object[])s.CreateCriteria<Student>()
				.Add(Restrictions.IdEq(667L))
				.SetProjection(Projections.ProjectionList()
					.Add(Projections.Id().As("StudentNumber"))
					.Add(Property.ForName("Name"), "name")
					.Add(Property.ForName("CityState"), "cityState")
					.Add(Property.ForName("PreferredCourse"), "preferredCourse")
				).UniqueResult();
			
			Assert.That(aResult, Is.Not.Null);
			Assert.That(aResult.Length, Is.EqualTo(4));
			Assert.That(aResult[0], Is.InstanceOf<long>());
			Assert.That(aResult[1], Is.InstanceOf<string>());
			Assert.That(aResult[2], Is.InstanceOf<CityState>());
			Assert.That(aResult[3], Is.InstanceOf<Course>());
			
			// Subtest #5
			int count = (int)s.CreateCriteria(typeof(Enrolment))
								.SetProjection(Property.ForName("StudentNumber").Count().SetDistinct())
								.UniqueResult();
			
			Assert.AreEqual(2, count);

			// Subtest #6
			object obj = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Projections.ProjectionList()
								.Add(Property.ForName("StudentNumber").Count())
								.Add(Property.ForName("StudentNumber").Max())
								.Add(Property.ForName("StudentNumber").Min())
								.Add(Property.ForName("StudentNumber").Avg())
				)
				.UniqueResult();
			
			object[] result = (object[])obj;

			Assert.AreEqual(2, result[0]);
			Assert.AreEqual(667L, result[1]);
			Assert.AreEqual(101L, result[2]);
			Assert.AreEqual(384.0D, (double)result[3], 0.01D);

			// Subtest #7
			s.CreateCriteria(typeof(Enrolment))
				.Add(Property.ForName("StudentNumber").Gt(665L))
				.Add(Property.ForName("StudentNumber").Lt(668L))
				.Add(Property.ForName("CourseCode").Like("HIB", MatchMode.Start))
				.Add(Property.ForName("Year").Eq((short)1999))
				.AddOrder(Property.ForName("StudentNumber").Asc())
				.UniqueResult();

			// Subtest #8
			IList resultWithMaps = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Projections.ProjectionList()
								.Add(Property.ForName("StudentNumber").As("stNumber"))
								.Add(Property.ForName("CourseCode").As("cCode"))
				)
				.Add(Property.ForName("StudentNumber").Gt(665L))
				.Add(Property.ForName("StudentNumber").Lt(668L))
				.AddOrder(Property.ForName("StudentNumber").Asc())
				.SetResultTransformer(CriteriaSpecification.AliasToEntityMap)
				.List();

			Assert.AreEqual(1, resultWithMaps.Count);
			
			IDictionary m1 = (IDictionary)resultWithMaps[0];
			Assert.AreEqual(667L, m1["stNumber"]);
			Assert.AreEqual(course.CourseCode, m1["cCode"]);

			// Subtest #9
			resultWithMaps = s.CreateCriteria(typeof(Enrolment))
				.SetProjection(Property.ForName("StudentNumber").As("stNumber"))
				.AddOrder(Order.Desc("stNumber"))
				.SetResultTransformer(CriteriaSpecification.AliasToEntityMap)
				.List();

			Assert.AreEqual(2, resultWithMaps.Count);

			IDictionary m0 = (IDictionary)resultWithMaps[0];
			m1 = (IDictionary)resultWithMaps[1];
			Assert.AreEqual(101L, m1["stNumber"]);
			Assert.AreEqual(667L, m0["stNumber"]);

			// Subtest #10
			IList resultWithAliasedBean = s.CreateCriteria(typeof(Enrolment))
				.CreateAlias("Student", "st")
				.CreateAlias("Course", "co")
				.SetProjection(Projections.ProjectionList()
					.Add(Property.ForName("st.Name").As("studentName"))
					.Add(Property.ForName("co.Description").As("courseDescription"))
				)
				.AddOrder(Order.Desc("studentName"))
				.SetResultTransformer(Transformers.AliasToBean(typeof(StudentDTO)))
				.List();

			Assert.AreEqual(2, resultWithAliasedBean.Count);

			// Subtest #11
			StudentDTO dto = (StudentDTO)resultWithAliasedBean[0];
			Assert.IsNotNull(dto.Description);
			Assert.IsNotNull(dto.Name);

			// Subtest #12
			CourseMeeting courseMeetingDto = s.CreateCriteria<CourseMeeting>()
				.SetProjection(Projections.ProjectionList()
					.Add(Property.ForName("Id").As("id"))
					.Add(Property.ForName("Course").As("course"))
				)
				.AddOrder(Order.Desc("id"))
				.SetResultTransformer(Transformers.AliasToBean<CourseMeeting>())
				.UniqueResult<CourseMeeting>();
	
			Assert.That(courseMeetingDto.Id, Is.Not.Null);
			Assert.That(courseMeetingDto.Id.CourseCode, Is.EqualTo(course.CourseCode));
			Assert.That(courseMeetingDto.Id.Day, Is.EqualTo("Monday"));
			Assert.That(courseMeetingDto.Id.Location, Is.EqualTo("1313 Mockingbird Lane"));
			Assert.That(courseMeetingDto.Id.Period, Is.EqualTo(1));
			Assert.That(courseMeetingDto.Course.Description, Is.EqualTo(course.Description));

			// Subtest #13
			s.CreateCriteria(typeof(Student))
				.Add(Expression.Like("Name", "Gavin", MatchMode.Start))
				.AddOrder(Order.Asc("Name"))
				.CreateCriteria("Enrolments", "e")
				.AddOrder(Order.Desc("Year"))
				.AddOrder(Order.Desc("Semester"))
				.CreateCriteria("Course", "c")
				.AddOrder(Order.Asc("Description"))
				.SetProjection(Projections.ProjectionList()
					.Add(Property.ForName("this.Name"))
					.Add(Property.ForName("e.Year"))
					.Add(Property.ForName("e.Semester"))
					.Add(Property.ForName("c.CourseCode"))
					.Add(Property.ForName("c.Description"))
				)
				.UniqueResult();

			// Subtest #14
			ProjectionList p1 = Projections.ProjectionList()
				.Add(Property.ForName("StudentNumber").Count())
				.Add(Property.ForName("StudentNumber").Max())
				.Add(Projections.RowCount());

			ProjectionList p2 = Projections.ProjectionList()
				.Add(Property.ForName("StudentNumber").Min())
				.Add(Property.ForName("StudentNumber").Avg())
				.Add(Projections.SqlProjection(
					"1 as constOne, count(*) as countStar",
					new String[] { "constOne", "countStar" },
					new IType[] { NHibernateUtil.Int32, NHibernateUtil.Int32 }
					));

			object[] array = (object[])s.CreateCriteria(typeof(Enrolment))
											.SetProjection(Projections.ProjectionList().Add(p1).Add(p2))
											.UniqueResult();
			
			Assert.AreEqual(7, array.Length);

			// Subtest #15
			IList list = s.CreateCriteria(typeof(Enrolment))
				.CreateAlias("Student", "st")
				.CreateAlias("Course", "co")
				.SetProjection(Projections.ProjectionList()
					.Add(Property.ForName("co.CourseCode").Group())
					.Add(Property.ForName("st.StudentNumber").Count().SetDistinct())
					.Add(Property.ForName("Year").Group())
				)
				.List();

			Assert.AreEqual(2, list.Count);

			// Subtest #16
			list = s.CreateCriteria<Enrolment>()
						.CreateAlias("Student", "st")
						.CreateAlias("Course", "co")
						.SetProjection(Projections.ProjectionList()
							.Add(Property.ForName("co.CourseCode").Group().As("courseCode"))
							.Add(Property.ForName("st.StudentNumber").Count().SetDistinct().As("studentNumber"))
							.Add(Property.ForName("Year").Group())
				)
				.AddOrder(Order.Asc("courseCode"))
				.AddOrder(Order.Asc("studentNumber"))
				.List();
	
			Assert.That(list.Count, Is.EqualTo(2));

			// Subtest #17
			list = s.CreateCriteria<Enrolment>()
				.CreateAlias("Student", "st")
				.CreateAlias("Course", "co")
				.SetProjection(Projections.ProjectionList()
					.Add(Property.ForName("co.CourseCode").Group().As("cCode"))
					.Add(Property.ForName("st.StudentNumber").Count().SetDistinct().As("stNumber"))
					.Add(Property.ForName("Year").Group())
				)
				.AddOrder(Order.Asc("cCode"))
				.AddOrder(Order.Asc("stNumber"))
				.List();
	
			Assert.That(list.Count, Is.EqualTo(2));

			s.Delete(gavin);
			s.Delete(xam);
			s.Delete(course);

			t.Commit();
			s.Close();
		}
		
		[Test]
		public void DistinctProjectionsOfComponents()
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
			gavin.CityState = new CityState("Odessa", "WA");;
			s.Save(gavin);

			Student xam = new Student();
			xam.Name = "Max Rydahl Andersen";
			xam.StudentNumber = 101;
			xam.PreferredCourse = course;
			xam.CityState = new CityState("Odessa", "WA");;
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
			
			object result = s.CreateCriteria<Student>()
				.SetProjection(Projections.Distinct(Property.ForName("CityState")))
				.UniqueResult();
			
			Assert.That(result, Is.InstanceOf<CityState>());
			Assert.That(((CityState)result).City, Is.EqualTo("Odessa"));
			Assert.That(((CityState)result).State, Is.EqualTo("WA"));
	
			result = s.CreateCriteria<Student>()
				.SetProjection(Projections.Distinct(Property.ForName("CityState").As("cityState")))
				.AddOrder(Order.Asc("cityState"))
				.UniqueResult();
			
			Assert.That(result, Is.InstanceOf<CityState>());
			Assert.That(((CityState)result).City, Is.EqualTo("Odessa"));
			Assert.That(((CityState)result).State, Is.EqualTo("WA"));
	
			result = s.CreateCriteria<Student>()
				.SetProjection(Projections.Count("CityState.City"))
				.UniqueResult();
			
			Assert.That(result, Is.EqualTo(2));
	
			result = s.CreateCriteria<Student>()
				.SetProjection(Projections.CountDistinct("CityState.City"))
				.UniqueResult();
			
			Assert.That(result, Is.EqualTo(1));
			
			t.Commit();
			s.Close();

//			s = OpenSession();
//			t = s.BeginTransaction();
//			try
//			{
//				result = s.CreateCriteria<Student>()
//					.SetProjection(Projections.Count("CityState"))
//					.UniqueResult();
//				
//				if (!Dialect.SupportsTupleCounts)
//				{
//					fail( "expected SQLGrammarException" );
//				}
//				
//				Assert.That((long)result, Is.EqualTo(1L));
//			}
//			catch (NHibernate.Exceptions.SQLGrammarException ex)
//			{
//				throw ex;
//				if (!Dialect.SupportsTupleCounts)
//				{
//					// expected
//				}
//				else 
//				{
//					throw ex;
//				}
//			}
//			finally
//			{
//				t.Rollback();
//				s.Close();
//			}

//			s = OpenSession();
//			t = s.BeginTransaction();
//			try 
//			{
//				result = s.CreateCriteria<Student>()
//					.SetProjection(Projections.CountDistinct("CityState"))
//					.UniqueResult();
//				
//				if (!Dialect.SupportsTupleDistinctCounts)
//				{
//					fail("expected SQLGrammarException");
//				}
//				
//				Assert.That((long)result, Is.EqualTo(1L));
//			}
//			catch (NHibernate.Exceptions.SQLGrammarException ex)
//			{
//				throw ex;
//				if (Dialect.SupportsTupleDistinctCounts)
//				{
//					// expected
//				}
//				else 
//				{
//					throw ex;
//				}
//			}
//			finally
//			{
//				t.Rollback();
//				s.Close();
//			}
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete(gavin);
			s.Delete(xam);
			s.Delete(course);
	
			t.Commit();
			s.Close();
		}

		[Test]
		public void GroupByComponent()
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
			gavin.CityState = new CityState("Odessa", "WA");;
			s.Save(gavin);

			Student xam = new Student();
			xam.Name = "Max Rydahl Andersen";
			xam.StudentNumber = 101;
			xam.PreferredCourse = course;
			xam.CityState = new CityState("Odessa", "WA");;
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
	
			object result = s.CreateCriteria<Student>()
				.SetProjection(Projections.GroupProperty("CityState"))
				.UniqueResult();
			
			Assert.That(result, Is.InstanceOf<CityState>());
			Assert.That(((CityState)result).City, Is.EqualTo("Odessa"));
			Assert.That(((CityState)result).State, Is.EqualTo("WA"));
	
			result = s.CreateCriteria<Student>("st")
				.SetProjection(Projections.GroupProperty("st.CityState"))
				.UniqueResult();

			Assert.That(result, Is.InstanceOf<CityState>());
			Assert.That(((CityState)result).City, Is.EqualTo("Odessa"));
			Assert.That(((CityState)result).State, Is.EqualTo("WA"));
	
			result = s.CreateCriteria<Student>("st")
				.SetProjection(Projections.GroupProperty("st.CityState"))
				.AddOrder(Order.Asc("CityState"))
				.UniqueResult();
			
			Assert.That(result, Is.InstanceOf<CityState>());
			Assert.That(((CityState)result).City, Is.EqualTo("Odessa"));
			Assert.That(((CityState)result).State, Is.EqualTo("WA"));
	
			result = s.CreateCriteria<Student>("st")
				.SetProjection(Projections.GroupProperty("st.CityState").As("cityState"))
				.AddOrder(Order.Asc("cityState"))
				.UniqueResult();
			
			Assert.That(result, Is.InstanceOf<CityState>());
			Assert.That(((CityState)result).City, Is.EqualTo("Odessa"));
			Assert.That(((CityState)result).State, Is.EqualTo("WA"));
	
			result = s.CreateCriteria<Student>("st")
				.SetProjection(Projections.GroupProperty("st.CityState").As("cityState"))
				.AddOrder(Order.Asc("cityState"))
				.UniqueResult();
			
			Assert.That(result, Is.InstanceOf<CityState>());
			Assert.That(((CityState)result).City, Is.EqualTo("Odessa"));
			Assert.That(((CityState)result).State, Is.EqualTo("WA"));
	
			result = s.CreateCriteria<Student>("st")
				.SetProjection(Projections.GroupProperty("st.CityState").As("cityState"))
				.Add(Restrictions.Eq("st.CityState", new CityState("Odessa", "WA")))
				.AddOrder(Order.Asc("cityState"))
				.UniqueResult();
			
			Assert.That(result, Is.InstanceOf<CityState>());
			Assert.That(((CityState)result).City, Is.EqualTo("Odessa"));
			Assert.That(((CityState)result).State, Is.EqualTo("WA"));
	
			IList list = s.CreateCriteria<Enrolment>()
				.CreateAlias("Student", "st")
				.CreateAlias("Course", "co")
				.SetProjection(Projections.ProjectionList()
					.Add(Property.ForName("co.CourseCode").Group())
					.Add(Property.ForName("st.CityState").Group())
					.Add(Property.ForName("Year").Group())
				)
				.List();
	
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

			int count = (int)CriteriaTransformer.Clone(s.CreateCriteria(typeof(Enrolment))
															.SetProjection(Property.ForName("StudentNumber").Count().SetDistinct())
								)
								.UniqueResult();
			Assert.AreEqual(2, count);

			object obj = CriteriaTransformer.Clone(s.CreateCriteria(typeof(Enrolment))
													.SetProjection(Projections.ProjectionList()
																	.Add(Property.ForName("StudentNumber").Count())
																	.Add(Property.ForName("StudentNumber").Max())
																	.Add(Property.ForName("StudentNumber").Min())
																	.Add(Property.ForName("StudentNumber").Avg())
													)
				)
				.UniqueResult();
			object[] result = (object[])obj;

			Assert.AreEqual(2, result[0]);
			Assert.AreEqual(667L, result[1]);
			Assert.AreEqual(101L, result[2]);
			Assert.AreEqual(384.0D, (double)result[3], 0.01D);


			CriteriaTransformer.Clone(
				s.CreateCriteria(typeof(Enrolment))
					.Add(Property.ForName("StudentNumber").Gt(665L))
					.Add(Property.ForName("StudentNumber").Lt(668L))
					.Add(Property.ForName("CourseCode").Like("HIB", MatchMode.Start))
					.Add(Property.ForName("Year").Eq((short)1999))
					.AddOrder(Property.ForName("StudentNumber").Asc())
				)
				.UniqueResult();

			ICriteria clonedCriteriaProjection = CriteriaTransformer.Clone(s.CreateCriteria(typeof(Enrolment))
																			.SetProjection(Projections.ProjectionList()
																							.Add(Property.ForName("StudentNumber").As("stNumber"))
																							.Add(Property.ForName("CourseCode").As("cCode"))
																			)
																			.Add(Property.ForName("StudentNumber").Gt(665L))
																			.Add(Property.ForName("StudentNumber").Lt(668L))
																			.AddOrder(Property.ForName("StudentNumber").Asc())
																			.SetResultTransformer(CriteriaSpecification.AliasToEntityMap)
				);
			IList resultWithMaps = clonedCriteriaProjection
				.List();

			Assert.AreEqual(1, resultWithMaps.Count);
			IDictionary m1 = (IDictionary)resultWithMaps[0];

			Assert.AreEqual(667L, m1["stNumber"]);
			Assert.AreEqual(course.CourseCode, m1["cCode"]);

			resultWithMaps = CriteriaTransformer.Clone(s.CreateCriteria(typeof(Enrolment))
														.SetProjection(Property.ForName("StudentNumber").As("stNumber"))
														.AddOrder(Order.Desc("stNumber"))
														.SetResultTransformer(CriteriaSpecification.AliasToEntityMap)
				)
				.List();

			Assert.AreEqual(2, resultWithMaps.Count);
			IDictionary m0 = (IDictionary)resultWithMaps[0];
			m1 = (IDictionary)resultWithMaps[1];

			Assert.AreEqual(101L, m1["stNumber"]);
			Assert.AreEqual(667L, m0["stNumber"]);


			IList resultWithAliasedBean = CriteriaTransformer.Clone(s.CreateCriteria(typeof(Enrolment))
																		.CreateAlias("Student", "st")
																		.CreateAlias("Course", "co")
																		.SetProjection(Projections.ProjectionList()
																						.Add(Property.ForName("st.Name").As("studentName"))
																						.Add(Property.ForName("co.Description").As("courseDescription"))
																		)
																		.AddOrder(Order.Desc("studentName"))
																		.SetResultTransformer(Transformers.AliasToBean(typeof(StudentDTO)))
				)
				.List();

			Assert.AreEqual(2, resultWithAliasedBean.Count);

			StudentDTO dto = (StudentDTO)resultWithAliasedBean[0];
			Assert.IsNotNull(dto.Description);
			Assert.IsNotNull(dto.Name);

			ICriteria complexCriteriaWithProjections = s.CreateCriteria(typeof(Student))
				.Add(Expression.Like("Name", "Gavin", MatchMode.Start))
				.AddOrder(Order.Asc("Name"))
				.CreateCriteria("Enrolments", "e")
				.AddOrder(Order.Desc("Year"))
				.AddOrder(Order.Desc("Semester"))
				.CreateCriteria("Course", "c")
				.AddOrder(Order.Asc("Description"))
				.SetProjection(Projections.ProjectionList()
								.Add(Property.ForName("this.Name"))
								.Add(Property.ForName("e.Year"))
								.Add(Property.ForName("e.Semester"))
								.Add(Property.ForName("c.CourseCode"))
								.Add(Property.ForName("c.Description"))
				);
			CriteriaTransformer.Clone(complexCriteriaWithProjections)
				.UniqueResult();

			ProjectionList p1 = Projections.ProjectionList()
				.Add(Property.ForName("StudentNumber").Count())
				.Add(Property.ForName("StudentNumber").Max())
				.Add(Projections.RowCount());

			ProjectionList p2 = Projections.ProjectionList()
				.Add(Property.ForName("StudentNumber").Min())
				.Add(Property.ForName("StudentNumber").Avg())
				.Add(Projections.SqlProjection(
						"1 as constOne, count(*) as countStar",
						new String[] { "constOne", "countStar" },
						new IType[] { NHibernateUtil.Int32, NHibernateUtil.Int32 }
						));

			object[] array = (object[])CriteriaTransformer.Clone(s.CreateCriteria(typeof(Enrolment))
																	.SetProjection(Projections.ProjectionList().Add(p1).Add(p2))
											)
											.UniqueResult();
			Assert.AreEqual(7, array.Length);

			IList list = CriteriaTransformer.Clone(s.CreateCriteria(typeof(Enrolment))
													.CreateAlias("Student", "st")
													.CreateAlias("Course", "co")
													.SetProjection(Projections.ProjectionList()
																	.Add(Property.ForName("co.CourseCode").Group())
																	.Add(Property.ForName("st.StudentNumber").Count().SetDistinct())
																	.Add(Property.ForName("Year").Group())
													)
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
		public void RestrictionOnSubclassCollection()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			s.CreateCriteria(typeof(Reptile))
				.Add(Expression.IsEmpty("offspring"))
				.List();

			s.CreateCriteria(typeof(Reptile))
				.Add(Expression.IsNotEmpty("offspring"))
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
			ICriteria c = s.CreateCriteria<Animal>("a")
				.CreateAlias("mother", "m")
				.Add(Property.ForName("m.class").Eq(typeof(Reptile)));
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
		public void ProjectedEmbeddedCompositeId()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
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
				gavin.Enrolments.Add(enrolment);
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
				
				IList enrolments = (IList)s.CreateCriteria<Enrolment>()
					.SetProjection(Projections.Id())
					.List();
				
				t.Rollback();
			}
		}
		
		[Test]
		public void ProjectedListIncludesEmbeddedCompositeId()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
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
				gavin.Enrolments.Add(enrolment);
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
				
				IList data = (IList)s.CreateCriteria<Enrolment>()
					.SetProjection(Projections.ProjectionList()
						.Add(Projections.Property("Semester"))
						.Add(Projections.Property("Year"))
						.Add(Projections.Id()))
				.List();
				
				t.Rollback();
			}
		}
		
		[Test]
		public void ProjectedCompositeId()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				Course course = new Course();
				course.CourseCode = "HIB";
				course.Description = "Hibernate Training";
				course.CourseMeetings.Add(new CourseMeeting(course, "Monday", 1, "1313 Mockingbird Lane"));
				s.Save(course);
				s.Flush();
		
				IList data = (IList)s.CreateCriteria<CourseMeeting>().SetProjection(Projections.Id()).List();

				t.Rollback();
			}
		}

		[Test]
		public void ProjectedCompositeIdWithAlias()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				Course course = new Course();
				course.CourseCode = "HIB";
				course.Description = "Hibernate Training";
				course.CourseMeetings.Add(new CourseMeeting(course, "Monday", 1, "1313 Mockingbird Lane"));
				s.Save(course);
				s.Flush();
		
				IList data = (IList)s.CreateCriteria<CourseMeeting>()
					.SetProjection(Projections.Id().As("id"))
					.List();

				t.Rollback();
			}
		}
	
		[Test]
		public void ProjectedComponent()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				Student gaith = new Student();
				gaith.Name = "Gaith Bell";
				gaith.StudentNumber = 123;
				gaith.CityState = new CityState("Chicago", "Illinois");
				s.Save(gaith);
				s.Flush();
		
				IList cityStates = (IList)s.CreateCriteria<Student>()
					.SetProjection(Projections.Property("CityState"))
					.List();
				
				t.Rollback();
			}
		}

		[Test]
		public void ProjectedListIncludesComponent()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				Student gaith = new Student();
				gaith.Name = "Gaith Bell";
				gaith.StudentNumber = 123;
				gaith.CityState = new CityState("Chicago", "Illinois");
				s.Save(gaith);
				s.Flush();
	
				IList data = (IList)s.CreateCriteria<Student>()
					.SetProjection(Projections.ProjectionList()
						.Add(Projections.Property("CityState"))
						.Add(Projections.Property("Name")))
						.List();
				
				t.Rollback();
			}
		}
		
		[Test]
		public void CloningProjectedId()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.CreateCriteria<Course>().SetProjection(Projections.Property("CourseCode")).List();
			CriteriaTransformer.Clone(s.CreateCriteria(typeof(Course)).SetProjection(Projections.Id())).List();
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
				.SetProjection(Property.ForName("PreferredCourse.CourseCode"))
				.CreateCriteria("PreferredCourse", JoinType.LeftOuterJoin)
				.AddOrder(Order.Asc("CourseCode"));

			IList result = CriteriaTransformer.Clone(criteria).List();

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
				.SetProjection(Property.ForName("PreferredCourse.CourseCode"))
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

		[Test]
		public void TypeMismatch()
		{
			using (ISession session = OpenSession())
			{
				Assert.Throws<QueryException>(() => session.CreateCriteria(typeof(Enrolment))
					.Add(Expression.Eq("Student", 10)) // Type mismatch!
					.List());
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
					.Add(Property.ForName("ma.class").Eq(typeof(MaterialUnitable)))
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

		[Test]
		public void CriteriaInspection()
		{
			using (ISession session = OpenSession())
			{
				ICriteria criteria = session.CreateCriteria(typeof(MaterialUnit), "mu")
					.CreateAlias("mu.Material", "ma");
				Assert.IsNotNull(criteria.GetCriteriaByAlias("ma"));
				Assert.AreEqual("ma", criteria.GetCriteriaByPath("mu.Material").Alias);
				Assert.AreEqual(criteria, criteria.GetCriteriaByAlias("mu"));
				Assert.AreEqual(criteria.CreateCriteria("fooBar"), criteria.GetCriteriaByPath("fooBar"));
			}
		}

		[Test]
		public void DetachedCriteriaInspection()
		{
			DetachedCriteria criteria = DetachedCriteria.For(typeof(Student))
				.CreateAlias("mu.Material", "ma");
			Assert.IsNotNull(criteria.GetCriteriaByAlias("ma"));
			Assert.AreEqual("ma", criteria.GetCriteriaByPath("mu.Material").Alias);

			Assert.IsNull(criteria.GetCriteriaByPath("fooBar"));
			Assert.IsNull(criteria.GetCriteriaByAlias("fooBar"));
		}

		[Test]
		public void SameColumnAndAliasNames()
		{
			DetachedCriteria dc = DetachedCriteria.For(typeof(Student))
				.Add(Property.ForName("Name").Eq("Gavin King"))
				.AddOrder(Order.Asc("StudentNumber"))
				.SetProjection(
					Projections.ProjectionList()
						.Add(Projections.Property("StudentNumber"), "StudentNumber")
						.Add(Projections.Property("Name"), "Name"));


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

			session.Delete(gavin);
			session.Delete(bizarroGavin);
			t.Commit();
			session.Close();
		}

		[Test]
		public void SameColumnAndAliasNamesResultTransformer()
		{
			DetachedCriteria dc = DetachedCriteria.For(typeof(Student))
				.SetProjection(
					Projections.ProjectionList()
						.Add(Projections.Property("StudentNumber"), "StudentNumber")
						.Add(Projections.Property("Name"), "Name"))
				.SetResultTransformer(new AliasToBeanResultTransformer(typeof(Student)))
				.Add(Property.ForName("Name").Eq("Gavin King"))
				.AddOrder(Order.Asc("StudentNumber"));


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

			Assert.That(result.Count, Is.EqualTo(2));
			Assert.That(result[0], Is.InstanceOf(typeof(Student)));
			Assert.That(result[1], Is.InstanceOf(typeof(Student)));

			session.Delete(gavin);
			session.Delete(bizarroGavin);
			t.Commit();
			session.Close();
		}
		
		[Test]
		public void CacheDetachedCriteria()
		{
			using (ISession session = OpenSession())
			{
				bool current = sessions.Statistics.IsStatisticsEnabled;
				sessions.Statistics.IsStatisticsEnabled = true;
				sessions.Statistics.Clear();
				DetachedCriteria dc = DetachedCriteria.For(typeof (Student))
				.Add(Property.ForName("Name").Eq("Gavin King"))
				.SetProjection(Property.ForName("StudentNumber"))
				.SetCacheable(true);
				Assert.That(sessions.Statistics.QueryCacheMissCount,Is.EqualTo(0));
				Assert.That(sessions.Statistics.QueryCacheHitCount, Is.EqualTo(0));
				dc.GetExecutableCriteria(session).List();
				Assert.That(sessions.Statistics.QueryCacheMissCount, Is.EqualTo(1));

				dc = DetachedCriteria.For(typeof(Student))
				.Add(Property.ForName("Name").Eq("Gavin King"))
				.SetProjection(Property.ForName("StudentNumber"))
				.SetCacheable(true);
				dc.GetExecutableCriteria(session).List();

				Assert.That(sessions.Statistics.QueryCacheMissCount, Is.EqualTo(1));
				Assert.That(sessions.Statistics.QueryCacheHitCount, Is.EqualTo(1));
				sessions.Statistics.IsStatisticsEnabled = false;
			}
		}
		
		[Test]
		public void PropertyWithFormulaAndPagingTest()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			ICriteria crit = s.CreateCriteria(typeof(Animal))
				.SetFirstResult(0)
				.SetMaxResults(1)
				.AddOrder(new Order("bodyWeight", true));

			crit.List<Animal>();

			t.Rollback();
			s.Close();
		}

		[Test]
		public void SqlExpressionWithParameters()
		{
			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				ICriteria c = session.CreateCriteria(typeof(Student));
				c.Add(Expression.Eq("StudentNumber", (long)232));
				c.Add(Expression.Sql("2 = ?", 1, NHibernateUtil.Int32));

				Student gavin = new Student();
				gavin.Name = "Gavin King";
				gavin.StudentNumber = 232;
				session.Save(gavin);

				IList result = c.List();

				Assert.AreEqual(0, result.Count);

				session.Delete(gavin);

				t.Commit();
			}
		}

		[Test]
		public void ParametersInCountExpression()
		{
			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				ICriteria criteria = session.CreateCriteria(typeof(Student), "c");

				DetachedCriteria subselect = DetachedCriteria.For(typeof(Enrolment));
				subselect.Add(Expression.Eq("Year", (short)2008));
				subselect.SetProjection(Projections.Distinct(Projections.Property("StudentNumber")));

				criteria.Add(Subqueries.PropertyNotIn("StudentNumber", subselect));

				ICriteria rowCount = CriteriaTransformer.TransformToRowCount(criteria);

				// IMPORTANT: The problem is executing BOTH queries at the same time... not just one
				criteria.List();
				rowCount.List();

				t.Commit();
			}
		}
	
		[Test]
		public void TransformToRowCountTest()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				ICriteria crit = s.CreateCriteria(typeof (Student));
				ICriteria subCriterium = crit.CreateCriteria("PreferredCourse");
				subCriterium.Add(Property.ForName("CourseCode").Eq("PREFFERED_CODE"));


				ICriteria countCriteria = CriteriaTransformer.TransformToRowCount(crit);

				countCriteria.List();

				t.Rollback();
			}
		}

		[Test]
		public void OrderProjectionTest()
		{
			using (ISession session = this.OpenSession())
			{
				ICriteria criteria = session.CreateCriteria(typeof(Student), "c");

				criteria.AddOrder(
					Order.Asc(
						Projections.Conditional(
							Restrictions.Eq("StudentNumber", (long)1),
							Projections.Constant(0),
							Projections.Constant(1))));

				criteria.List();
			}
		}

		[Test]
		public void OrderProjectionAliasedTest()
		{
			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{

				Course courseA = new Course();
				courseA.CourseCode = "HIB-A";
				courseA.Description = "Hibernate Training A";
				session.Save(courseA);
			
				Student gavin = new Student();
				gavin.Name = "Gavin King";
				gavin.StudentNumber = 232;
				gavin.PreferredCourse = courseA;
				session.Save(gavin);
			
				Student leonardo = new Student();
				leonardo.Name = "Leonardo Quijano";
				leonardo.StudentNumber = 233;
				leonardo.PreferredCourse = courseA;
				session.Save(leonardo);
			
				Student johnDoe = new Student();
				johnDoe.Name = "John Doe";
				johnDoe.StudentNumber = 235;
				johnDoe.PreferredCourse = null;
				session.Save(johnDoe);
			
				IProjection conditional =
					Projections.Conditional(
						Restrictions.Eq("Name", "Gavin King"),
						Projections.Constant("Name"),
						Projections.Constant("AnotherName"));

				ICriteria criteria = session.CreateCriteria(typeof(Student));
				criteria.SetMaxResults(1);
				criteria.SetFirstResult(1);
				IList result = criteria.SetProjection(Projections.Alias(conditional, "CheckName"))
					.AddOrder(Order.Asc("CheckName"))
					.List();
	
				session.Delete(gavin);
				session.Delete(leonardo);
				session.Delete(johnDoe);
				session.Delete(courseA);

				t.Commit();
			}
		}

		[Test]
		public void LikeProjectionTest()
		{
			Student john = new Student { Name = "John" };
			using (ISession session = this.OpenSession())
			{
				session.Save(john);
				session.Flush();
			}

			using (ISession session = this.OpenSession())
			{
				ICriteria criteria = session.CreateCriteria(typeof(Student), "c");

				criteria.Add(new LikeExpression(Projections.Property("Name"), "John", MatchMode.Anywhere));

				Assert.AreEqual(1, criteria.List().Count);
			}

			using (ISession session = this.OpenSession())
			{
				ICriteria criteria = session.CreateCriteria(typeof(Student), "c");

				criteria.Add(new LikeExpression("Name", "John"));

				Assert.AreEqual(1, criteria.List().Count);
			}

			using (ISession session = this.OpenSession())
			{
				session.Delete(john);
				session.Flush();
			}
		}

		[Test]
		public void LikeProjectionUsingRestrictionsTest()
		{
			using (ISession session = this.OpenSession())
			{
				ICriteria criteria = session.CreateCriteria(typeof(Student), "c");

				criteria.Add(Restrictions.Like(Projections.Constant("Name"), "John", MatchMode.Anywhere));

				criteria.List();
			}
		}

		[Test]
		public void InsensitiveLikeProjectionUsingRestrictionsTest()
		{
			using (ISession session = this.OpenSession())
			{
				ICriteria criteria = session.CreateCriteria(typeof(Student), "c");

				criteria.Add(Restrictions.InsensitiveLike(Projections.Constant("Name"), "John", MatchMode.Anywhere));

				criteria.List();
			}
		}

		[Test]
		public void AliasJoinCriterion()
		{
			using (ISession session = this.OpenSession())
			{
				using (ITransaction t = session.BeginTransaction())
				{
					Course courseA = new Course();
					courseA.CourseCode = "HIB-A";
					courseA.Description = "Hibernate Training A";
					session.Persist(courseA);
					
					Course courseB = new Course();
					courseB.CourseCode = "HIB-B";
					courseB.Description = "Hibernate Training B";
					session.Persist(courseB);

					Student gavin = new Student();
					gavin.Name = "Gavin King";
					gavin.StudentNumber = 232;
					gavin.PreferredCourse = courseA;
					session.Persist(gavin);

					Student leonardo = new Student();
					leonardo.Name = "Leonardo Quijano";
					leonardo.StudentNumber = 233;
					leonardo.PreferredCourse = courseB;
					session.Persist(leonardo);

					Student johnDoe = new Student();
					johnDoe.Name = "John Doe";
					johnDoe.StudentNumber = 235;
					johnDoe.PreferredCourse = null;
					session.Persist(johnDoe);

					// test == on one value exists
					IList<string> result = session.CreateCriteria<Student>()
						.CreateAlias("PreferredCourse", "pc", JoinType.LeftOuterJoin,
							Restrictions.Eq("pc.CourseCode", "HIB-A"))
						.SetProjection(Property.ForName("pc.CourseCode"))
						.AddOrder(Order.Asc("pc.CourseCode"))
						.List<string>();
					
					// can't be sure of NULL comparison ordering aside from they should
					// either come first or last
					if (result[0] == null)
					{
						Assert.IsNull(result[1]);
						Assert.AreEqual("HIB-A", result[2]);
					}
					else
					{
						Assert.IsNull(result[2]);
						Assert.IsNull(result[1]);
						Assert.AreEqual("HIB-A", result[0]);
					}

					// test == on non existent value
					result = session.CreateCriteria<Student>()
						.CreateAlias("PreferredCourse", "pc", JoinType.LeftOuterJoin,
							Restrictions.Eq("pc.CourseCode", "HIB-R"))
						.SetProjection(Property.ForName("pc.CourseCode"))
						.AddOrder(Order.Asc("pc.CourseCode"))
						.List<string>();

					Assert.AreEqual(3, result.Count);
					Assert.IsNull(result[2]);
					Assert.IsNull(result[1]);
					Assert.IsNull(result[0]);

					// test != on one existing value
					result = session.CreateCriteria<Student>()
						.CreateAlias("PreferredCourse", "pc", JoinType.LeftOuterJoin,
							Restrictions.Not(Restrictions.Eq("pc.CourseCode", "HIB-A")))
						.SetProjection(Property.ForName("pc.CourseCode"))
						.AddOrder(Order.Asc("pc.CourseCode"))
						.List<string>();

					Assert.AreEqual(3, result.Count);

					// can't be sure of NULL comparison ordering aside from they should
					// either come first or last
					if (result[0] == null)
					{
						Assert.IsNull(result[1]);
						Assert.AreEqual("HIB-B", result[2]);
					}
					else
					{
						Assert.AreEqual("HIB-B", result[0]);
						Assert.IsNull(result[1]);
						Assert.IsNull(result[2]);
					}

					// test != on one existing value (using clone)
					var criteria = session.CreateCriteria<Student>()
						.CreateAlias("PreferredCourse", "pc", JoinType.LeftOuterJoin,
						             Restrictions.Not(Restrictions.Eq("pc.CourseCode", "HIB-A")))
						.SetProjection(Property.ForName("pc.CourseCode"))
						.AddOrder(Order.Asc("pc.CourseCode"));
					var clonedCriteria = CriteriaTransformer.Clone(criteria);
					result = clonedCriteria.List<string>();

					Assert.AreEqual(3, result.Count);

					// can't be sure of NULL comparison ordering aside from they should
					// either come first or last
					if (result[0] == null)
					{
						Assert.IsNull(result[1]);
						Assert.AreEqual("HIB-B", result[2]);
					}
					else
					{
						Assert.AreEqual("HIB-B", result[0]);
						Assert.IsNull(result[1]);
						Assert.IsNull(result[2]);
					}
					
					session.Delete(gavin);
					session.Delete(leonardo);
					session.Delete(johnDoe);
					session.Delete(courseA);
					session.Delete(courseB);

					t.Commit();
				}
			}
		}

		[Test]
		public void IgnoreCase()
		{
			//SqlServer collation set to Latin1_General_BIN
			//when database created to validate this test
			Course c1 = new Course();
			c1.CourseCode = "course-1";
			c1.Description = "Advanced NHibernate";
			Course c2 = new Course();
			c2.CourseCode = "course-2";
			c2.Description = "advanced csharp";
			Course c3 = new Course();
			c3.CourseCode = "course-3";
			c3.Description = "advanced UnitTesting";

			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				session.Save(c1);
				session.Save(c2);
				session.Save(c3);
				t.Commit();
			}

			// this particular selection is commented out if collation is not Latin1_General_BIN
			//using (ISession session = OpenSession())
			//{
			//    // order the courses in binary order - assumes collation Latin1_General_BIN
			//    IList result =
			//        session.CreateCriteria(typeof(Course)).AddOrder(Order.Asc("Description")).List();
			//    Assert.AreEqual(3, result.Count);
			//    Course firstResult = (Course)result[0];
			//    Assert.IsTrue(firstResult.Description.Contains("Advanced NHibernate"), "Description should have 'Advanced NHibernate', but has " + firstResult.Description);
			//}

			using (ISession session = OpenSession())
			{
				// order the courses after all descriptions have been converted to lower case
				IList result =
					session.CreateCriteria(typeof (Course)).AddOrder(Order.Asc("Description").IgnoreCase()).List();
				Assert.AreEqual(3, result.Count);
				Course firstResult = (Course) result[0];
				Assert.IsTrue(firstResult.Description.Contains("advanced csharp"), "Description should have 'advanced csharp', but has " + firstResult.Description);
			}

			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				session.Delete(c1);
				session.Delete(c2);
				session.Delete(c3);
				t.Commit();
			}
		}

		[Test]
		public void CanSetLockModeOnDetachedCriteria()
		{
			//NH-3710
			var dc = DetachedCriteria
				.For(typeof(Student))
				.SetLockMode(LockMode.Upgrade);

			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Save(new Student { Name = "Ricardo Peres", StudentNumber = 666, CityState = new CityState("Coimbra", "Portugal") });
				session.Flush();

				var ec = dc.GetExecutableCriteria(session);
				var countExec = CriteriaTransformer.TransformToRowCount(ec);
				var countRes = countExec.UniqueResult();

				Assert.AreEqual(countRes, 1);
			}
		}
	}
}