using System.Collections;

namespace NHibernate.Test.UserCollection.Parameterized
{
	public class Entity
	{
		private string name;
		private IList values = new ArrayList();

		public Entity() {}

		public Entity(string name)
		{
			this.name = name;
		}

		public virtual string Name
		{
			get { return name; }
		}

		public virtual IList Values
		{
			get { return values; }
			set { values = value; }
		}
	}
}