using System;

namespace NHibernate.Test.Criteria
{
	[Serializable]
	public class Enrolment
	{
		private Student student;
		private Course course;
		private long studentNumber;
		private string courseCode = string.Empty;
		private short year;
		private short semester;
		
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

		public override bool Equals(object obj)
		{
			Enrolment that = obj as Enrolment;
			if (that == null)
				return false;
			return studentNumber == that.StudentNumber && courseCode.Equals(that.CourseCode);
		}

		public override int GetHashCode()
		{
			return courseCode.GetHashCode();
		}
	}
}