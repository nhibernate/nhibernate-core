using System;

namespace NHibernate.Test.Criteria
{
	[Serializable]
	public class Enrolment
	{
		private Student student;

		public virtual Student Student
		{
			get { return student; }
			set { student = value; }
		}

		private Course course;

		public virtual Course Course
		{
			get { return course; }
			set { course = value; }
		}

		private long studentNumber;

		public virtual long StudentNumber
		{
			get { return studentNumber; }
			set { studentNumber = value; }
		}

		private string courseCode = string.Empty;

		public virtual string CourseCode
		{
			get { return courseCode; }
			set { courseCode = value; }
		}

		private short year;

		public virtual short Year
		{
			get { return year; }
			set { year = value; }
		}

		private short semester;

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