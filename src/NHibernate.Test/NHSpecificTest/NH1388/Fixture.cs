﻿using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1388
{
	public class Student
	{
		// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
		private int _id;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
		private readonly IDictionary<Subject, Major> _majors = new Dictionary<Subject, Major>();

		public virtual int Id
		{
			get { return _id; }
		}

		public virtual IDictionary<Subject, Major> Majors
		{
			get { return _majors; }
		}
	}

	public class Subject
	{
		public int Id { get; set; }

		public string Title { get; set; }
	}

	public class Major
	{
		public string Note { get; set; }
	}

	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return TestDialect.SupportsEmptyInsertsOrHasNonIdentityNativeGenerator;
		}

		[Test]
		public void BagTest()
		{
			object studentId;

			// Set major.
			using (ISession session = OpenSession())
			{
				ITransaction t = session.BeginTransaction();
				var student = new Student();
				var subject1 = new Subject {Id = 1};
				var subject2 = new Subject {Id = 2};

				// Create major objects.
				var major1 = new Major {Note = ""};

				var major2 = new Major {Note = ""};

				// Set major objects.
				student.Majors[subject1] = major1;
				student.Majors[subject2] = major2;

				session.Save(subject1);
				session.Save(subject2);
				studentId = session.Save(student);

				session.Flush();
				t.Commit();
			}
			// Remove major for subject 2.
			using (ISession session = OpenSession())
			{
				ITransaction t = session.BeginTransaction();
				var student = session.Get<Student>(studentId);
				var subject2 = session.Get<Subject>(2);

				// Remove major.
				student.Majors.Remove(subject2);

				session.Flush();
				t.Commit();
			}

			// Get major for subject 2.
			using (ISession session = OpenSession())
			{
				ITransaction t = session.BeginTransaction();
				var student = session.Get<Student>(studentId);
				var subject2 = session.Get<Subject>(2);

				Assert.IsNotNull(subject2);

				// Major for subject 2 should have been removed.
				Assert.IsFalse(student.Majors.ContainsKey(subject2));

				t.Commit();
			}

			// Remove all - NHibernate will now succeed in removing all.
			using (ISession session = OpenSession())
			{
				ITransaction t = session.BeginTransaction();
				var student = session.Get<Student>(studentId);
				student.Majors.Clear();
				session.Flush();
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			// clean up the database
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				foreach (var student in session.CreateCriteria(typeof (Student)).List<Student>())
				{
					session.Delete(student);
				}
				foreach (var subject in session.CreateCriteria(typeof (Subject)).List<Subject>())
				{
					session.Delete(subject);
				}
				tran.Commit();
			}
		}

		protected override string CacheConcurrencyStrategy
		{
			get { return null; }
		}
	}
}
