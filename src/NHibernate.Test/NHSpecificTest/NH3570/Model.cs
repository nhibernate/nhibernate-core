using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3570
{
	public class UniParent
	{
		public UniParent()
		{
			Children = new List<UniChild>();
		}

		public virtual Guid Id { get; set; }
		public virtual IList<UniChild> Children { get; set; }
		public virtual int Version { get; set; }
	}

	public class UniChild
	{
		public virtual Guid Id { get; set; }
	}

	public class BiParent
	{
		public BiParent()
		{
			Children = new List<BiChild>();
		}

		public virtual Guid Id { get; set; }
		public virtual IList<BiChild> Children { get; set; }
		public virtual int Version { get; set; }
		
		public virtual void AddChild(BiChild child)
		{
			child.Parent = this;
			Children.Add(child);
		}
	}

	public class BiChild
	{
		public virtual Guid Id { get; set; }
		public virtual BiParent Parent { get; set; }
	}
}