using System;
using System.Collections;
using System.Data;

using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection
{
	/// <summary>
	/// An <c>IdentiferBag</c> implements "bag" semantics more efficiently than
	/// a regular <see cref="Bag" /> by adding a synthetic identifier column to the
	/// table.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The identifier is unique for all rows in the table, allowing very efficient
	/// updates and deletes.  The value of the identifier is never exposed to the 
	/// application. 
	/// </para>
	/// <para>
	/// <c>IdentifierBag</c>s may not be used for a many-to-one association.  Furthermore,
	/// there is no reason to use <c>inverse="true"</c>.
	/// </para>
	/// </remarks>
	public class IdentifierBag : ODMGCollection, IList
	{
		private IList values;
		private IList valuesIdentifiers;

		private IDictionary identifiers; //element -> id

		public IdentifierBag(ISessionImplementor session) : base(session)
		{
		}

		public IdentifierBag(ISessionImplementor session, ICollection coll) : base(session)
		{
			if(coll is IList) 
			{
				values = (IList)coll;
			}
			else 
			{
				values = new ArrayList();
				foreach(object obj in coll) 
				{
					values.Add(obj);
				}
			}

			initialized = true;
			directlyAccessible = true;
			identifiers = new Hashtable();
		}

		public IdentifierBag(ISessionImplementor session, CollectionPersister persister, object disassembled, object owner) : base(session)
		{
			BeforeInitialize(persister);
			object[] array = (object[])disassembled;
			
			for(int i=0; i < array.Length; i+=2) 
			{
				object obj = persister.ElementType.Assemble( array[i+1], session, owner );
				identifiers[obj] = persister.IdentifierType.Assemble( array[i], session, owner );
				values.Add(obj);
			}

			initialized = true;
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
				return values[index];
			}
			set
			{
				Write();
				values[index] = value; 
			}
		}

		public void RemoveAt(int index)
		{
			Write();
			values.RemoveAt(index);
		}

		public void Insert(int index, object value)
		{
			Write();
			values.Insert(index, value);
		}

		public void Remove(object value)
		{
			Write();
			values.Remove(value);
		}

		public bool Contains(object value)
		{
			Read();
			return values.Contains(value);
		}

		public void Clear()
		{
			Write();
			values.Clear();
			identifiers.Clear();
		}

		public int IndexOf(object value)
		{
			Read();
			return values.IndexOf(value);
		}

		public int Add(object value)
		{
			Write();
			return values.Add(value);
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
				return values.Count;
			}
		}

		public override void CopyTo(Array array, int index)
		{
			Read();
			values.CopyTo(array, index);
		}

		public override object SyncRoot
		{
			get { return values.SyncRoot; }
		}

		#endregion

		#region IEnumerable Members

		public override IEnumerator GetEnumerator()
		{
			return values.GetEnumerator();
		}

		#endregion
	
		public override void BeforeInitialize(CollectionPersister persister)
		{
			identifiers = new Hashtable();
			values = new ArrayList();
			valuesIdentifiers = new ArrayList();
		}

		public override object Disassemble(CollectionPersister persister)
		{
			object[] result = new object[values.Count * 2];

			int i = 0;
			foreach(object obj in values) 
			{
				result[i++] = persister.IdentifierType.Disassemble( identifiers[obj], session );
				result[i++] = persister.ElementType.Disassemble( obj, session );
			}

			return result;
		}

		public override ICollection Elements()
		{
			return values;
		}

		public override bool Empty
		{
			get
			{
				return (values.Count == 0);
			}
		}

		public override void EndRead(CollectionPersister persister, object owner) 
		{
			for(int i = 0 ;i < valuesIdentifiers.Count; i++) 
			{
				object element = persister.ElementType.ResolveIdentifier(valuesIdentifiers[i], session, owner);
				values[i] = element;
			}
		}

		[Obsolete("See PersistentCollection.ReadEntries for reason")]
		public override void ReadEntries(ICollection entries) 
		{
			throw new NotSupportedException("Should not call...");
		}

		public override ICollection Entries()
		{
			return values;
		}

		public override bool EntryExists(object entry, int i)
		{
			return entry!=null;
		}

		public override bool EqualsSnapshot(IType elementType)
		{
			IDictionary snap = (IDictionary)GetSnapshot();
			if(snap.Count!=values.Count) return false;

			int i = 0;
			foreach(object obj in values) 
			{
				object id = identifiers[i++];
				if(id==null) return false;

				object old = snap[id];
				if( elementType.IsDirty( old, obj, session ) ) return false;
			}

			return true;

		}


		public override ICollection GetDeletes(IType elemType)
		{
			IDictionary snap = (IDictionary) GetSnapshot();
			IList deletes = new ArrayList( snap.Keys );

			int i = 0;
			foreach(object obj in values) 
			{
				if( obj!=null ) deletes.Remove( identifiers[i++] );
			}

			return deletes;
		}

		public override object GetIndex(object entry, int i)
		{
			return new NotImplementedException("Bags don't have indexes");
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			IDictionary snap = (IDictionary)GetSnapshot();
			object id = identifiers[i];
			
			return entry!=null && ( id==null || snap[id]==null );
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType)
		{
			if(entry==null) return false;
			IDictionary snap = (IDictionary)GetSnapshot();

			object id = identifiers[i];
			if(id==null) return false;

			object old = snap[id];
			return entry!=null && old!=null && elemType.IsDirty(old, entry, session);
		}

		public override object ReadFrom(IDataReader reader, CollectionPersister persister, object owner)
		{
			object elementIdentifier = persister.ReadElementIdentifier(reader, owner, session);
			values.Add(null);
			valuesIdentifiers.Add(elementIdentifier);

			identifiers[values.Count - 1] = persister.ReadIdentifier(reader, session);

			return elementIdentifier;
		}

		protected override object Snapshot(CollectionPersister persister)
		{
			IDictionary map = new Hashtable(values.Count);
			
			int i = 0;
			foreach(object obj in values) 
			{
				object key = identifiers[i++];
				map[key] = persister.ElementType.DeepCopy(obj);
			}

			return map;
		}

		public override ICollection GetOrphans(object snapshot)
		{
			IDictionary sn = (IDictionary)GetSnapshot();
			ArrayList result = new ArrayList();
			result.AddRange(sn.Values);
			PersistentCollection.IdentityRemoveAll(result, values, session);
			return result;
		}

		public override void PreInsert(CollectionPersister persister, object entry, int i)
		{
			try 
			{
				object id = persister.IdentifierGenerator.Generate(session, entry);
				// TODO: native ids
				identifiers[i] = id;
			}
			catch (Exception sqle) 
			{
				throw new ADOException("Could not generate collection row id.", sqle);
			}
		}

		public override void WriteTo(IDbCommand st, CollectionPersister persister, object entry, int i, bool writeOrder)
		{
			persister.WriteElement(st, entry, writeOrder, session);
			persister.WriteIdentifier(st, identifiers[i], writeOrder, session);
		}



		
	}
}
