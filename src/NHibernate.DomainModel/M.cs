using System;
using Iesi.Collections;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for M.
	/// </summary>
	public class M
	{
		private long uniqueSequence;
		protected ISet children;

		public M()
		{
			children = new HashedSet();
		}

		public long UniqueSequence
		{
			get { return uniqueSequence; }
			set { uniqueSequence = value; }
		}

		public ISet Children
		{
			get { return children; }
			set { children = value; }
		}

		public void AddChildren( N pChildren )
		{
			pChildren.Parent = this;
			children.Add( pChildren );
		}

		public void RemoveChildren( N pChildren )
		{
			if ( children.Contains( pChildren ) )
			{
				children.Remove( pChildren );
			}
		}
	}
}
