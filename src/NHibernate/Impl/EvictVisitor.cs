using System;
using NHibernate.Type;

namespace NHibernate.Impl
{
	/// <summary>
	/// Evict any collections referenced by the object from the ISession cache.
	/// </summary>
	/// <remarks>
	/// This will <b>NOT</b> pick up any collections that were dereferenced, so
	/// they will be deleted (suboptimal but not exactly incorrect).
	/// </remarks>
	internal class EvictVisitor : AbstractVisitor
	{
		public EvictVisitor(SessionImpl session)
			: base( session )
		{
		}

		protected override object ProcessCollection(object collection, CollectionType type)
		{
			if( collection != null )
			{
				Session.EvictCollection( collection, type );
			}

			return null;
		}

	}
}