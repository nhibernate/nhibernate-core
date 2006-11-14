using System;

namespace NHibernate.Test.Criteria
{
	public class Course
	{
		private string courseCode;
		public virtual string CourseCode
		{
			get { return courseCode; }
			set { courseCode = value; }
		}

		private string description;
		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}
	}
}
