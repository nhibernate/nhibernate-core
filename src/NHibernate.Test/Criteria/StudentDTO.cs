using System;

namespace NHibernate.Test.Criteria
{
	public class StudentDTO
	{
#pragma warning disable 649
		private string studentName;
		private string courseDescription;
#pragma warning restore 649

		public StudentDTO()
		{
		}

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