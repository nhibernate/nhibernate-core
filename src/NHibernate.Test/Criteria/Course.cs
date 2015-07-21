using System;
using System.Collections.Generic;

namespace NHibernate.Test.Criteria
{
	public class Course
	{
		private string courseCode;
		private string description;
		private ISet<CourseMeeting> courseMeetings = new HashSet<CourseMeeting>();
		
		public virtual string CourseCode
		{
			get { return courseCode; }
			set { courseCode = value; }
		}

		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}
		
		public virtual ISet<CourseMeeting> CourseMeetings
		{
			get { return courseMeetings; }
			set { courseMeetings = value; }
		}
	}
}