using System;
using System.Collections;

using Iesi.Collections;

namespace NHibernate.Examples.Cascades
{
	/// <summary>
	/// Summary description for Parent.
	/// </summary>
	public class Parent
	{
		private int id;
		private string name;
		private ISet children;
		private IDictionary aliases;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public ISet Children
		{
			get
			{
				if (children == null)
				{
					children = new HashedSet();
				}
				return children;
			}
			set { children = value; }
		}

		public void AddChild(Child child)
		{
			Children.Add(child);
			child.SingleParent = this;
		}

		public IDictionary Aliases
		{
			get
			{
				if (aliases == null) aliases = new Hashtable();
				return aliases;
			}
			set { aliases = value; }
		}
	}
}