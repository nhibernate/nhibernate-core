namespace NHibernate.Test.NHSpecificTest.LoadingNullEntityInSet
{
	public class WantedProfession
	{
		private int id;
		private PrimaryProfession primaryProfession;
		private SecondaryProfession secondaryProfession;
		private Employee employee;

		public PrimaryProfession PrimaryProfession
		{
			get { return primaryProfession; }
			set { primaryProfession = value; }
		}

		public SecondaryProfession SecondaryProfession
		{
			get { return secondaryProfession; }
			set { secondaryProfession = value; }
		}

		public Employee Employee
		{
			get { return employee; }
			set { employee = value; }
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}