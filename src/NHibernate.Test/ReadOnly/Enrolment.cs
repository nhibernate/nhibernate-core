using System;

namespace NHibernate.Test.ReadOnly
{
	public class Enrolment
	{
		private Student student;
		private Course course;
		private long studentNumber;
		private string courseCode;
		private short semester;
		private short year;
		
		public virtual Student Student
		{
			get { return student; }
			set { student = value; }
		}
		
		public virtual Course Course
		{
			get { return course; }
			set { course = value; }
		}
		
		public virtual long StudentNumber
		{
			get { return studentNumber; }
			set { studentNumber = value; }
		}
		
		public virtual string CourseCode
		{
			get { return courseCode; }
			set { courseCode = value; }
		}
		
		public virtual short Year
		{
			get { return year; }
			set { year = value; }
		}
		
		public virtual short Semester
		{
			get { return semester; }
			set { semester = value; }
		}

		public override int GetHashCode()
		{
			return courseCode.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			Enrolment other = obj as Enrolment;
			
			if (other == null)
				return false;
			
			return (courseCode.Equals(other.courseCode) && studentNumber.Equals(other.studentNumber));
		}
	}
}
