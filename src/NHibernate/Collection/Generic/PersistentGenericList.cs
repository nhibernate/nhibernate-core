using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NHibernate.DebugHelpers;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Collection.Generic
{
	/// <summary>
	/// A persistent wrapper for an <see cref="IList{T}"/>
	/// </summary>
	/// <typeparam name="T">The type of the element the list should hold.</typeparam>
	/// <remarks>The underlying collection used is a <see cref="List{T}"/></remarks>
	[Serializable]
	[DebuggerTypeProxy(typeof (CollectionProxy<>))]
	public class PersistentGenericList<T> : PersistentList, IList<T>
	{
		// TODO NH: find a way to writeonce (no duplicated code from PersistentList)

		/* NH considerations:
		 * For various reason we know that the underlining type will be a List<T> or a 
		 * PersistentList<T>; in both cases the class implement all we need to don't duplicate
		 * all code from PersistentList.
		 * In the explicit implementation of IList<T> we need to duplicate 
		 * code to take advantage from the better performance the use of generic implementation have 
		 * (mean .NET implementation of the underlining list).
		 * In other cases, where PersistentList use for example list.Add, a cast, probably, is more
		 * expensive than .NET original implementation.
		 */
		protected IList<T> glist;

		protected override object DefaultForType
		{
			get { return default(T); }
		}

		public PersistentGenericList() {}
		public PersistentGenericList(ISessionImplementor session) : base(session) {}

		public PersistentGenericList(ISessionImplementor session, IList<T> list) : base(session, list as IList)
		{
			glist = list;
		}

		public override void BeforeInitialize(ICollectionPersister persister, int anticipatedSize)
		{
			glist = (IList<T>) persister.CollectionType.Instantiate(anticipatedSize);
			list = (IList) glist;
		}

		#region IList<T> Members

		int IList<T>.IndexOf(T item)
		{
			Read();
			return glist.IndexOf(item);
		}

		void IList<T>.Insert(int index, T item)
		{
			if (index < 0)
			{
				throw new IndexOutOfRangeException("negative index");
			}
			if (!IsOperationQueueEnabled)
			{
				Write();
				glist.Insert(index, item);
			}
			else
			{
				QueueOperation(new AddDelayedOperation(this, index, item));
			}
		}

		T IList<T>.this[int index]
		{
			get
			{
				if (index < 0)
				{
					throw new IndexOutOfRangeException("negative index");
				}
				object result = ReadElementByIndex(index);
				if (result == Unknown)
				{
					return glist[index];
				}
				else
				{
					return (T) result;
				}
			}
			set
			{
				if (index < 0)
				{
					throw new IndexOutOfRangeException("negative index");
				}
				object old = PutQueueEnabled ? ReadElementByIndex(index) : Unknown;
				if (old == Unknown)
				{
					Write();
					glist[index] = value;
				}
				else
				{
					QueueOperation(new SetDelayedOperation(this, index, value, old));
				}
			}
		}

		#endregion

		#region ICollection<T> Members

		void ICollection<T>.Add(T item)
		{
			if (!IsOperationQueueEnabled)
			{
				Write();
				glist.Add(item);
			}
			else
			{
				QueueOperation(new SimpleAddDelayedOperation(this, item));
			}
		}

		bool ICollection<T>.Contains(T item)
		{
			bool? exists = ReadElementExistence(item);
			return !exists.HasValue ? glist.Contains(item) : exists.Value;
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			for (int i = arrayIndex; i < Count; i++)
			{
				array.SetValue(this[i], i);
			}
		}

		bool ICollection<T>.Remove(T item)
		{
			bool? exists = PutQueueEnabled ? ReadElementExistence(item) : null;
			if (!exists.HasValue)
			{
				Initialize(true);
				bool contained = glist.Remove(item);
				if (contained)
				{
					Dirty();
					return true;
				}
			}
			else if (exists.Value)
			{
				QueueOperation(new SimpleRemoveDelayedOperation(this, item));
				return true;
			}
			return false;
		}

		#endregion

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			Read();
			return glist.GetEnumerator();
		}

		#endregion
	}
}
