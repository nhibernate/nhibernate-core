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
		IDictionary children;
		IDictionary	aliases;

		static object emptyObject = new object();

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

		private IDictionary ChildrenSet
		{
			get 
			{ 
				if (children==null) children = new Hashtable();
				return children; 
			}
			set { children = value; }
		}

		public ICollection Children 
		{
			get { return ChildrenSet;}
		}

		public void AddChild(Child child) 
		{
			ChildrenSet[child] = emptyObject;
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
