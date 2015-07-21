using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Event.Default
{
	/// <summary>
	/// A Visitor that determines if a dirty collection was found.
	/// </summary>
	/// <remarks>
	/// <list type="number">
	///		<listheader>
	///			<description>Reason for dirty collection</description>
	///		</listheader>
	///		<item>
	///			<description>
	///			If it is a new application-instantiated collection, return true (does not occur anymore!)
	///			</description>
	///		</item>
	///		<item>
	///			<description>
	///			If it is a component, recurse.
	///			</description>
	///		</item>
	///		<item>
	///			<description>
	///			If it is a wrapped collection, ask the collection entry.
	///			</description>
	///		</item>
	/// </list>
	/// </remarks>
	public class DirtyCollectionSearchVisitor : AbstractVisitor
	{
		private readonly bool[] propertyVersionability;
		private bool dirty = false;

		public DirtyCollectionSearchVisitor(IEventSource session, bool[] propertyVersionability)
			: base(session)
		{
			this.propertyVersionability = propertyVersionability;
		}

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if a dirty collection was found.
		/// </summary>
		/// <value><see langword="true" /> if a dirty collection was found.</value>
		public bool WasDirtyCollectionFound
		{
			get { return dirty; }
		}

		internal override object ProcessCollection(object collection, CollectionType type)
		{

			if (collection != null)
			{
				ISessionImplementor session = Session;
				IPersistentCollection persistentCollection;

				if (type.IsArrayType)
				{
					persistentCollection = session.PersistenceContext.GetCollectionHolder(collection);
					// if no array holder we found an unwrappered array (this can't occur,
					// because we now always call wrap() before getting to here)
					// return (ah==null) ? true : searchForDirtyCollections(ah, type);
				}
				else
				{
					// if not wrappered yet, its dirty (this can't occur, because
					// we now always call wrap() before getting to here)
					// return ( ! (obj instanceof PersistentCollection) ) ?
					//true : searchForDirtyCollections( (PersistentCollection) obj, type );
					persistentCollection = (IPersistentCollection)collection;
				}

				if (persistentCollection.IsDirty)
				{
					//we need to check even if it was not initialized, because of delayed adds!
					dirty = true;
					return null; //NOTE: EARLY EXIT!
				}
			}
			return null;
		}

		internal override bool IncludeEntityProperty(object[] values, int i)
		{
			return propertyVersionability[i] && base.IncludeEntityProperty(values, i);
		}
	}
}
