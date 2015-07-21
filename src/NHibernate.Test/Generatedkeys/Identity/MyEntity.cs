using System;
using System.Collections.Generic;

namespace NHibernate.Test.Generatedkeys.Identity
{
	public class MyEntity
	{
		private long id;
		private String name;
		private MySibling sibling;
		private ISet<MyChild> nonInverseChildren = new HashSet<MyChild>();
		private ISet<MyChild> inverseChildren = new HashSet<MyChild>();

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

		public virtual ISet<MyChild> NonInverseChildren
		{
			get { return nonInverseChildren; }
			set { nonInverseChildren = value; }
		}

		public virtual ISet<MyChild> InverseChildren
		{
			get { return inverseChildren; }
			set { inverseChildren = value; }
		}
	}
}
