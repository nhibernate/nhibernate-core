using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for M.
	/// </summary>
	public class M
	{
		private long uniqueSequence;
		protected ISet<N> children;

		public M()
		{
			children = new HashSet<N>();
		}

		public long UniqueSequence
		{
			get { return uniqueSequence; }
			set { uniqueSequence = value; }
		}

		public ISet<N> Children
		{
			get { return children; }
			set { children = value; }
		}

		public void AddChildren(N pChildren)
		{
			pChildren.Parent = this;
			children.Add(pChildren);
		}

		public void RemoveChildren(N pChildren)
		{
			if (children.Contains(pChildren))
			{
				children.Remove(pChildren);
			}
		}
	}
}