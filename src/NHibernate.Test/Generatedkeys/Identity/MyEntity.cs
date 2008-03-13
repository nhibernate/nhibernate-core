using System;
using Iesi.Collections;

namespace NHibernate.Test.Generatedkeys.Identity
{
	public class MyEntity
	{
		private long id;
		private String name;
		private MySibling sibling;
		private ISet nonInverseChildren = new HashedSet();
		private ISet inverseChildren = new HashedSet();

		public MyEntity()
		{
		}

		public MyEntity(string name)
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

		public virtual MySibling Sibling
		{
			get { return sibling; }
			set { sibling = value; }
		}

		public virtual ISet NonInverseChildren
		{
			get { return nonInverseChildren; }
			set { nonInverseChildren = value; }
		}

		public virtual ISet InverseChildren
		{
			get { return inverseChildren; }
			set { inverseChildren = value; }
		}
	}
}
