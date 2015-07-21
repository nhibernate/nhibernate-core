using System;

namespace NHibernate.Test.Generatedkeys.Identity
{
	public class MySibling
	{
		private long id;
		private string name;
		private MyEntity entity;

		public MySibling()
		{
		}

		public MySibling(String name)
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

		public virtual MyEntity Entity
		{
			get { return entity; }
			set { entity = value; }
		}
	}
}
