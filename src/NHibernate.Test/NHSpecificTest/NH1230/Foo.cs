namespace NHibernate.Test.NHSpecificTest.NH1230
{
	public class Foo
	{
		private string description;
		private int id;

		public Foo()
		{
			
		}

		public Foo(string description)
		{
			this.description = description;
		}

		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		public int Id
		{
			set { id = value; }
			get { return id; }
		}
	}
}