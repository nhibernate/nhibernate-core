using System;

using NCollection = NHibernate.Collection;
using NHibernate.Type;

namespace NHibernate.Mapping {

	/// <summary>
	/// A Set with no nullable element columns will have a primary
	/// key consisting of all table columns (ie - key columns + 
	/// element columns).
	/// </summary>
	public class Set : Collection {
		public Set(PersistentClass owner) : base(owner)	{
		}

		/// <summary>
		/// <see cref="Collection.IsSet"/>
		/// </summary>
		public override bool IsSet {
			get {return true;	}
		}

		/// <summary>
		/// <see cref="Collection.Type"/>
		/// </summary>
		public override PersistentCollectionType Type {
			get {
				//TODO: modify when added implementation of sorted set
				return IsSorted ?
					TypeFactory.SortedSet(Role, Comparer) :
					TypeFactory.Set(Role);
			}
		}

		/// <summary>
		/// <see cref="Collection.WrapperClass"/>
		/// </summary>
		public override System.Type WrapperClass {
			get {
				return IsSorted ? 
					typeof(NCollection.SortedSet) :
					typeof(NCollection.Set);
			}
		}


		public void CreatePrimaryKey() {
			PrimaryKey pk = new PrimaryKey();
			foreach(Column col in Key.ColumnCollection) {
				pk.AddColumn(col);
			}

			bool nullable = false;
			foreach(Column col in Element.ColumnCollection) {
				if(col.IsNullable) nullable = true;
				pk.AddColumn(col);
			}

			// some databases (Postgres) will tolerate nullable
			// column in a primary key - others (DB2) won't
			if(!nullable) Table.PrimaryKey = pk;

		}


	}
}
