namespace NHibernate.Test.Unionsubclass
{
	public class Thing
	{
		private long id;
		private string description;
		private Being owner;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}

		public virtual Being Owner
		{
			get { return owner; }
			set { owner = value; }
		}
	}
}