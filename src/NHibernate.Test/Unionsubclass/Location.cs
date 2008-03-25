using System.Collections;

namespace NHibernate.Test.Unionsubclass
{
	public class Location
	{
		private long id;
		private string name;
		private IList beings = new ArrayList();

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

		public virtual IList Beings
		{
			get { return beings; }
			set { beings = value; }
		}
	}
}