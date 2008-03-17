using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using log4net;
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
	[Serializable]
	[DebuggerTypeProxy(typeof(CollectionProxy))]
	public class PersistentArrayHolder : AbstractPersistentCollection
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(PersistentArrayHolder));

		/// <summary>
		/// The <see cref="Array"/> that NHibernate is wrapping.
		/// </summary>
		private Array array;

		[NonSerialized]
		private System.Type elementClass;

		/// <summary>
		/// A temporary list that holds the objects while the PersistentArrayHolder is being
		/// populated from the database.
		/// </summary>
		[NonSerialized]
		private ArrayList tempList;


		public PersistentArrayHolder(ISessionImplementor session, object array) : base(session)
		{
			this.array = (Array) array;
			SetInitialized();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <returns></returns>
		protected override ICollection Snapshot(ICollectionPersister persister)
		{
			EntityMode entityMode = Session.EntityMode;

			int length = /*(array==null) ? temp.Count :*/ array.Length;
			Array result = System.Array.CreateInstance(persister.ElementClass, length);
			for (int i = 0; i < length; i++)
			{
				object elt = /*(array==null) ? temp[i] :*/ array.GetValue(i);
				try
				{
					result.SetValue(persister.ElementType.DeepCopy(elt, entityMode, persister.Factory), i);
				}
				catch (Exception e)
				{
					log.Error("Array element type error", e);
					throw new HibernateException("Array element type error", e);
				}
			}
			return result;
		}

		public override ICollection GetOrphans(object snapshot, string entityName)
		{
			object[] sn = (object[]) snapshot;
			object[] arr = (object[]) array;
			ArrayList result = new ArrayList(sn.Length);
			for (int i = 0; i < sn.Length; i++)
			{
				result.Add(sn[i]);
			}
			for (int i = 0; i < sn.Length; i++)
			{
				IdentityRemove(result, arr[i], entityName, Session);
			}
			return result;
		}


		public PersistentArrayHolder(ISessionImplementor session, ICollectionPersister persister)
			: base(session)
		{
			elementClass = persister.ElementClass;
		}

		/// <summary>
		/// 
		/// </summary>
		public object Array
		{
			get { return array; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override bool IsWrapper(object collection)
		{
			return array == collection;
		}

		public override bool EqualsSnapshot(ICollectionPersister persister)
		{
			IType elementType = persister.ElementType;
			Array snapshot = GetSnapshot() as Array;

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

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ICollection Elements()
		{
			//if (array==null) return tempList;
			int length = array.Length;
			IList list = new ArrayList(length);
			for (int i = 0; i < length; i++)
			{
				list.Add(array.GetValue(i));
			}
			return list;
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool Empty
		{
			get { return false; }
		}

		public override object ReadFrom(IDataReader rs, ICollectionPersister role, ICollectionAliases descriptor, object owner)
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

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override IEnumerable Entries()
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
			tempList = new ArrayList();
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
			int index = 0;
			foreach (object element in tempList)
			{
				array.SetValue(element, index);
				index++;
			}
			tempList = null;

			return true;
		}

		public override void BeforeInitialize(ICollectionPersister persister)
		{
		}

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
			SetInitialized();
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

		public override IEnumerable GetDeletes(IType elemType, bool indexIsFormula)
		{
			IList deletes = new ArrayList();
			Array sn = GetSnapshot() as Array;
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
			Array sn = GetSnapshot() as Array;
			return array.GetValue(i) != null && (i >= sn.Length || sn.GetValue(i) == null);
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType)
		{
			Array sn = GetSnapshot() as Array;
			return i < sn.Length &&
			       sn.GetValue(i) != null &&
			       array.GetValue(i) != null &&
			       elemType.IsDirty(array.GetValue(i), sn.GetValue(i), Session);
		}

		public override object GetIndex(object entry, int i)
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

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
		}

		public int Count
		{
			get { return array.Length; }
		}

		public IEnumerator GetEnumerator()
		{
			return array.GetEnumerator();
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { return this; }
		}

		public override IEnumerable Entries(ICollectionPersister persister)
		{
			return Elements();
		}
	}
}
