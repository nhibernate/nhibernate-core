namespace NHibernate.Test.ReadOnly
{
	public class Course
	{
		private string courseCode;
		private string description;

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
	}
}
