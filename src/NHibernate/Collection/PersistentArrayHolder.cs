using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;

using NHibernate.DebugHelpers;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Collection
{
	/// <summary>
	/// A persistent wrapper for an array. lazy initialization is NOT supported
	/// </summary>
	/// <remarks> Use of Hibernate arrays is not really recommended. </remarks>
	[Serializable]
	[DebuggerTypeProxy(typeof (CollectionProxy))]
	public partial class PersistentArrayHolder : AbstractPersistentCollection, ICollection
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof (PersistentArrayHolder));

		private Array array;

		[NonSerialized] private readonly System.Type elementClass;

		/// <summary>
		/// A temporary list that holds the objects while the PersistentArrayHolder is being
		/// populated from the database.
		/// </summary>
		[NonSerialized] private List<object> tempList;

		public PersistentArrayHolder(ISessionImplementor session, object array) : base(session)
		{
			this.array = (Array) array;
			SetInitialized();
		}

		public PersistentArrayHolder(ISessionImplementor session, ICollectionPersister persister) : base(session)
		{
			elementClass = persister.ElementClass;
		}

		/// <summary>
		/// Gets or sets the array.
		/// </summary>
		/// <value>The array.</value>
		public object Array
		{
			get { return array; }
			protected set
			{
				array = (Array)value;
			}
		}

		/// <summary>
		/// Returns the user-visible portion of the NHibernate PersistentArrayHolder.
		/// </summary>
		/// <returns>
		/// The array that contains the data, not the NHibernate wrapper.
		/// </returns>
		public override object GetValue()
		{
			return array;
		}

		public override object GetSnapshot(ICollectionPersister persister)
		{
			int length = array.Length;
			Array result = System.Array.CreateInstance(persister.ElementClass, length);
			for (int i = 0; i < length; i++)
			{
				object elt = array.GetValue(i);
				try
				{
					result.SetValue(persister.ElementType.DeepCopy(elt, persister.Factory), i);
				}
				catch (Exception e)
				{
					log.Error("Array element type error", e);
					throw new HibernateException("Array element type error", e);
				}
			}
			return result;
		}

		public override bool IsSnapshotEmpty(object snapshot)
		{
			return ((Array) snapshot).Length == 0;
		}

		public override ICollection GetOrphans(object snapshot, string entityName)
		{
			object[] sn = (object[]) snapshot;
			object[] arr = (object[]) array;
			List<object> result = new List<object>(sn);
			for (int i = 0; i < sn.Length; i++)
			{
				IdentityRemove(result, arr[i], entityName, Session);
			}
			return result;
		}

		public override bool IsWrapper(object collection)
		{
			return array == collection;
		}

		public override bool EqualsSnapshot(ICollectionPersister persister)
		{
			IType elementType = persister.ElementType;
			Array snapshot = (Array) GetSnapshot();

			int xlen = snapshot.Length;
			if (xlen != array.Length)
			{
				return false;
			}
			for (int i = 0; i < xlen; i++)
			{
				if (elementType.IsDirty(snapshot.GetValue(i), array.GetValue(i), Session))
				{
					return false;
				}
			}
			return true;
		}

		public ICollection Elements()
		{
			// NH Different implementation but same result
			return (ICollection) array.Clone();
		}

		public override bool Empty
		{
			get { return false; }
		}

		public override object ReadFrom(DbDataReader rs, ICollectionPersister role, ICollectionAliases descriptor, object owner)
		{
			object element = role.ReadElement(rs, owner, descriptor.SuffixedElementAliases, Session);
			int index = (int) role.ReadIndex(rs, descriptor.SuffixedIndexAliases, Session);
			for (int i = tempList.Count; i <= index; i++)
			{
				tempList.Add(null);
			}
			tempList[index] = element;
			return element;
		}

		public override IEnumerable Entries(ICollectionPersister persister)
		{
			return Elements();
		}

		/// <summary>
		/// Before <see cref="ReadFrom" /> is called the PersistentArrayHolder needs to setup 
		/// a temporary list to hold the objects.
		/// </summary>
		public override void BeginRead()
		{
			base.BeginRead();
			tempList = new List<object>();
		}

		/// <summary>
		/// Takes the contents stored in the temporary list created during <see cref="BeginRead" />
		/// that was populated during <see cref="ReadFrom" /> and write it to the underlying 
		/// array.
		/// </summary>
		public override bool EndRead(ICollectionPersister persister)
		{
			SetInitialized();
			array = System.Array.CreateInstance(elementClass, tempList.Count);
			for (int i = 0; i < tempList.Count; i++)
			{
				array.SetValue(tempList[i], i);
			}
			tempList = null;
			return true;
		}

		public override void BeforeInitialize(ICollectionPersister persister, int anticipatedSize) {}

		public override bool IsDirectlyAccessible
		{
			get { return true; }
		}

		/// <summary>
		/// Initializes this array holder from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the Array.</param>
		/// <param name="disassembled">The disassembled Array.</param>
		/// <param name="owner">The owner object.</param>
		public override void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner)
		{
			object[] cached = (object[]) disassembled;

			array = System.Array.CreateInstance(persister.ElementClass, cached.Length);

			for (int i = 0; i < cached.Length; i++)
			{
				array.SetValue(persister.ElementType.Assemble(cached[i], Session, owner), i);
			}
		}

		public override object Disassemble(ICollectionPersister persister)
		{
			int length = array.Length;
			object[] result = new object[length];
			for (int i = 0; i < length; i++)
			{
				result[i] = persister.ElementType.Disassemble(array.GetValue(i), Session, null);
			}
			return result;
		}

		public override IEnumerable GetDeletes(ICollectionPersister persister, bool indexIsFormula)
		{
			IList deletes = new List<object>();
			Array sn = (Array) GetSnapshot();
			int snSize = sn.Length;
			int arraySize = array.Length;
			int end;
			if (snSize > arraySize)
			{
				for (int i = arraySize; i < snSize; i++)
				{
					deletes.Add(i);
				}
				end = arraySize;
			}
			else
			{
				end = snSize;
			}
			for (int i = 0; i < end; i++)
			{
				if (array.GetValue(i) == null && sn.GetValue(i) != null)
				{
					deletes.Add(i);
				}
			}
			return deletes;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			Array sn = (Array) GetSnapshot();
			return array.GetValue(i) != null && (i >= sn.Length || sn.GetValue(i) == null);
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType)
		{
			Array sn = (Array) GetSnapshot();
			return
				i < sn.Length && sn.GetValue(i) != null && array.GetValue(i) != null
				&& elemType.IsDirty(array.GetValue(i), sn.GetValue(i), Session);
		}

		public override object GetIndex(object entry, int i, ICollectionPersister persister)
		{
			return i;
		}

		public override object GetElement(object entry)
		{
			return entry;
		}

		public override object GetSnapshotElement(object entry, int i)
		{
			Array sn = (Array) GetSnapshot();
			return sn.GetValue(i);
		}

		public override bool EntryExists(object entry, int i)
		{
			return entry != null;
		}

		#region ICollection Members

		// NH Different : we implement one of the "minimal" interface the NET Array support
		void ICollection.CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
		}

		int ICollection.Count
		{
			get { return array.Length; }
		}

		object ICollection.SyncRoot
		{
			get { return this; }
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return array.GetEnumerator();
		}

		#endregion
	}
}