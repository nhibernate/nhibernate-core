using System.Collections;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public abstract class Collection : IFetchable, IValue
	{
		private static readonly ICollection EmptyColumns = new ArrayList();

		/// <summary></summary>
		public const string DefaultElementColumnName = "elt";

		/// <summary></summary>
		public const string DefaultKeyColumnName = "id";

		private SimpleValue key;
		private IValue element;
		private Table collectionTable;
		private string role;
		private bool lazy;
		//private bool isOneToMany;
		private bool inverse;
		//private OneToMany oneToMany;
		private ICacheConcurrencyStrategy cache;
		private string orderBy;
		private string where;
		private PersistentClass owner;
		private bool sorted;
		private IComparer comparer;
		private bool orphanDelete;
		private int batchSize = 1;
		private OuterJoinFetchStrategy joinedFetch;
		private System.Type collectionPersisterClass;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		protected Collection( PersistentClass owner )
		{
			this.owner = owner;
		}

		/// <summary></summary>
		public int ColumnSpan
		{
			get { return 0; }
		}

		/// <summary></summary>
		public virtual bool IsSet
		{
			get { return false; }
		}

		/// <summary></summary>
		public SimpleValue Key
		{
			get { return key; }
			set { key = value; }
		}

		/// <summary></summary>
		public IValue Element
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
		public Table CollectionTable
		{
			get { return collectionTable; }
			set { collectionTable = value; }
		}

		/// <summary></summary>
		public Table Table
		{
			get { return Owner.Table; }
		}

		/// <summary></summary>
		public bool IsSorted
		{
			get { return sorted; }
			set { sorted = value; }
		}

		/// <summary></summary>
		public PersistentClass Owner
		{
			get { return owner; }
			set { owner = value; }
		}

		/// <summary></summary>
		public System.Type CollectionPersisterClass
		{
			get { return collectionPersisterClass; }
			set { collectionPersisterClass = value; }
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
		public ICollection ColumnCollection
		{
			get { return EmptyColumns; }
		}

		/// <summary></summary>
		public Formula Formula
		{
			get { return null; }
		}

		/// <summary></summary>
		public bool IsNullable
		{
			get { return true; }
		}

		/// <summary></summary>
		public bool IsUnique
		{
			get { return false; }
		}

		/// <summary></summary>
		public abstract PersistentCollectionType CollectionType { get; }
		
		/// <summary></summary>
		public IType Type
		{
			get { return CollectionType; }
		}

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
			get { return element is OneToMany; }
			//set { isOneToMany = value; }
		}

		/*
		/// <summary></summary>
		public OneToMany OneToMany
		{
			get { return oneToMany; }
			set { oneToMany = value; }
		}
		*/

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
			get { return owner.MappedClass; }
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

		/// <summary></summary>
		public int BatchSize
		{
			get { return batchSize; }
			set { batchSize = value; }
		}

		/// <summary></summary>
		public OuterJoinFetchStrategy OuterJoinFetchSetting
		{
			get { return joinedFetch; }
			set { joinedFetch = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public void CreateForeignKey( )
		{
		}

		private void CreateForeignKeys( )
		{
			if ( !IsInverse )
			{
				Element.CreateForeignKey();
				Key.CreateForeignKeyOfClass( Owner.MappedClass );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract void CreatePrimaryKey( );

		/// <summary>
		/// 
		/// </summary>
		public void CreateAllKeys()
		{
			CreateForeignKeys();
			if ( !IsInverse )
			{
				CreatePrimaryKey( );
			}
		}

		/// <summary></summary>
		public bool IsSimpleValue
		{
			get { return false; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		/// <returns></returns>
		public bool IsValid( IMapping mapping )
		{
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		public virtual void Validate( IMapping mapping )
		{
			if ( !Key.IsValid( mapping ) )
			{
				throw new MappingException( string.Format( "collection foreign key mapping has wrong number of columns: {0} type: {1}", Role, Key.Type.Name ) );
			}

			if ( !Element.IsValid( mapping ) )
			{
				throw new MappingException( string.Format( "collection element key mapping has wrong number of columns: {0} type: {1}", Role, Element.Type.Name ) );
			}
		}
	}
}