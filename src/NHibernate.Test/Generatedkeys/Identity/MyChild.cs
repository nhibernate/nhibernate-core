using System;

namespace NHibernate.Test.Generatedkeys.Identity
{
	public class MyChild
	{
		private long id;
		private string name;
		private MyEntity inverseParent;

		public MyChild()
		{
		}

		public MyChild(String name)
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

		public virtual MyEntity InverseParent
		{
			get { return inverseParent; }
			set { inverseParent = value; }
		}
	}
}
