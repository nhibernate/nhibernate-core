using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3731
{
	[Serializable]
	class Parent
	{
		public Parent()
		{
			ChildrenList = new List<ListChild>();
			ChildrenMap = new Dictionary<string, MapChild>();
		}

		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<ListChild> ChildrenList { get; set; }
		public virtual IDictionary<string, MapChild> ChildrenMap { get; set; }
	}

	[Serializable]
	class ListChild
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	[Serializable]
	class MapChild
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}