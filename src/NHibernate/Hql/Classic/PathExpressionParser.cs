using System;
using System.Collections;
using System.Text;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Hql.Classic
{
	/// <summary> 
	/// Parses an expression of the form foo.bar.baz and builds up an expression
	/// involving two less table joins than there are path components.
	/// </summary>
	public class PathExpressionParser : IParser
	{
		//TODO: this class does too many things! we need a different 
		//kind of path expression parser for each of the different 
		//ways in which path expressions can occur 

		//We should actually rework this class to not implement Parser 
		//and just process path expressions in the most convenient way.

		//The class is now way to complex!

		public const string EntityID = "id";

		public const string EntityClass = "class";

		private int dotcount;
		private string currentName;
		private string currentProperty;
		private string oneToOneOwnerName;
		private IAssociationType ownerAssociationType;
		private string[ ] columns;
		private string collectionName;
		private string collectionOwnerName;
		private string collectionRole;
		private readonly StringBuilder componentPath = new StringBuilder();
		//private string componentPath;
		private IType type;
		private readonly StringBuilder path = new StringBuilder();
		private bool ignoreInitialJoin;
		private bool continuation;
		private JoinType joinType = JoinType.InnerJoin; //default mode
		private bool useThetaStyleJoin = true;
		private IPropertyMapping currentPropertyMapping;
		private JoinSequence joinSequence;

		public JoinType JoinType
		{
			get { return joinType; }
			set { joinType = value; }
		}

		public bool UseThetaStyleJoin
		{
			get { return useThetaStyleJoin; }
			set { useThetaStyleJoin = value; }
		}

		private IPropertyMapping PropertyMapping
		{
			get { return currentPropertyMapping; }
		}
		
		private void AddJoin(string name, IAssociationType joinableType)
		{
			AddJoin(name, joinableType, CurrentColumns());
		}

		private void AddJoin(string name, IAssociationType joinableType, string[] foreignKeyColumns)
		{
			try
			{
				joinSequence.AddJoin(joinableType, name, joinType, foreignKeyColumns);
			}
			catch (MappingException me)
			{
				throw new QueryException(me);
			}
		}

		public string ContinueFromManyToMany( System.Type clazz, string[ ] joinColumns, QueryTranslator q )
		{
			Start( q );
			continuation = true;
			currentName = q.CreateNameFor( clazz );
			q.AddType( currentName, clazz );
			IQueryable classPersister = q.GetPersister( clazz );
			AddJoin( currentName, TypeFactory.ManyToOne(clazz), joinColumns );
			currentPropertyMapping = classPersister;
			return currentName;
		}

		public void IgnoreInitialJoin()
		{
			ignoreInitialJoin = true;
		}

		public void Token( string token, QueryTranslator q )
		{
			if( token != null )
			{
				path.Append(token);
			}

			string alias = q.GetPathAlias( path.ToString() );
			if( alias != null )
			{
				Reset( q ); //reset the dotcount (but not the path)
				currentName = alias; //after reset!
				currentPropertyMapping = q.GetPropertyMapping( currentName );
				if( !ignoreInitialJoin )
				{
					JoinSequence ojf = q.GetPathJoin( path.ToString() );
					try
					{
						joinSequence.AddCondition(ojf.ToJoinFragment(q.EnabledFilters, true).ToWhereFragmentString); //after reset!
					}
					catch (MappingException me)
					{
						throw new QueryException(me);
					}
					// we don't need to worry about any condition in the ON clause
					// here (toFromFragmentString), since anything in the ON condition 
					// is already applied to the whole query
				}
			}
			else if( ".".Equals( token ) )
			{
				dotcount++;
			}
			else
			{
				if( dotcount == 0 )
				{
					if( !continuation )
					{
						if( !q.IsName( token ) )
						{
							throw new QueryException( "undefined alias or unknown mapping: " + token );
						}
						currentName = token;
						currentPropertyMapping = q.GetPropertyMapping( currentName );
					}
				}
				else if( dotcount == 1 )
				{
					if( currentName != null )
					{
						currentProperty = token;
					}
					else if( collectionName != null )
					{
						//IQueryableCollection p = q.GetCollectionPersister( collectionRole );
						//DoCollectionProperty( token, p, collectionName );
						continuation = false;
					}
					else
					{
						throw new QueryException( "unexpected" );
					}
				}
				else
				{ // dotcount>=2

					// Do the corresponding RHS
					IType propertyType = PropertyType;

					if( propertyType == null )
					{
						throw new QueryException( "unresolved property: " + currentProperty );
					}

					if( propertyType.IsComponentType )
					{
						DereferenceComponent( token );
					}
					else if( propertyType.IsEntityType )
					{
						DereferenceEntity( token, (EntityType) propertyType, q );
					}
					else if( propertyType.IsCollectionType )
					{
						DereferenceCollection( token, ( (CollectionType) propertyType).Role, q );
					}
					else if( token != null )
					{
						throw new QueryException( "dereferenced: " + currentProperty );
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="propertyType"></param>
		/// <param name="q"></param>
		/// <remarks>NOTE: we avoid joining to the next table if the named property is just the foreign key value</remarks>
		private void DereferenceEntity( string propertyName, EntityType propertyType, QueryTranslator q )
		{
			//if its "id"
			bool isIdShortcut = EntityID.Equals(propertyName) && !propertyType.IsUniqueKeyReference;
			
			//or its the id property name
			string idPropertyName;
			try 
			{
				idPropertyName = propertyType.GetIdentifierOrUniqueKeyPropertyName( q.Factory );
			}
			catch (MappingException me) 
			{
				throw new QueryException(me);
			}
			bool isNamedIdPropertyShortcut = idPropertyName != null && 	idPropertyName.Equals( propertyName );
			
			if ( isIdShortcut || isNamedIdPropertyShortcut ) 
			{
				// special shortcut for id properties, skip the join!
				// this must only occur at the _end_ of a path expression
				DereferenceProperty( propertyName );
			}
			else 
			{
				System.Type entityClass = propertyType.AssociatedClass;
				String name = q.CreateNameFor( entityClass );
				q.AddType( name, entityClass );
				//String[] keyColNames = memberPersister.getIdentifierColumnNames();
				AddJoin( name, propertyType );
				if ( propertyType.IsOneToOne ) 
				{
					oneToOneOwnerName = currentName;
				}
				ownerAssociationType = propertyType;
				currentName = name;
				currentProperty = propertyName;
				q.AddPathAliasAndJoin( path.ToString( 0, path.ToString().LastIndexOf( StringHelper.Dot ) ), name, joinSequence.Copy() );
				componentPath.Length = 0;
				currentPropertyMapping = q.GetPersister( entityClass );
			}
		}

		private void DereferenceProperty( string propertyName )
		{
			if ( propertyName != null ) 
			{
				if ( componentPath != null && componentPath.Length > 0 ) 
				{
					componentPath.Append(StringHelper.Dot);
				}
				componentPath.Append(propertyName);
			}
		}

		private void DereferenceComponent( string propertyName )
		{
			DereferenceProperty( propertyName );
		}
	
		private void DereferenceCollection(String propertyName, String role, QueryTranslator q)
		{
			collectionRole = role;
			IQueryableCollection collPersister = q.GetCollectionPersister( role );
			string name = q.CreateNameForCollection(role);
			AddJoin(name, collPersister.CollectionType);

			//if ( collPersister.HasWhere ) 
			//{
			//    join.AddCondition( collPersister.GetSQLWhereString( name ) );
			//}
			
			collectionName = name;
			collectionOwnerName = currentName;
			currentName = name;
			currentProperty = propertyName;
			componentPath.Length = 0;
			//componentPath = new StringBuilder();
			currentPropertyMapping = new CollectionPropertyMapping( collPersister );
		}

		private string PropertyPath
		{
			get
			{
				if( currentProperty == null )
				{
					return EntityID;
				}
				else if ( componentPath != null && componentPath.Length > 0 ) 
				{
					return currentProperty + StringHelper.Dot + componentPath;
				}
				else
				{
					return currentProperty;
				}
			}
		}

		private void SetType( QueryTranslator q )
		{
			if( currentProperty == null )
			{
				type = PropertyMapping.Type;
			}
			else
			{
				type = PropertyType;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected IType PropertyType
		{
			get
			{
				string path = PropertyPath;
				IType type = PropertyMapping.ToType( PropertyPath );

				if( type == null )
				{
					throw new QueryException( "could not resolve property type: " + path );
				}

				return type;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected string[ ] CurrentColumns( )
		{
			string path = PropertyPath;
			string[ ] propertyColumns = PropertyMapping.ToColumns( currentName, path );
			if( propertyColumns == null )
			{
				throw new QueryException( "could not resolve property columns: " + path );
			}
			return propertyColumns;
		}

		private void Reset( QueryTranslator q )
		{
			//join = q.CreateJoinFragment( useThetaStyleJoin );
			dotcount = 0;
			currentName = null;
			currentProperty = null;
			collectionName = null;
			collectionRole = null;
			componentPath.Length = 0;
			type = null;
			collectionName = null;
			columns = null;
			expectingCollectionIndex = false;
			continuation = false;
			currentPropertyMapping = null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		public void Start( QueryTranslator q )
		{
			if( !continuation )
			{
				Reset( q );
				path.Length = 0;
				joinSequence = new JoinSequence(q.Factory).SetUseThetaStyle(useThetaStyleJoin);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		public virtual void End( QueryTranslator q )
		{
			ignoreInitialJoin = false;

			IType propertyType = PropertyType;
			if( propertyType != null && propertyType.IsCollectionType )
			{
				collectionRole = ( ( CollectionType ) propertyType ).Role;
				collectionName = q.CreateNameForCollection( collectionRole );
				PrepareForIndex( q );
			}
			else
			{
				columns = CurrentColumns( );
				SetType( q );
			}

			//important!!
			continuation = false;
		}

		private void PrepareForIndex( QueryTranslator q )
		{
			IQueryableCollection collPersister = q.GetCollectionPersister( collectionRole );

			if( !collPersister.HasIndex )
			{
				throw new QueryException( "unindexed collection before []" );
			}
			string[ ] indexCols = collPersister.IndexColumnNames;
			if( indexCols.Length != 1 )
			{
				throw new QueryException( "composite-index appears in []: " + path );
			}

			JoinSequence fromJoins = new JoinSequence(q.Factory)
				.SetUseThetaStyle(useThetaStyleJoin)
				.SetRoot(collPersister, collectionName)
				.SetNext(joinSequence.Copy());

			if( !continuation )
			{
				AddJoin( collectionName, collPersister.CollectionType );
			}
			joinSequence.AddCondition( new SqlString(collectionName + '.' + indexCols[0] + " = ") );

			CollectionElement elem = new CollectionElement();
			elem.ElementColumns = collPersister.GetElementColumnNames(collectionName);
			elem.Type = collPersister.ElementType;
			elem.IsOneToMany = collPersister.IsOneToMany;
			elem.Alias = collectionName;
			elem.JoinSequence = joinSequence;
			collectionElements.Add( elem ); //addlast
			SetExpectingCollectionIndex();

			q.AddCollection( collectionName, collectionRole );
			q.AddJoin( collectionName, fromJoins );
		}

		/// <summary></summary>
		public sealed class CollectionElement // struct?
		{
			/// <summary></summary>
			public IType Type;

			/// <summary></summary>
			public bool IsOneToMany;

			/// <summary></summary>
			public string Alias;

			/// <summary></summary>
			public string[ ] ElementColumns;

			/// <summary></summary>
			public JoinSequence JoinSequence;

			/// <summary></summary>
			public SqlStringBuilder IndexValue = new SqlStringBuilder();
		}

		private bool expectingCollectionIndex;
		private ArrayList collectionElements = new ArrayList();

		/// <summary></summary>
		public CollectionElement LastCollectionElement()
		{
			CollectionElement ce = ( CollectionElement ) collectionElements[ collectionElements.Count - 1 ];
			collectionElements.RemoveAt( collectionElements.Count - 1 );
			return ce; //remove last
		}

		/// <summary></summary>
		public void SetLastCollectionElementIndexValue(SqlString value)
		{
			( ( CollectionElement ) collectionElements[ collectionElements.Count - 1 ] ).IndexValue.Add( value ); //getlast
		}

		/// <summary></summary>
		public bool IsExpectingCollectionIndex
		{
			get { return expectingCollectionIndex; }
			set { expectingCollectionIndex = value; }
		}

		/// <summary></summary>
		protected virtual void SetExpectingCollectionIndex()
		{
			expectingCollectionIndex = true;
		}

		public JoinSequence WhereJoin
		{
			get { return joinSequence; }
		}

		/// <summary></summary>
		public string WhereColumn
		{
			get
			{
				if( columns.Length != 1 )
				{
					throw new QueryException( "path expression ends in a composite value" );
				}
				return columns[ 0 ];
			}
		}

		/// <summary></summary>
		public string[ ] WhereColumns
		{
			get { return columns; }
		}

		/// <summary></summary>
		public IType WhereColumnType
		{
			get { return type; }
		}

		/// <summary></summary>
		public string Name
		{
			get { return currentName == null ? collectionName : currentName; }
		}

		/// <summary></summary>
		public string GetCollectionSubquery(IDictionary enabledFilters)
		{
			return CollectionSubqueryFactory.CreateCollectionSubquery(
				joinSequence, enabledFilters, CurrentColumns());
		}

		/// <summary></summary>
		public bool IsCollectionValued
		{
			// TODO: Is there a better way
			get { return collectionName != null && !PropertyType.IsCollectionType; }
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		public void AddAssociation( QueryTranslator q )
		{
			q.AddJoin( Name, joinSequence );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		/// <returns></returns>
		public string AddFromAssociation( QueryTranslator q )
		{
			if ( IsCollectionValued )
			{
				return AddFromCollection( q );
			}
			else
			{
				q.AddFrom( currentName, joinSequence );
				return currentName;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		/// <returns></returns>
		public string AddFromCollection( QueryTranslator q )
		{
			IType collectionElementType = PropertyType;

			if( collectionElementType == null )
			{
				throw new QueryException( string.Format( "must specify 'elements' for collection valued property in from clause: {0}", path ) );
			}
			if( collectionElementType.IsEntityType )
			{
				// an association
				IQueryableCollection collectionPersister = q.GetCollectionPersister( collectionRole );
				IQueryable entityPersister = (IQueryable) collectionPersister.ElementPersister;
				System.Type clazz = entityPersister.MappedClass;

				string elementName;
				if ( collectionPersister.IsOneToMany )
				{
					elementName = collectionName;
					// allow index() function
					q.DecoratePropertyMapping( elementName, collectionPersister );
				}
				else
				{
					// many to many
					q.AddCollection( collectionName, collectionRole );
					elementName = q.CreateNameFor( clazz );
					AddJoin( elementName, ( IAssociationType ) collectionElementType );
				}
				q.AddFrom( elementName, clazz, joinSequence );
				currentPropertyMapping = new CollectionPropertyMapping( collectionPersister );
				return elementName;
			}
			else
			{
				// collection of values
				q.AddFromCollection( collectionName, collectionRole, joinSequence );
				return collectionName;
			}
		}

		/// <summary></summary>
		public string CollectionName
		{
			get { return collectionName; }
		}

		/// <summary></summary>
		public string CollectionRole
		{
			get { return collectionRole; }
		}

		/// <summary></summary>
		public String CollectionOwnerName
		{
			get { return collectionOwnerName; }
		}

		/// <summary></summary>
		public string CurrentName
		{
			get { return currentName; }
		}

		/// <summary></summary>
		public string CurrentProperty
		{
			get { return currentProperty; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		/// <param name="entityName"></param>
		public void Fetch( QueryTranslator q, string entityName )
		{
			if ( IsCollectionValued )
			{
				q.SetCollectionToFetch( CollectionRole, CollectionName, CollectionOwnerName, entityName );
			}
			else
			{
				q.AddEntityToFetch( entityName, oneToOneOwnerName, ownerAssociationType );
			}
		}
	}
}