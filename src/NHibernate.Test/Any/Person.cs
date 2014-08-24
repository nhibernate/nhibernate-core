namespace NHibernate.Test.Any
{
	public class Person
	{
		private object data;
		private long id;
		private string name;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual object Data
		{
			get { return data; }
			set { data = value; }
		}
	}
}