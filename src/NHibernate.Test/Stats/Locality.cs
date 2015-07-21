namespace NHibernate.Test.Stats
{
	public class Locality
	{
		private long id;
		private string name;
		private Country country;

		public Locality()
		{
		}

		public Locality(string name, Country country)
		{
			this.name = name;
			this.country = country;
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

		public virtual Country Country
		{
			get { return country; }
			set { country = value; }
		}
	}
}
