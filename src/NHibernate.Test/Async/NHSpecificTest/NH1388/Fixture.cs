﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1388
{
	using System.Threading.Tasks;

	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return TestDialect.SupportsEmptyInsertsOrHasNonIdentityNativeGenerator;
		}

		[Test]
		public async Task BagTestAsync()
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

				await (session.SaveAsync(subject1));
				await (session.SaveAsync(subject2));
				studentId = await (session.SaveAsync(student));

				await (session.FlushAsync());
				await (t.CommitAsync());
			}
			// Remove major for subject 2.
			using (ISession session = OpenSession())
			{
				ITransaction t = session.BeginTransaction();
				var student = await (session.GetAsync<Student>(studentId));
				var subject2 = await (session.GetAsync<Subject>(2));

				// Remove major.
				student.Majors.Remove(subject2);

				await (session.FlushAsync());
				await (t.CommitAsync());
			}

			// Get major for subject 2.
			using (ISession session = OpenSession())
			{
				ITransaction t = session.BeginTransaction();
				var student = await (session.GetAsync<Student>(studentId));
				var subject2 = await (session.GetAsync<Subject>(2));

				Assert.IsNotNull(subject2);

				// Major for subject 2 should have been removed.
				Assert.IsFalse(student.Majors.ContainsKey(subject2));

				await (t.CommitAsync());
			}

			// Remove all - NHibernate will now succeed in removing all.
			using (ISession session = OpenSession())
			{
				ITransaction t = session.BeginTransaction();
				var student = await (session.GetAsync<Student>(studentId));
				student.Majors.Clear();
				await (session.FlushAsync());
				await (t.CommitAsync());
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
	}
}
