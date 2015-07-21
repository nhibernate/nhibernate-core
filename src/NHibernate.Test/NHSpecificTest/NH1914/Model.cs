using System;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using NHibernate.Classic;

namespace NHibernate.Test.NHSpecificTest.NH1914
{
	public class IDS
	{
		public String Identifier { get; set; }

		public String Name { get; set; }

		public IDictionary<String,ListOfHLUT> CRSPLUTs { get; set; }
	}

	public class ListOfHLUT : CustomList<HLUT>
	{
		public ListOfHLUT() : base() { }
		public ListOfHLUT(IEnumerable<HLUT> theValues) : base(theValues) { }
	}

	public class HLUT : LUT
	{
		public String Name { get; set; }
	}

	public class LUT
	{
		public long Identifier { get; set; }

		public IList<Entry> Entries { get; set; }
	}

	public struct Entry
	{
		public Entry(Double theKey, Double theValue)
		{
			Key1 = theKey;
			Value = theValue;
		}

		public Double Key1;

		public Double Value;
	}

	public class CustomList<T> : IList<T>, IList, ILifecycle
	{
		#region Constructors

		public CustomList()
		{
			myValues = new List<T>();
		}

		public CustomList(IEnumerable<T> theValues)
		{
			myValues = new List<T>(theValues);
		}
		#endregion

		#region Member Variables
		protected IList<T> myValues;
		#endregion

		#region NHibernate Members
		[XmlIgnore]
		public virtual String Identifier { get; set; }

		[XmlIgnore]
		public virtual long ID { get; set; }

		[XmlIgnore]
		public virtual IList<T> Values
		{
			get
			{
				return myValues;
			}
			set
			{
				myValues = value;
			}
		}
		#endregion

		#region ILifecycle Members

		public LifecycleVeto OnDelete(ISession s)
		{
			return LifecycleVeto.NoVeto;
		}

		public void OnLoad(ISession s, object id)
		{

		}

		public LifecycleVeto OnSave(ISession s)
		{
			return LifecycleVeto.NoVeto;
		}

		public LifecycleVeto OnUpdate(ISession s)
		{
			return LifecycleVeto.NoVeto;
		}

		#endregion

		#region IList<T> Members

		public int IndexOf(T item)
		{
			return myValues.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			myValues.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			myValues.RemoveAt(index);
		}

		public T this[int index]
		{
			get
			{
				return myValues[index];
			}
			set
			{
				myValues[index] = value;
			}
		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{
			myValues.Add(item);
		}

		public void Clear()
		{
			myValues.Clear();
		}

		public bool Contains(T item)
		{
			return myValues.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			myValues.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return myValues.Count; }
		}

		public bool IsReadOnly
		{
			get { return myValues.IsReadOnly; }
		}

		public bool Remove(T item)
		{
			return myValues.Remove(item);
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return myValues.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region IList Members

		public int Add(object value)
		{
			return ((IList)myValues).Add(value);
		}

		public bool Contains(object value)
		{
			return ((IList)myValues).Contains(value);
		}

		public int IndexOf(object value)
		{
			return ((IList)myValues).IndexOf(value);
		}

		public void Insert(int index, object value)
		{
			((IList)myValues).Insert(index, value);
		}

		public bool IsFixedSize
		{
			get { return ((IList)myValues).IsFixedSize; }
		}

		public void Remove(object value)
		{
			((IList)myValues).Remove(value);
		}

		object IList.this[int index]
		{
			get
			{
				return ((IList)myValues)[index];
			}
			set
			{
				((IList)myValues)[index] = value;
			}
		}

		#endregion

		#region ICollection Members

		public void CopyTo(Array array, int index)
		{
			((IList)myValues).CopyTo(array, index);
		}

		public bool IsSynchronized
		{
			get { return ((IList)myValues).IsSynchronized; }
		}

		public object SyncRoot
		{
			get { return ((IList)myValues).SyncRoot; }
		}

		#endregion
	}
}
