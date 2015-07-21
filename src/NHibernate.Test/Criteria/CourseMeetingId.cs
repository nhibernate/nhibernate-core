using System;

namespace NHibernate.Test.Criteria
{
	[Serializable]
	public class CourseMeetingId
	{
		private string courseCode;
		private string day;
		private int period;
		private string location;
	
		public CourseMeetingId() { }
	
		public CourseMeetingId(Course course, string day, int period, string location)
		{
			this.courseCode = course.CourseCode;
			this.day = day;
			this.period = period;
			this.location = location;
		}
		
		public virtual string CourseCode
		{
			get { return courseCode; }
			set { courseCode = value; }
		}
		
		public virtual string Day
		{
			get { return day; }
			set { day = value; }
		}
		
		public virtual int Period
		{
			get { return period; }
			set { period = value; }
		}
		
		public virtual string Location
		{
			get { return location; }
			set { location = value; }
		}
		
		public override bool Equals(object obj)
		{
			CourseMeetingId that = obj as CourseMeetingId;
			if (that == null)
				return false;
			return courseCode == that.CourseCode && day == that.Day && period == that.Period && location == that.Location;
		}

		public override int GetHashCode()
		{
			return courseCode.GetHashCode() ^ day.GetHashCode() ^ period.GetHashCode() ^ location.GetHashCode();
		}		
	}
}