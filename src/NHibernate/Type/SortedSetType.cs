using System.Collections;
using Iesi.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using SortedSet = NHibernate.Collection.SortedSet;

namespace NHibernate.Type
{
	/// <summary>
	/// Extends the SetType to provide Sorting.
	/// </summary>
	public class SortedSetType : SetType
	{
		private IComparer comparer;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="role"></param>
		/// <param name="comparer"></param>
		public SortedSetType( string role, IComparer comparer ) : base( role )
		{
			this.comparer = comparer;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="persister"></param>
		/// <returns></returns>
		public override PersistentCollection Instantiate( ISessionImplementor session, CollectionPersister persister )
		{
			SortedSet sortedSet = new SortedSet( session, comparer );
			return sortedSet;
		}

		//public System.Type ReturnedClass {get;} -> was overridden in H2.0.3
		// because they have different Interfaces for Sorted/UnSorted - since .NET
		// doesn't have that I don't need to override it.

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override PersistentCollection Wrap( ISessionImplementor session, object collection )
		{
			return new SortedSet( session, ( ISet ) collection, comparer );

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="persister"></param>
		/// <param name="disassembled"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override PersistentCollection AssembleCachedCollection( ISessionImplementor session, CollectionPersister persister, object disassembled, object owner )
		{
			return new SortedSet( session, persister, comparer, disassembled, owner );
		}

	}
}