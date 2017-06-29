using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Transaction;
using NUnit.Framework;

namespace NHibernate.Test.Component.Basic
{
	[TestFixture]
	public class ComponentTest : TestCase 
	{			
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}		

		protected override System.Collections.IList Mappings
		{
			get { return new string[] { }; }
		}
		
		protected override void Configure(Configuration configuration)
		{
			if (Dialect.Functions.ContainsKey("year"))
			{
				using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NHibernate.Test.Component.Basic.User.hbm.xml"))
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						string mapping = reader.ReadToEnd();

						IList args = new ArrayList();
						args.Add("dob");
						// We don't have a session factory yet... is there some way to get one sooner?
						string replacement = Dialect.Functions["year"].Render(args, null).ToString().Replace("\"", "&quot;");
						mapping = mapping.Replace("year(dob)", replacement);

						configuration.AddXml(mapping);
						configuration.SetProperty(Cfg.Environment.GenerateStatistics, "true");
					}
				}
			}
		}
	
		protected override void OnTearDown()
		{
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete("from User");
				s.Delete("from Employee");
				t.Commit();
			}
			
			base.OnTearDown();
		}

		[Test]
		public void TestUpdateFalse() 
		{
			User u;
			
			Sfi.Statistics.Clear();
				
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				u = new User("gavin", "secret", new Person("Gavin King", new DateTime(1999, 12, 31), "Karbarook Ave"));
				s.Persist(u);
				s.Flush();
				u.Person.Name = "XXXXYYYYY";
				t.Commit();
				s.Close();
			}
			
			Assert.That(Sfi.Statistics.EntityInsertCount, Is.EqualTo(1));
			Assert.That(Sfi.Statistics.EntityUpdateCount, Is.EqualTo(0));

			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				u = (User)s.Get(typeof(User), "gavin");
				Assert.That(u.Person.Name, Is.EqualTo("Gavin King"));
				s.Delete(u);
				t.Commit();
				s.Close();
			}
			
			Assert.That(Sfi.Statistics.EntityDeleteCount, Is.EqualTo(1));
		}
		
		[Test]
		public void TestComponent() 
		{
			User u;
			
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				u = new User("gavin", "secret", new Person("Gavin King", new DateTime(1999, 12, 31), "Karbarook Ave"));
				s.Persist(u);
				s.Flush();
				u.Person.ChangeAddress("Phipps Place");
				t.Commit();
			}
			
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{			
				u = (User)s.Get(typeof(User), "gavin");
				Assert.That(u.Person.Address, Is.EqualTo("Phipps Place"));
				Assert.That(u.Person.PreviousAddress, Is.EqualTo("Karbarook Ave"));
				Assert.That(u.Person.Yob, Is.EqualTo(u.Person.Dob.Year));
				u.Password = "$ecret";
				t.Commit();
			}
			
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				u = (User)s.Get(typeof(User), "gavin");
				Assert.That(u.Person.Address, Is.EqualTo("Phipps Place"));
				Assert.That(u.Person.PreviousAddress, Is.EqualTo("Karbarook Ave"));
				Assert.That(u.Password, Is.EqualTo("$ecret"));
				s.Delete(u);
				t.Commit();
			}
		}
		
		[Test]
		public void TestComponentStateChangeAndDirtiness() 
		{
			// test for HHH-2366
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				User u = new User("steve", "hibernater", new Person( "Steve Ebersole", new DateTime(1999, 12, 31), "Main St"));
				s.Persist(u);
				s.Flush();
				long intialUpdateCount = Sfi.Statistics.EntityUpdateCount;
				u.Person.Address = "Austin";
				s.Flush();
				Assert.That(Sfi.Statistics.EntityUpdateCount, Is.EqualTo(intialUpdateCount + 1));
				intialUpdateCount = Sfi.Statistics.EntityUpdateCount;
				u.Person.Address = "Cedar Park";
				s.Flush();
				Assert.That(Sfi.Statistics.EntityUpdateCount, Is.EqualTo(intialUpdateCount + 1));
				s.Delete(u);
				t.Commit();
				s.Close();
			}
		}
		
		[Test]
		[Ignore("Ported from Hibernate. Read properties not supported in NH yet.")]
		public void TestCustomColumnReadAndWrite() 
		{
			const double HEIGHT_INCHES = 73;
			const double HEIGHT_CENTIMETERS = HEIGHT_INCHES * 2.54d;
			
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				User u = new User("steve", "hibernater", new Person( "Steve Ebersole", new DateTime(1999, 12, 31), "Main St"));
				u.Person.HeightInches = HEIGHT_INCHES;
				s.Persist(u);
				s.Flush();
			
				// Test value conversion during insert
				double heightViaSql = (double)s.CreateSQLQuery("select height_centimeters from t_user where t_user.username='steve'").UniqueResult();
				Assert.That(heightViaSql, Is.EqualTo(HEIGHT_CENTIMETERS).Within(0.01d));
		
				// Test projection
				double heightViaHql = (double)s.CreateQuery("select u.Person.HeightInches from User u where u.Id = 'steve'").UniqueResult();
				Assert.That(heightViaHql, Is.EqualTo(HEIGHT_INCHES).Within(0.01d));
				
				// Test restriction and entity load via criteria
				u = (User)s.CreateCriteria(typeof(User))
					.Add(Restrictions.Between("Person.HeightInches", HEIGHT_INCHES - 0.01d, HEIGHT_INCHES + 0.01d))
					.UniqueResult();
				Assert.That(u.Person.HeightInches, Is.EqualTo(HEIGHT_INCHES).Within(0.01d));

					// Test predicate and entity load via HQL
				u = (User)s.CreateQuery("from User u where u.Person.HeightInches between ? and ?")
					.SetDouble(0, HEIGHT_INCHES - 0.01d)
					.SetDouble(1, HEIGHT_INCHES + 0.01d)
					.UniqueResult();
				
				Assert.That(u.Person.HeightInches, Is.EqualTo(HEIGHT_INCHES).Within(0.01d));
				
				// Test update
				u.Person.HeightInches = 1;
				s.Flush();
				heightViaSql = (double)s.CreateSQLQuery("select height_centimeters from t_user where t_user.username='steve'").UniqueResult();
				Assert.That(heightViaSql, Is.EqualTo(2.54d).Within(0.01d));
				s.Delete(u);
				t.Commit();
				s.Close();
			}
		}
	
		[Test]
		[Ignore("Ported from Hibernate - failing in NH")]
		public void TestComponentQueries() 
		{
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				Employee emp = new Employee();
				emp.HireDate = new DateTime(1999, 12, 31);
				emp.Person = new Person();
				emp.Person.Name = "steve";
				emp.Person.Dob = new DateTime(1999, 12, 31);
				s.Save(emp);
		
				s.CreateQuery("from Employee e where e.Person = :p and 1=1 and 2=2").SetParameter("p", emp.Person).List();
				s.CreateQuery("from Employee e where :p = e.Person").SetParameter("p", emp.Person).List();
				s.CreateQuery("from Employee e where e.Person = ('steve', current_timestamp)").List();
		
				s.Delete( emp );
				t.Commit();
				s.Close();
			}
		}
		
		[Test]
		public void TestComponentFormulaQuery() 
		{
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{			
				s.CreateQuery("from User u where u.Person.Yob = 1999").List();
				s.CreateCriteria(typeof(User))
					.Add(Property.ForName("Person.Yob").Between(1999, 2002))
					.List();
				
				if (Dialect.SupportsRowValueConstructorSyntax) 
				{
					s.CreateQuery("from User u where u.Person = ('gavin', :dob, 'Peachtree Rd', 'Karbarook Ave', 1974, 'Peachtree Rd')")
						.SetDateTime("dob", new DateTime(1974, 3, 25)).List();
					s.CreateQuery("from User where Person = ('gavin', :dob, 'Peachtree Rd', 'Karbarook Ave', 1974, 'Peachtree Rd')")
						.SetDateTime("dob", new DateTime(1974, 3, 25)).List();
				}
				t.Commit();
				s.Close();
			}
		}
	
		[Test]
		public void TestNamedQuery() 
		{
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.GetNamedQuery("userNameIn")
					.SetParameterList( "nameList", new object[] {"1ovthafew", "turin", "xam"} )
					.List();
				t.Commit();
				s.Close();
			}
		}
	
		[Test]
		public void TestMergeComponent() 
		{
			Employee emp = null;
			IEnumerator<Employee> enumerator = null;
				
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				emp = new Employee();
				emp.HireDate = new DateTime(1999, 12, 31);
				emp.Person = new Person();
				emp.Person.Name = "steve";
				emp.Person.Dob = new DateTime(1999, 12, 31);
				s.Persist(emp);
				t.Commit();
				s.Close();
			}
			
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				emp = (Employee)s.Get(typeof(Employee), emp.Id);
				t.Commit();
				s.Close();
			}
			
			Assert.That(emp.OptionalComponent, Is.Null);
			
			emp.OptionalComponent = new OptionalComponent();
			emp.OptionalComponent.Value1 = "emp-value1";
			emp.OptionalComponent.Value2 = "emp-value2";
	
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				emp = (Employee)s.Merge(emp);
				t.Commit();
				s.Close();
			}
			
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				emp = (Employee)s.Get(typeof(Employee), emp.Id);
				t.Commit();
				s.Close();
			}
			
			Assert.That(emp.OptionalComponent.Value1, Is.EqualTo("emp-value1"));
			Assert.That(emp.OptionalComponent.Value2, Is.EqualTo("emp-value2"));

			emp.OptionalComponent.Value1 = null;
			emp.OptionalComponent.Value2 = null;
	
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				emp = (Employee)s.Merge(emp);
				t.Commit();
				s.Close();
			}
			
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				emp = (Employee)s.Get(typeof(Employee), emp.Id);
				NHibernateUtil.Initialize(emp.DirectReports);
				t.Commit();
				s.Close();
			}
			
			Assert.That(emp.OptionalComponent, Is.Null);
	
			Employee emp1 = new Employee();
			emp1.HireDate = new DateTime(1999, 12, 31);
			emp1.Person = new Person();
			emp1.Person.Name = "bozo";
			emp1.Person.Dob = new DateTime(1999, 12, 31);
			emp.DirectReports.Add(emp1);
	
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				emp = (Employee)s.Merge(emp);
				t.Commit();
				s.Close();
			}
			
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				emp = (Employee)s.Get(typeof(Employee), emp.Id);
				NHibernateUtil.Initialize(emp.DirectReports);
				t.Commit();
				s.Close();
			}
			
			Assert.That(emp.DirectReports.Count, Is.EqualTo(1));
			
			enumerator = emp.DirectReports.GetEnumerator();
			enumerator.MoveNext();
			emp1 = (Employee)enumerator.Current;
			Assert.That(emp1.OptionalComponent, Is.Null);
			
			emp1.OptionalComponent = new OptionalComponent();
			emp1.OptionalComponent.Value1 = "emp1-value1";
			emp1.OptionalComponent.Value2 = "emp1-value2";
	
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				emp = (Employee)s.Merge(emp);
				t.Commit();
				s.Close();
			}
			
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				emp = (Employee)s.Get(typeof(Employee), emp.Id);
				NHibernateUtil.Initialize(emp.DirectReports);
				t.Commit();
				s.Close();
			}
			
			Assert.That(emp.DirectReports.Count, Is.EqualTo(1));
			
			enumerator = emp.DirectReports.GetEnumerator();
			enumerator.MoveNext();
			emp1 = (Employee)enumerator.Current;
			Assert.That(emp1.OptionalComponent.Value1, Is.EqualTo("emp1-value1"));
			Assert.That(emp1.OptionalComponent.Value2, Is.EqualTo("emp1-value2"));
			
			emp1.OptionalComponent.Value1 = null;
			emp1.OptionalComponent.Value2 = null;
	
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				emp = (Employee)s.Merge(emp);
				t.Commit();
				s.Close();
			}
			
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				emp = (Employee)s.Get(typeof(Employee), emp.Id);
				NHibernateUtil.Initialize(emp.DirectReports);
				t.Commit();
				s.Close();
			}
	
			Assert.That(emp.DirectReports.Count, Is.EqualTo(1));
			
			enumerator = emp.DirectReports.GetEnumerator();
			enumerator.MoveNext();
			emp1 = (Employee)enumerator.Current;
			Assert.That(emp1.OptionalComponent, Is.Null);
	
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete( emp );
				t.Commit();
				s.Close();
			}
		}
	}
}