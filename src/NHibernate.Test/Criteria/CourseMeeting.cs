namespace NHibernate.Test.Criteria
{
	public class CourseMeeting 
	{
		private CourseMeetingId id;
		private Course course;
	
		public CourseMeeting() { }
	
		public CourseMeeting(Course course, string day, int period, string location)
		{
			this.id = new CourseMeetingId(course, day, period, location);
			this.course = course;
		}
	
		public virtual CourseMeetingId Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual Course Course
		{
			get { return course; }
			set { course = value; }
		}
	}
}