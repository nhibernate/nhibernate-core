using System.Collections.Generic;

namespace NHibernate.Test.Unionsubclass
{
	public class Location
	{
		private long id;
		private string name;
		private IList<Being> beings = new List<Being>();

		protected Location() { }

		public Location(string name)
		{
			this.name = name;
		}

		public virtual void AddBeing(Being b)
		{
			b.Location=this;
			beings.Add(b);
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

		public virtual IList<Being> Beings
		{
			get { return beings; }
			set { beings = value; }
		}
	}
}