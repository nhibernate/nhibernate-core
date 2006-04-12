using System;
using System.Collections;

using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Base class that stores the mapping information for <c>&lt;array&gt;</c>, <c>&lt;bag&gt;</c>, 
	/// <c>&lt;id-bag&gt;</c>, <c>&lt;list&gt;</c>, <c>&lt;map&gt;</c>, and <c>&lt;set&gt;</c>
	/// collections.
	/// </summary>
	/// <remarks> 
	/// Subclasses are responsible for the specialization required for the particular
	/// collection style.
	/// </remarks>
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
		private bool inverse;
		private ICacheConcurrencyStrategy cache;
		private string orderBy;
		private string where;
		private PersistentClass owner;
		private bool sorted;
		private object comparer;
		private bool orphanDelete;
		private int batchSize = 1;
		private FetchMode fetchMode;
		private System.Type collectionPersisterClass;
		private string referencedPropertyName;

#if NET_2_0
		private bool isGeneric;
		private System.Type[] genericArguments;
#endif

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

		// The type of this property is object, so as to accomodate
		// both IComparer and IComparer<T>.
		public object Comparer
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
		public FetchMode FetchMode
		{
			get { return fetchMode; }
			set { fetchMode = value; }
		}

#if NET_2_0
		/// <summary>
		/// Gets or sets a <see cref="Boolean"/> indicating if this is a 
		/// mapping for a generic collection.
		/// </summary>
		/// <value>
		/// <c>true</c> if a collection from the System.Collections.Generic namespace
		/// should be used, <c>false</c> if a collection from the System.Collections 
		/// namespace should be used.
		/// </value>
		/// <remarks>
		/// This has no affect on any versions of the .net framework before .net-2.0.
		/// </remarks>
		public bool IsGeneric
		{
			get { return isGeneric; }
			set { isGeneric = value; }
		}

		/// <summary>
		/// Gets or sets a <see cref="System.Type[]"/> that contains the arguments needed
		/// to construct an instance of a closed type.
		/// </summary>
		public System.Type[] GenericArguments
		{
			get { return genericArguments; }
			set { genericArguments = value; }
		}
#endif

		/// <summary>
		/// 
		/// </summary>
		public void CreateForeignKey( )
		{
		}

		private void CreateForeignKeys( )
		{
			// for inverse collections, let the "other end" hanlde it
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
		public virtual void CreateAllKeys()
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

		public string ReferencedPropertyName
		{
			get { return referencedPropertyName; }
			set { referencedPropertyName = value == null ? null : string.Intern( value ); }
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

		public bool[] ColumnInsertability
		{
			get { return ArrayHelper.EmptyBoolArray; }
		}

		public bool[] ColumnUpdateability
		{
			get { return ArrayHelper.EmptyBoolArray; }
		}
	}
}