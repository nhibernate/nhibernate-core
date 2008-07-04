using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1300
{
	public class Parent
	{
		private int id;
		private string description;
		private IList<Child> childs = new List<Child>();
		
		public Parent() {}

		public Parent(string description)
		{
			this.description = description;
		}

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}

		public virtual IEnumerable<Child> Childs
		{
			get { return childs; }
		}

		public virtual Child AddChild()
		{
			Child result = new Child(this);
			childs.Add(result);
			return result;
		}
	}

	public class Child
	{
		private int id;
		private string description;
		private Parent owner;
		protected Child() {}

		public Child(Parent owner)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			this.owner = owner;
		}

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}

		public virtual Parent Owner
		{
			get { return owner; }
			set { owner = value; }
		}
	}
}