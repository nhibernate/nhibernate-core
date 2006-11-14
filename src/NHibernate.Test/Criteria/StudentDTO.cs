using System;

namespace NHibernate.Test.Criteria
{
	public class StudentDTO
	{
		private string studentName;
		private string courseDescription;

		public StudentDTO() { }

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
