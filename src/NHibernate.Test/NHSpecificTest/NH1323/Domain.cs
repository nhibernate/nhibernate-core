using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1323
{
	[Serializable]
	public class MyClass
	{
		public MyClass()
		{
			Children = new List<MyChild>();
			Components = new List<MyComponent>();
			Elements = new List<string>();
		}
		public virtual string Description { get; set; }
		public virtual Guid Id { get; set; }
		public virtual IList<MyChild> Children { get; set; }
		public virtual IList<MyComponent> Components { get; set; }
		public virtual IList<string> Elements { get; set; }
	}

	[Serializable]
	public class MyChild
	{
		public virtual Guid Id { get; set; }
		public virtual MyClass Parent { get; set; }
	}

	[Serializable]
	public class MyComponent
	{
		public virtual string Something { get; set; }
	}
}