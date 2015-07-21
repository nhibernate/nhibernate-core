namespace NHibernate.Test.GeneratedTest
{
	public class ComponentOwner
	{
		private long id;
		private string name;
		private Component component;

		public ComponentOwner()
		{
		}

		public ComponentOwner(string name)
		{
			this.name = name;
		}

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

		public virtual Component Component
		{
			get { return component; }
			set { component = value; }
		}
	}
}