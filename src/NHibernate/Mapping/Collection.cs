using System;
using System.Collections;

using NHibernate.Cache;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping 
{

	public abstract class Collection 
	{
		public const string DefaultElementColumnName = "elt";
		public const string DefaultKeyColumnName = "id";

		private Value key;
		private Value element;
		private Table table;
		private string role;
		private bool lazy;
		private bool isOneToMany;
		private bool inverse;
		private OneToMany oneToMany;
		private ICacheConcurrencyStrategy cache;
		private string orderBy;
		private string where;
		private PersistentClass owner;
		private bool sorted;
		private IComparer comparer;
		private bool orphanDelete;

		protected Collection(PersistentClass owner) 
		{
			this.owner = owner;
		}

		public virtual bool IsSet 
		{
			get { return false; }
		}

		public Value Key 
		{
			get { return key; }
			set { key = value; }
		}

		public Value Element 
		{
			get { return element; }
			set { element = value; }
		}

		public virtual bool IsIndexed 
		{
			get { return false; }
		}

		public Table Table 
		{
			get { return table; }
			set { table = value; }
		}

		public bool IsSorted 
		{
			get { return sorted; }
			set { sorted = value; }
		}

		public IComparer Comparer 
		{
			get { return comparer; }
			set { comparer = value; }
		}

		public bool IsLazy 
		{
			get { return lazy; }
			set { lazy = value; }
		}

		public string Role 
		{
			get { return role; }
			set { role = value; }
		}

		public abstract PersistentCollectionType Type { get; }
		public abstract System.Type WrapperClass { get; }

		public virtual bool IsPrimitiveArray 
		{
			get { return false; }
		}

		public virtual bool IsArray 
		{
			get { return false; }
		}

		public virtual bool IsIdentified 
		{
			get { return false; }
		}

		public bool IsOneToMany 
		{
			get { return isOneToMany; }
			set { isOneToMany = value; }
		}

		public OneToMany OneToMany 
		{
			get { return oneToMany; }
			set { oneToMany = value; }
		}

		//TODO: H2.0.3 - not in this class - where did it move to???
		public Index CreateIndex() {
			string name = "IX" + table.UniqueColumnString( Key.ColumnCollection );
			Index index = table.GetIndex(name);
			foreach(Column col in Key.ColumnCollection ) {
				index.AddColumn(col);
			}
			return index;
		}

		public ICacheConcurrencyStrategy Cache {
			get { return cache; }
			set { cache = value; }
		}

		public bool IsInverse 
		{
			get { return inverse; }
			set { inverse = value; }
		}

		public System.Type OwnerClass {
			get { return owner.PersistentClazz; }
		}

		public PersistentClass Owner 
		{
			get { return owner; }
			set { owner = value; }
		}

		public string OrderBy 
		{
			get { return orderBy; }
			set { orderBy = value; }
		}

		public string Where 
		{
			get { return where; }
			set { where = value; }
		}

		public bool OrphanDelete 
		{
			get { return orphanDelete; }
			set { orphanDelete = value; }
		}

	}
}
