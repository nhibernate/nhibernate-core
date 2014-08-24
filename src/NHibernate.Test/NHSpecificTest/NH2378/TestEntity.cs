namespace NHibernate.Test.NHSpecificTest.NH2378
{
    public class TestEntity
    {
    	private string name;
        private int id;
    	private Person testPerson;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

    	public Person TestPerson
    	{
			get { return testPerson; }
			set { testPerson = value; }
    	}
    }

	public class Person
	{
		private string name;
		private short id;

		public short Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}