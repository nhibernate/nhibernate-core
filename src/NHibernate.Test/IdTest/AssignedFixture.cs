using System;
using System.Collections.Generic;
using log4net;
using log4net.Core;
using NUnit.Framework;

namespace NHibernate.Test.IdTest
{

	[TestFixture]
	public class AssignedFixture : IdFixtureBase
	{

		private string[] GetAssignedIdentifierWarnings(LogSpy ls)
		{
			List<string> warnings = new List<string>();

			foreach (string logEntry in ls.GetWholeLog().Split('\n'))
				if (logEntry.Contains("Unable to determine if") && logEntry.Contains("is transient or detached"))
					warnings.Add(logEntry);

			return warnings.ToArray();
		}

		protected override string TypeName
		{
			get { return "Assigned"; }
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateQuery("delete from Child").ExecuteUpdate();
				s.CreateQuery("delete from Parent").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void SaveOrUpdate_Save()
		{
			using (LogSpy ls = new LogSpy(LogManager.GetLogger(typeof(AssignedFixture).Assembly, "NHibernate"), Level.Warn))
			using (ISession s = OpenSession())
			{
				ITransaction t = s.BeginTransaction();

				Parent parent =
					new Parent()
					{
						Id = "parent",
						Children = new List<Child>(),
					};

				s.SaveOrUpdate(parent);
				t.Commit();

				long actual = s.CreateQuery("select count(p) from Parent p").UniqueResult<long>();
				Assert.That(actual, Is.EqualTo(1));

				string[] warnings = GetAssignedIdentifierWarnings(ls);
				Assert.That(warnings.Length, Is.EqualTo(1));
				Assert.IsTrue(warnings[0].Contains("parent"));
			}
		}

		[Test]
		public void SaveNoWarning()
		{
			using (LogSpy ls = new LogSpy(LogManager.GetLogger(typeof(AssignedFixture).Assembly, "NHibernate"), Level.Warn))
			using (ISession s = OpenSession())
			{
				ITransaction t = s.BeginTransaction();

				Parent parent =
					new Parent()
					{
						Id = "parent",
						Children = new List<Child>(),
					};

				s.Save(parent);
				t.Commit();

				long actual = s.CreateQuery("select count(p) from Parent p").UniqueResult<long>();
				Assert.That(actual, Is.EqualTo(1));

				string[] warnings = GetAssignedIdentifierWarnings(ls);
				Assert.That(warnings.Length, Is.EqualTo(0));
			}
		}

		[Test]
		public void SaveOrUpdate_Update()
		{
			using (ISession s = OpenSession())
			{
				ITransaction t = s.BeginTransaction();

				s.Save(new Parent() { Id = "parent", Name = "before" });
				t.Commit();
			}

			using (LogSpy ls = new LogSpy(LogManager.GetLogger(typeof(AssignedFixture).Assembly, "NHibernate"), Level.Warn))
			using (ISession s = OpenSession())
			{
				ITransaction t = s.BeginTransaction();

				Parent parent =
					new Parent()
					{
						Id = "parent",
						Name = "after",
					};

				s.SaveOrUpdate(parent);
				t.Commit();

				string[] warnings = GetAssignedIdentifierWarnings(ls);
				Assert.That(warnings.Length, Is.EqualTo(1));
				Assert.IsTrue(warnings[0].Contains("parent"));
			}

			using (ISession s = OpenSession())
			{
				Parent parent = s.CreateQuery("from Parent").UniqueResult<Parent>();
				Assert.That(parent.Name, Is.EqualTo("after"));
			}
		}

		[Test]
		public void UpdateNoWarning()
		{
			using (ISession s = OpenSession())
			{
				ITransaction t = s.BeginTransaction();

				s.Save(new Parent() { Id = "parent", Name = "before" });
				t.Commit();
			}

			using (LogSpy ls = new LogSpy(LogManager.GetLogger(typeof(AssignedFixture).Assembly, "NHibernate"), Level.Warn))
			using (ISession s = OpenSession())
			{
				ITransaction t = s.BeginTransaction();

				Parent parent =
					new Parent()
					{
						Id = "parent",
						Name = "after",
					};

				s.Update(parent);
				t.Commit();

				string[] warnings = GetAssignedIdentifierWarnings(ls);
				Assert.That(warnings.Length, Is.EqualTo(0));
			}

			using (ISession s = OpenSession())
			{
				Parent parent = s.CreateQuery("from Parent").UniqueResult<Parent>();
				Assert.That(parent.Name, Is.EqualTo("after"));
			}
		}

		[Test]
		public void InsertCascade()
		{
			using (ISession s = OpenSession())
			{
				ITransaction t = s.BeginTransaction();

				s.Save(new Child() { Id = "detachedChild" });
				t.Commit();
			}

			using (LogSpy ls = new LogSpy(LogManager.GetLogger(typeof(AssignedFixture).Assembly, "NHibernate"), Level.Warn))
			using (ISession s = OpenSession())
			{
				ITransaction t = s.BeginTransaction();

				Parent parent =
					new Parent()
					{
						Id = "parent",
						Children = new List<Child>(),
					};

				parent.Children.Add(new Child() { Id = "detachedChild", Parent = parent });
				parent.Children.Add(new Child() { Id = "transientChild", Parent = parent });

				s.Save(parent);
				t.Commit();

				long actual = s.CreateQuery("select count(c) from Child c").UniqueResult<long>();
				Assert.That(actual, Is.EqualTo(2));

				string[] warnings = GetAssignedIdentifierWarnings(ls);
				Assert.That(warnings.Length, Is.EqualTo(2));
				Assert.IsTrue(warnings[0].Contains("detachedChild"));
				Assert.IsTrue(warnings[1].Contains("transientChild"));
			}
		}

		[Test]
		public void InsertCascadeNoWarning()
		{
			using (ISession s = OpenSession())
			{
				ITransaction t = s.BeginTransaction();

				s.Save(new Child() { Id = "persistedChild" });
				t.Commit();
			}

			using (LogSpy ls = new LogSpy(LogManager.GetLogger(typeof(AssignedFixture).Assembly, "NHibernate"), Level.Warn))
			using (ISession s = OpenSession())
			{
				ITransaction t = s.BeginTransaction();

				Parent parent =
					new Parent()
					{
						Id = "parent",
						Children = new List<Child>(),
					};

				s.Save(parent);

				Child child1 = s.Load<Child>("persistedChild");
				child1.Parent = parent;
				parent.Children.Add(child1);

				Child child2 = new Child() { Id = "transientChild", Parent = parent };
				s.Save(child2);
				parent.Children.Add(child2);

				t.Commit();

				long actual = s.CreateQuery("select count(c) from Child c").UniqueResult<long>();
				Assert.That(actual, Is.EqualTo(2));

				string[] warnings = GetAssignedIdentifierWarnings(ls);
				Assert.That(warnings.Length, Is.EqualTo(0));
			}
		}

	}

}
