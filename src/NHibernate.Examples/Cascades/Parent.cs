using System;
using System.Collections;

namespace NHibernate.Examples.Cascades
{
	/// <summary>
	/// Summary description for Parent.
	/// </summary>
	public class Parent
	{
		int id;
		string name;
		Iesi.Collections.ISet children;
		IDictionary	aliases;

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

		public Iesi.Collections.ISet Children
		{
			get 
			{ 
				if (children==null) 
				{
					children = new Iesi.Collections.HashedSet();
				}
				return children; 
			}
			set { children = value; }
		}

		public void AddChild(Child child) 
		{
			Children.Add( child );
			child.SingleParent = this;
		}

		public IDictionary Aliases 
		{
			get 
			{ 
				if(aliases==null) aliases = new Hashtable();
				return aliases; 
			}
			set { aliases = value; }
		}

	}
}
