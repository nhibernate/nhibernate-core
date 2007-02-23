using System;
using NHibernate.Collection;
using NHibernate.Type;

namespace NHibernate.Impl
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
	internal class DirtyCollectionSearchVisitor : AbstractVisitor
	{
		private bool _dirty;
		private bool[] _propertyVersionability;

		public DirtyCollectionSearchVisitor(SessionImpl session, bool[] propertyVersionability)
			: base(session)
		{
			_propertyVersionability = propertyVersionability;
		}

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if a dirty collection was found.
		/// </summary>
		/// <value><c>true</c> if a dirty collection was found.</value>
		public bool WasDirtyCollectionFound
		{
			get { return _dirty; }
		}

		protected override object ProcessCollection(object collection, CollectionType type)
		{
			if (collection != null)
			{
				SessionImpl session = Session;
				IPersistentCollection coll;

				if (type.IsArrayType)
				{
					coll = session.GetArrayHolder(collection);
					// if no array holder we found an unwrappered array (this can't occur,
					// because we now always call wrap() before getting to here)
					// return (ah==null) ? true : searchForDirtyCollections(ah, type);
				}
				else
				{
					// if not wrappered yet, its dirty (this can't occur, because 
					// we now always call wrap() before getting to here) 
					// return ( ! (obj is AbstractPersistentCollection) ) ?
					//	true : SearchForDirtyCollections( (AbstractPersistentCollection) obj, type );
					coll = (IPersistentCollection) collection;
				}

				if (coll.IsDirty)
				{
					_dirty = true;
					return null; // NOTE: early exit
				}
			}
			return null;
		}

		protected override bool IncludeEntityProperty(object[] values, int i)
		{
			return _propertyVersionability[i] && base.IncludeEntityProperty(values, i);
		}
	}
}