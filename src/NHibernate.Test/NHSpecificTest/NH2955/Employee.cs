namespace NHibernate.Test.NHSpecificTest.NH2955
{
	public class Employee
	{
		private int id;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		private string firstName;

		public string FirstName
		{
			get { return firstName; }
			set { firstName = value; }
		}

		private string lastName;

		public string LastName
		{
			get { return lastName; }
			set { lastName = value; }
		}

		private string department;

		public string Department
		{
			get { return department; }
			set { department = value; }
		}
	}
}
