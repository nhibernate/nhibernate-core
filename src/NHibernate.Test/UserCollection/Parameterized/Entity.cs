using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.UserCollection.Parameterized
{
	public class Entity
	{
		private string name;
		private IList<string> values = new List<string>();

		public Entity() {}

		public Entity(string name)
		{
			this.name = name;
		}

		public virtual string Name
		{
			get { return name; }
		}

		public virtual IList<string> Values
		{
			get { return values; }
			set { values = value; }
		}
	}
}