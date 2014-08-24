namespace NHibernate.Test.ReadOnly
{
	public class StudentDto
	{
		private string studentName;
		private string courseDescription;
		
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
