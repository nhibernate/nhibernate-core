using System;
using System.Collections;
using System.Data;

using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection
{
	/// <summary>
	/// An unordered, unkeyed collection that can contain the same element
	/// multiple times. The .net collections API, has no <tt>Bag</tt>.
	/// Most developers seem to use <tt>IList</tt>s to represent bag semantics,
	/// so NHibernate follows this practice.
	/// </summary>
	[Serializable]
	public class Bag : ODMGCollection, IList
	{
		private IList bag;
		// used to hold the Identifiers of the Elements that will later
		// be moved to the bag field.
		private IList bagIdentifiers;

		public Bag(ISessionImplementor session) : base(session)
		{
		}

		public Bag(ISessionImplementor session, ICollection coll) : base(session) 
		{
			bag = coll as IList;
			
			if(bag == null) 
			{
				bag = new ArrayList();
				((ArrayList)bag).AddRange(coll);
			}

			initialized = true;
			directlyAccessible = true;
		}

		public Bag(ISessionImplementor session, CollectionPersister persister, object disassembled, object owner) : base(session)
		{
			BeforeInitialize(persister);
			object[] array = (object[])disassembled;
			for(int i=0; i<array.Length; i++) 
			{
				bag.Add(persister.ElementType.Assemble(array[i], session, owner));
			}
			initialized = true;
		}

		public override ICollection Elements()
		{
			return bag;
		}

		public override bool Empty
		{
			get{ return bag.Count==0;}
		}

		public override ICollection Entries()
		{
			return bag;
		}

		public override void EndRead(CollectionPersister persister, object owner)
		{
			for(int i = 0 ;i < bagIdentifiers.Count; i++) 
			{
				object element = persister.ElementType.ResolveIdentifier(bagIdentifiers[i], session, owner);
				bag[i] = element;
			}
		}

		public override object ReadFrom(IDataReader reader, CollectionPersister persister, object owner)
		{
			object elementIdentifier = persister.ReadElementIdentifier(reader, owner, session);
			int index = bag.Add(null);
			bagIdentifiers.Insert(index, elementIdentifier);
			
			return elementIdentifier;
		}

		[Obsolete("See PersistentCollection.ReadEntries for reason")]
		public override void ReadEntries(ICollection entries) 
		{
			foreach(object obj in entries) 
			{
				bag.Add(obj);
			}
		}

		public override void WriteTo(IDbCommand st, CollectionPersister persister, object entry, int i, bool writeOrder)
		{
			persister.WriteElement(st, entry, writeOrder, session);
		}

		public override void BeforeInitialize(CollectionPersister persister)
		{
			this.bag = new ArrayList();
			this.bagIdentifiers = new ArrayList();
		}

		public override bool EqualsSnapshot(IType elementType)
		{
			IList sn = (IList)GetSnapshot();
			if(sn.Count!=bag.Count) return false;

			foreach(object elt in bag) 
			{
				if ( CountOccurrences(elt, bag, elementType)!=CountOccurrences(elt, sn, elementType) ) return false;
			}
			
			return true;
		}

		private int CountOccurrences(object element, IList list, IType elementType) 
		{
			int result = 0;
			foreach(object obj in list) 
			{
				if ( elementType.Equals(element, obj) ) result++;
			}

			return result;
		}

		protected override object Snapshot(CollectionPersister persister)
		{
			ArrayList clonedList = new ArrayList( bag.Count );
			foreach(object obj in bag) 
			{
				clonedList.Add( persister.ElementType.DeepCopy(obj) );
			}
			return clonedList;
		}

		public override ICollection GetOrphans(object snapshot)
		{
			IList sn = (IList)snapshot;
			ArrayList result = new ArrayList();
			result.AddRange(sn);
			PersistentCollection.IdentityRemoveAll(result, bag, session);
			return result;
		}

		public override object Disassemble(CollectionPersister persister)
		{
			int length = bag.Count;
			object[] result = new object[length];

			for(int i = 0; i<length; i++) 
			{
				result[i] = persister.ElementType.Disassemble(bag[i], session);
			}

			return result;
		}

		public override bool NeedsRecreate(CollectionPersister persister)
		{
			return !persister.IsOneToMany;
		}


		// For a one-to-many, a <bag> is not really a bag;
		// it is *really* a set, since it can't contain the
		// same element twice. It could be considered a bug
		// in the mapping dtd that <bag> allows <one-to-many>.
		
		// Anyway, here we implement <set> semantics for a
		// <one-to-many> <bag>!
		
		public override ICollection GetDeletes(IType elemType)
		{
			ArrayList deletes = new ArrayList();
			IList sn = (IList)GetSnapshot();

			int i = 0;
			
			foreach(object oldObject in sn) 
			{
				bool found = false;
				if(bag.Count>i && elemType.Equals(oldObject, bag[i++])) 
				{
					//a shortcut if its location didn't change!
					found = true;
				}
				else 
				{
					//search for it
					foreach(object newObject in bag) 
					{
						if( elemType.Equals(oldObject, newObject) )
						{
							found = true;
							break;
						}
					}
				}
				if(!found) deletes.Add(oldObject);
			}

			return deletes;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			IList sn = (IList)GetSnapshot();
			if( sn.Count>i && elemType.Equals(sn[i], entry) )
			{
				// a shortcut if its location didn't change
				return false;
			}
			else 
			{
				//search for it
				foreach( object oldObject in sn ) 
				{
					if( elemType.Equals(oldObject, entry) ) return false;
				}
				return true;
			}
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType)
		{
			return false;
		}


		#region IList Members

		public bool IsReadOnly
		{
			get { return false; }
		}

		public object this[int index]
		{
			get 
			{ 
				Read();
				return bag[index];
			}
			set
			{
				Write();
				bag[index] = value;
			}
		}

		public void RemoveAt(int index)
		{
			Write();
			bag.RemoveAt(index);
		}

		public void Insert(int index, object value)
		{
			Write();
			bag.Insert(index, value);
		}

		public void Remove(object value)
		{
			Write();
			bag.Remove(value);
		}

		public bool Contains(object value)
		{
			Read();
			return bag.Contains(value);
		}

		public void Clear()
		{
			Write();
			bag.Clear();
		}

		public int IndexOf(object value)
		{
			Read();
			return bag.IndexOf(value);
		}

		public int Add(object value)
		{
			if ( !QueueAdd(value) ) 
			{
				Write();
				return bag.Add(value);
			}
			else 
			{
				//TODO: take a look at this - I don't like it because it changes the 
				// meaning of Add - instead of returning the index it was added at 
				// returns a "fake" index - not consistent with IList interface...
				return -1;
			}
		}

		public bool IsFixedSize
		{
			get { return false; }
		}

		#endregion

		#region ICollection Members

		public override bool IsSynchronized
		{
			get { return false; }
		}

		public override int Count
		{
			get
			{
				Read();
				return bag.Count;
			}
		}

		public override void CopyTo(Array array, int index)
		{
			Read();
			bag.CopyTo(array, index);
		}

		public override object SyncRoot
		{
			get { return this;	}
		}

		#endregion

		#region IEnumerable Members

		public override IEnumerator GetEnumerator()
		{
			Read();
			//TODO: H2.0.3 has an IteratorProxy - do we need??
			return bag.GetEnumerator();
		}

		#endregion
	
		public override void DelayedAddAll(ICollection coll)
		{
			foreach(object obj in coll) 
			{
				bag.Add(obj);
			}
		}

		public override object GetIndex(object entry, int i)
		{
			throw new NotSupportedException("Bags don't have indexes");
		}

		public override bool EntryExists(object entry, int i)
		{
			return entry!=null;
		}


	}
}
