namespace NHibernate.Test.Deletetransient
{
	public class Address
	{
		private long id;
		private string info;
		public Address() {}
		public Address(string info)
		{
			this.info = info;
		}

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Info
		{
			get { return info; }
			set { info = value; }
		}
	}
}