using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2554
{
	[TestFixture]
	public class Fixture: BugTestCase
	{
		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return (dialect is NHibernate.Dialect.MsSql2005Dialect) || (dialect is NHibernate.Dialect.MsSql2008Dialect);
		}
		
		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			configuration.SetProperty(NHibernate.Cfg.Environment.Hbm2ddlKeyWords, "keywords");
			base.Configure(configuration);
		}
		
		protected override void OnSetUp()
		{
			base.OnSetUp();
			
			using (ISession session = Sfi.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Persist(new Student() { FullName = "Julian Maughan" });
				transaction.Commit();
			}
		}
		
		protected override void OnTearDown()
		{
			using (ISession session = Sfi.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Student").ExecuteUpdate();
				transaction.Commit();
			}
			
			base.OnTearDown();
		}
		
		[Test]
		public void TestMappedFormulasContainingSqlServerDataTypeKeywords()
		{
			using (ISession session = Sfi.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var students = session.CreateQuery("from Student").List<Student>();
				Assert.That(students.Count, Is.EqualTo(1));
				Assert.That(students[0].FullName, Is.StringMatching("Julian Maughan"));
				Assert.That(students[0].FullNameAsVarBinary.Length, Is.EqualTo(28));
				Assert.That(students[0].FullNameAsVarBinary512.Length, Is.EqualTo(28));
				// Assert.That(students[0].FullNameAsBinary.Length, Is.EqualTo(28)); 30???
				Assert.That(students[0].FullNameAsBinary256.Length, Is.EqualTo(256));
				Assert.That(students[0].FullNameAsVarChar.Length, Is.EqualTo(14));
				Assert.That(students[0].FullNameAsVarChar125.Length, Is.EqualTo(14));
				
				transaction.Commit();
			}
		}
		
		[Test]
		public void TestHqlStatementsContainingSqlServerDataTypeKeywords()
		{
			using (ISession session = Sfi.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var students = session
					.CreateQuery("from Student where length(convert(varbinary, FullName)) = 28")
					.List<Student>();
				
				Assert.That(students.Count, Is.EqualTo(1));
				
				students = session
					.CreateQuery("from Student where length(convert(varbinary(256), FullName)) = 28")
					.List<Student>();
				
				Assert.That(students.Count, Is.EqualTo(1));
				
				students = session
					.CreateQuery("from Student where convert(int, 1) = 1")
					.List<Student>();
				
				Assert.That(students.Count, Is.EqualTo(1));
				
				transaction.Commit();
			}
		}
	}
}
