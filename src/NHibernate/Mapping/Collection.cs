using System.Collections;
using NHibernate.Cache;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public abstract class Collection
	{
		/// <summary></summary>
		public const string DefaultElementColumnName = "elt";

		/// <summary></summary>
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		protected Collection( PersistentClass owner )
		{
			this.owner = owner;
		}

		/// <summary></summary>
		public virtual bool IsSet
		{
			get { return false; }
		}

		/// <summary></summary>
		public Value Key
		{
			get { return key; }
			set { key = value; }
		}

		/// <summary></summary>
		public Value Element
		{
			get { return element; }
			set { element = value; }
		}

		/// <summary></summary>
		public virtual bool IsIndexed
		{
			get { return false; }
		}

		/// <summary></summary>
		public Table Table
		{
			get { return table; }
			set { table = value; }
		}

		/// <summary></summary>
		public bool IsSorted
		{
			get { return sorted; }
			set { sorted = value; }
		}

		/// <summary></summary>
		public IComparer Comparer
		{
			get { return comparer; }
			set { comparer = value; }
		}

		/// <summary></summary>
		public bool IsLazy
		{
			get { return lazy; }
			set { lazy = value; }
		}

		/// <summary></summary>
		public string Role
		{
			get { return role; }
			set { role = value; }
		}

		/// <summary></summary>
		public abstract PersistentCollectionType Type { get; }
		
		/// <summary></summary>
		public abstract System.Type WrapperClass { get; }

		/// <summary></summary>
		public virtual bool IsPrimitiveArray
		{
			get { return false; }
		}

		/// <summary></summary>
		public virtual bool IsArray
		{
			get { return false; }
		}

		/// <summary></summary>
		public virtual bool IsIdentified
		{
			get { return false; }
		}

		/// <summary></summary>
		public bool IsOneToMany
		{
			get { return isOneToMany; }
			set { isOneToMany = value; }
		}

		/// <summary></summary>
		public OneToMany OneToMany
		{
			get { return oneToMany; }
			set { oneToMany = value; }
		}

		/// <summary></summary>
		public ICacheConcurrencyStrategy Cache
		{
			get { return cache; }
			set { cache = value; }
		}

		/// <summary></summary>
		public bool IsInverse
		{
			get { return inverse; }
			set { inverse = value; }
		}

		/// <summary></summary>
		public System.Type OwnerClass
		{
			get { return owner.PersistentClazz; }
		}

		/// <summary></summary>
		public PersistentClass Owner
		{
			get { return owner; }
			set { owner = value; }
		}

		/// <summary></summary>
		public string OrderBy
		{
			get { return orderBy; }
			set { orderBy = value; }
		}

		/// <summary></summary>
		public string Where
		{
			get { return where; }
			set { where = value; }
		}

		/// <summary></summary>
		public bool OrphanDelete
		{
			get { return orphanDelete; }
			set { orphanDelete = value; }
		}

	}
}