using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3505
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (ISession s = Sfi.OpenSession())
			{
				s.Delete("from Student");
			    s.Delete("from Teacher");
				s.Flush();
			}
		}

		[Test]
		public void StatelessSessionLazyUpdate()
		{
		    var s = OpenSession();
		    Guid studentId;
		    Guid teacherId;
			try
			{
			    var teacher = new Teacher {Name = "Wise Man"};
				s.Save(teacher);
			    teacherId = teacher.Id;
			    var student = new Student {Name = "Rebelious Teenager", Teacher = teacher};
			    s.Save(student);
			    studentId = student.Id;
				s.Flush();
			}
			finally
			{
				s.Close();
			}

			var ss = Sfi.OpenStatelessSession();
			try
			{
			    var trans = ss.BeginTransaction();
                try
                {
                    var student = ss.Get<Student>(studentId);
                    Assert.AreEqual(teacherId, student.Teacher.Id);
                    Assert.AreEqual("Rebelious Teenager", student.Name);
                    student.Name = "Young Protege";
                    ss.Update(student);
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
			finally
			{
				ss.Close();
			}

			s = OpenSession();
			try
			{
			    var student = s.Get<Student>(studentId);
                Assert.AreEqual(teacherId, student.Teacher.Id);
			    Assert.AreEqual("Young Protege", student.Name);
			}
			finally
			{
				s.Close();
			}
		}
	}
}
