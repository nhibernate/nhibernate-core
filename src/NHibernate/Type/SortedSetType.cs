using System.Collections;

using NHibernate.Collection;
using NHibernate.Engine;

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
		/// Wraps an <see cref="Iesi.Collections.ISet"/> in a <see cref="SortedSet"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="Iesi.Collections.ISet"/>.</param>
		/// <returns>
		/// An <see cref="SortedSet"/> that wraps the non NHibernate <see cref="Iesi.Collections.ISet"/>.
		/// </returns>
		public override PersistentCollection Wrap( ISessionImplementor session, object collection )
		{
			return new SortedSet( session, ( Iesi.Collections.ISet ) collection, comparer );

		}
	}
}