namespace NHibernate.Test.NHSpecificTest.NH1275
{
	public class A
	{
		private int id;
		private string name;

		public A() {}

		public A(string name)
		{
			this.name = name;
		}

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}
