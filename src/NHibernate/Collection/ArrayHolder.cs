using System;
using System.Data;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection 
{
	/// <summary>
	/// A persistent wrapper for an array. lazy initialization is NOT supported
	/// </summary>
	public class ArrayHolder : PersistentCollection
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PersistentCollection));

		private object array;
		private System.Type elementClass;
		private IList tempList;
		private IList tempListIdentifier;

		public ArrayHolder(ISessionImplementor session, object array) : base(session) 
		{
			this.array = array;
			initialized = true;
		}

		protected override object Snapshot(CollectionPersister persister) 
		{
			int length = /*(array==null) ? temp.Count :*/ ((System.Array)array).Length;
			object result = System.Array.CreateInstance(persister.ElementClass, length);
			for (int i=0; i<length; i++) {
				object elt = /*(array==null) ? temp[i] :*/ ((System.Array)array).GetValue(i);
				try 
				{
					((System.Array)result).SetValue( persister.ElementType.DeepCopy(elt), i);
				} 
				catch (Exception e) 
				{
					log.Error("Array element type error", e);
					throw new HibernateException( "Array element type error", e);
				}
			}
			return result;
		}

		public ArrayHolder(ISessionImplementor session, CollectionPersister persister) : base(session) 
		{
			elementClass = persister.ElementClass;
		}

		public object Array 
		{
			get { return array; }
		}

		public override bool EqualsSnapshot(IType elementType) 
		{
			object snapshot = GetSnapshot();
			int xlen = ((System.Array)snapshot).Length;
			if ( xlen != ((System.Array)array).Length ) return false;
			for (int i=0; i<xlen; i++) 
			{
				if ( elementType.IsDirty( ((System.Array)snapshot).GetValue(i), ((System.Array)array).GetValue(i), session) ) return false;
			}
			return true;
		}

		public override ICollection Elements() 
		{
			if (array==null) return tempList;
			int length = ((System.Array)array).Length;
			IList list = new ArrayList(length);
			for (int i=0; i<length; i++) 
			{
				list.Add( ((System.Array)array).GetValue(i) );
			}
			return list;
		}

		public override bool Empty 
		{
			get { return false; }
		}

		public override void WriteTo(IDbCommand st, CollectionPersister persister, object entry, int i, bool writeOrder) 
		{
			persister.WriteElement(st, entry, writeOrder, session);
			persister.WriteIndex(st, i, writeOrder, session);
		}

		public override object ReadFrom(IDataReader rs, CollectionPersister persister, object owner) 
		{
			//object element = persister.ReadElement(rs, owner, session);
			object elementIdentifier = persister.ReadElementIdentifier(rs, owner, session);
			int index = (int) persister.ReadIndex(rs, session);
			for (int i=tempList.Count; i<=index; i++) 
			{
				tempList.Insert(i, null);
				tempListIdentifier.Insert(i , null);
			}
			//tempList[index] = element;
			tempListIdentifier[index] = elementIdentifier;
			return elementIdentifier;
		}

		public override ICollection Entries() 
		{
			return Elements();
		}

		public override void ReadEntries(ICollection entries) 
		{
			ArrayList list = new ArrayList();
			foreach(object obj in entries) 
			{
				list.Add(obj);
			}
			array = list.ToArray( elementClass );
		}

		public override void BeginRead() 
		{
			tempList = new ArrayList();
			tempListIdentifier = new ArrayList();
		}
		
		public override void EndRead() 
		{
			array = ((ArrayList)tempList).ToArray(elementClass);
			tempList = null;
		}

		public override void EndRead(CollectionPersister persister, object owner) 
		{
			
			for(int i = 0 ;i < tempListIdentifier.Count; i++) 
			{
				object element = persister.ElementType.ResolveIdentifier(tempListIdentifier[i], session, owner);
				tempList[i] = element;
			}

			array = ((ArrayList)tempList).ToArray(elementClass);
			//tempList = null;
			//tempListIdentifier = null;
		}


		public override object GetInitialValue(bool lazy) 
		{
			tempList = new ArrayList();
			base.GetInitialValue(false);
			array = System.Array.CreateInstance( elementClass, tempList.Count );
			for (int i=0; i<tempList.Count; i++) 
			{
				((System.Array)array).SetValue(tempList[i],i);
			}
			session.AddArrayHolder(this);
			tempList=null;
			return array;
		}

		public override void BeforeInitialize(CollectionPersister persister) 
		{
		}

		public override bool IsArrayHolder 
		{
			get { return true; }
		}

		public override bool IsDirectlyAccessible
		{
			get	{ return true;}
		}

		public ArrayHolder(ISessionImplementor session, CollectionPersister persister, object disassembled, object owner) : base(session) 
		{
			object[] cached = (object[]) disassembled;

			array = System.Array.CreateInstance( persister.ElementClass, cached.Length );

			for (int i=0; i<cached.Length; i++) 
			{
				((System.Array)array).SetValue( persister.ElementType.Assemble(cached[i], session, owner), i);
			}
		}

		public override object Disassemble(CollectionPersister persister) 
		{
			int length = tempList.Count;
			object[] result = new object[length];
			for (int i=0; i<length; i++) 
			{
				result[i] = persister.ElementType.Disassemble( tempList[i], session);
			}
			return result;
		}

		public override object GetCachedValue() 
		{
			session.AddArrayHolder(this);
			return array;
		}

		public override ICollection GetDeletes(IType elemType) 
		{
			IList deletes = new ArrayList();
			object sn = GetSnapshot();
			int snSize = ((System.Array)sn).Length;
			int arraySize = ((System.Array)array).Length;
			int end;
			if ( snSize > arraySize ) 
			{
				for (int i=arraySize; i<snSize; i++ ) deletes.Add( i );
				end = arraySize;
			} 
			else 
			{
				end = snSize;
			}
			for (int i=0; i<end; i++) 
			{
				if ( ((System.Array)array).GetValue(i)==null && ((System.Array)sn).GetValue(i)!=null )
					deletes.Add( i );
			}
			return deletes;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType) 
		{
			object sn = GetSnapshot();
			return ((System.Array)array).GetValue(i) != null && ( i >= ((System.Array)sn).Length || ((System.Array)sn).GetValue(i) == null );
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType) 
		{
			object sn = GetSnapshot();
			return i < ((System.Array)sn).Length &&
				((System.Array)sn).GetValue(i) != null &&
				((System.Array)array).GetValue(i) != null &&
				elemType.IsDirty( ((System.Array)array).GetValue(i), ((System.Array)sn).GetValue(i), session);
		}

		public override object GetIndex(object entry, int i) 
		{
			return i;
		}

		public override bool EntryExists(object entry, int i) 
		{
			return entry!=null;
		}
			

	}
}
