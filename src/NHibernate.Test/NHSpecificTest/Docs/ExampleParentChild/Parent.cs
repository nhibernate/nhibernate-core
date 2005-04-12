using System;
using Iesi.Collections;

namespace NHibernate.Test.NHSpecificTest.Docs.ExampleParentChild
{
	public class Parent
	{
		private long _id;
		private Iesi.Collections.ISet _children;

		public Parent()
		{
		}

		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public ISet Children
		{
			get { return _children; }
			set { _children = value; }
		}

		public void AddChild(Child c)
		{
			if( this.Children==null )
			{
				this.Children = new HashedSet();
			}
			this.Children.Add( c );
			c.Parent = this;
		}
	}
}
