namespace NHibernate.Test.NHSpecificTest.NH962
{
	public class Child
	{
		private int id;
		private string name;
		private Parent parent;

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

		public virtual Parent Parent
		{
			get { return parent; }
			set { parent = value; }
		}
	}
}