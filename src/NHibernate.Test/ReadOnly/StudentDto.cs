namespace NHibernate.Test.ReadOnly
{
	public class StudentDto
	{
		// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
		private string studentName;
		private string courseDescription;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

		public string Name
		{
			get { return studentName; }
		}
		
		public string Description
		{
			get { return courseDescription; }
		}
	}
}
