using System;
using System.Collections;
using System.Text;
using Iesi.Collections;
using NHibernate.Collection;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader
{
	/// <summary></summary>
	public enum OuterJoinFetchStrategy
	{
		/// <summary></summary>
		Lazy = -1,
		/// <summary></summary>
		Auto = 0,
		/// <summary></summary>
		Eager = 1
	}

	/// <summary></summary>
	public class OuterJoinLoader : Loader
	{
		/// <summary></summary>
		protected static readonly IType[ ] NoTypes = new IType[0];

		/// <summary></summary>
		protected static readonly string[ ][ ] NoStringArrays = new string[0][ ];

		/// <summary></summary>
		protected static readonly string[ ] NoStrings = new string[0];

		/// <summary></summary>
		protected static readonly ILoadable[ ] NoPersisters = new ILoadable[0];

		/// <summary></summary>
		protected ILoadable[ ] classPersisters;

		private LockMode[ ] lockModeArray;
		private int[] owners;
		private SqlString sqlString;
		private string[ ] suffixes;
		//private Dialect.Dialect dialect;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		public OuterJoinLoader( Dialect.Dialect dialect ) : base( dialect )
		{
			// The java version has Dialect here, but we have it at Loader
			//this.dialect = dialect;
		}

		/// <summary>
		/// Override on subclasses to enable or suppress joining of some associations
		/// </summary>
		/// <param name="type"></param>
		/// <param name="mappingDefault"></param>
		/// <param name="path"></param>
		/// <param name="table"></param>
		/// <param name="foreignKeyColumns"></param>
		/// <returns></returns>
		protected virtual bool IsJoinedFetchEnabled( IType type, bool mappingDefault, string path, string table, string[] foreignKeyColumns )
		{
			return type.IsEntityType && mappingDefault;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="config"></param>
		/// <param name="path"></param>
		/// <param name="table"></param>
		/// <param name="foreignKeyColumns"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		protected virtual JoinType GetJoinType( IAssociationType type, OuterJoinFetchStrategy config, string path, string table, string[] foreignKeyColumns, ISessionFactoryImplementor factory )
		{
			bool mappingDefault = IsJoinedFetchEnabledByDefault( config, type, factory );
			return IsJoinedFetchEnabled( type, mappingDefault, path, table, foreignKeyColumns) ? JoinType.LeftOuterJoin : JoinType.None;
		}

		/// <summary></summary>
		public sealed class OuterJoinableAssociation // struct?
		{
			/// <summary></summary>
			public IJoinable Joinable;
			/// <summary></summary>
			public string[ ] ForeignKeyColumns;
			/// <summary></summary>
			public string Subalias;
			/// <summary></summary>
			public string[] PrimaryKeyColumns;
			/// <summary></summary>
			public string TableName;
			/// <summary></summary>
			public int Owner;
			/// <summary></summary>
			public JoinType JoinType;
			/// <summary></summary>
			public bool IsOneToOne;
		}

		/// <summary>
		/// For an entity class, return a list of associations to be fetched by outerjion
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="alias"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public IList WalkTree( IOuterJoinLoadable persister, string alias, ISessionFactoryImplementor factory )
		{
			IList associations = new ArrayList();
			WalkClassTree( persister, alias, associations, new HashedSet(), String.Empty, 0, factory );
			return associations;
		}

		/// <summary>
		/// For a collection role, return a list of associations to be fetched by outerjoin
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="alias"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		protected IList WalkCollectionTree( IQueryableCollection persister, string alias, ISessionFactoryImplementor factory )
		{
			//TODO: when this is the entry point, we should use an INNER_JOIN for fetching the many-to-many elements!
			return WalkCollectionTree( persister, alias, new ArrayList(), new HashedSet(), string.Empty, 0, factory );
		}

		/// <summary>
		/// For a collection role, return a list of associations to be fetched by outerjoin
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="alias"></param>
		/// <param name="associations"></param>
		/// <param name="visitedPersisters"></param>
		/// <param name="path"></param>
		/// <param name="currentDepth"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		protected IList WalkCollectionTree( IQueryableCollection persister, string alias, IList associations, ISet visitedPersisters, string path, int currentDepth, ISessionFactoryImplementor factory )
		{
			if ( persister.IsOneToMany )
			{
				WalkClassTree(
					(IOuterJoinLoadable) persister.ElementPersister,
					alias,
					associations,
					visitedPersisters,
					path,
					currentDepth,
					factory
					);
			}
			else
			{
				IType type = persister.ElementType;
				if ( type.IsAssociationType )
				{
					// a many-to-many
					IAssociationType associationType = (IAssociationType) type;
					JoinType joinType = GetJoinType(
						associationType,
						persister.EnableJoinedFetch,
						path,
						persister.TableName,
						persister.ElementColumnNames,
						factory
						);

					if ( joinType != JoinType.None )
					{
						string[] columns = StringHelper.Qualify( alias, persister.ElementColumnNames );
						WalkAssociationTree(
							associationType,
							columns,
							persister,
							alias,
							associations,
							visitedPersisters,
							path,
							currentDepth,
							joinType,
							factory
							);
					}
				}
				else if ( type.IsComponentType )
				{
					WalkCompositeElementTree(
						(IAbstractComponentType) type,
						persister.ElementColumnNames,
						persister,
						alias,
						associations,
						new HashedSet(),
						path,
						currentDepth,
						factory
						);
				}
			}

			return associations;
		}

		/// <summary>
		/// Is this an association that we cannot possibly load by outer
		/// join, no matter what the mapping or subclass specifies?
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="type"></param>
		/// <param name="propertyNumber"></param>
		/// <returns></returns>
		private bool IsJoinedFetchAlwaysDisabled( IOuterJoinLoadable persister, IAssociationType type, int propertyNumber )
		{
			//NOTE: workaround for problem with 1-to-1 defined on a subclass "accidently" picking up an object
			//TODO: really, this should use the "key" columns of the subclass table, then we don't need this check!
			//      (I *think*)

			return type.IsEntityType && ( (EntityType) type).IsOneToOne && persister.IsDefinedOnSubclass( propertyNumber );
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="associationType"></param>
		/// <param name="persister"></param>
		/// <param name="propertyNumber"></param>
		/// <param name="alias"></param>
		/// <param name="associations"></param>
		/// <param name="visitedPersisters"></param>
		/// <param name="path"></param>
		/// <param name="currentDepth"></param>
		/// <param name="factory"></param>
		private void WalkAssociationTree(
			IAssociationType associationType,
			IOuterJoinLoadable persister,
			int propertyNumber,
			string alias,
			IList associations,
			ISet visitedPersisters,
			string path,
			int currentDepth,
			ISessionFactoryImplementor factory )
		{
			if ( IsJoinedFetchAlwaysDisabled( persister, associationType, propertyNumber ) )
			{
				return;
			}
			string[] aliasedForeignKeyColumns = GetAliasedForeignKeyColumns( persister, alias, associationType, persister.ToColumns( alias, propertyNumber ) );
			string[] foreignKeyColumns = GetForeignKeyColumns( persister, associationType, persister.GetSubclassPropertyColumnNames( propertyNumber ) );
			string subpath = SubPath( path, persister.GetSubclassPropertyName( propertyNumber ) );
			JoinType joinType = GetJoinType(
				associationType,
				persister.EnableJoinedFetch( propertyNumber ),
				subpath,
				persister.GetSubclassPropertyTableName( propertyNumber ),
				foreignKeyColumns,
				factory
				);

			if ( joinType != JoinType.None )
			{
				WalkAssociationTree( 
					associationType, 
					aliasedForeignKeyColumns, 
					persister, 
					alias, 
					associations, 
					visitedPersisters, 
					subpath, 
					currentDepth,
					joinType,
					factory
					);
			}
		}

		/// <summary>
		/// For an entity class, add to a list of associations to be fetched by outerjoin
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="alias"></param>
		/// <param name="associations"></param>
		/// <param name="visitedPersisters"></param>
		/// <param name="path"></param>
		/// <param name="currentDepth"></param>
		/// <param name="factory"></param>
		private void WalkClassTree( 
			IOuterJoinLoadable persister, 
			string alias, 
			IList associations, 
			ISet visitedPersisters, 
			string path,
			int currentDepth,
			ISessionFactoryImplementor factory )
		{
			int n = persister.CountSubclassProperties();
			for( int i = 0; i < n; i++ )
			{
				IType type = persister.GetSubclassPropertyType( i );
				if( type.IsAssociationType )
				{
					WalkAssociationTree(
						( IAssociationType ) type,
						persister,
						i,
						alias,
						associations,
						visitedPersisters,
						path,
						currentDepth,
						factory
						);

				}
				else if( type.IsComponentType )
				{
					WalkComponentTree(
						( IAbstractComponentType ) type,
						i,
						persister.GetSubclassPropertyColumnNames( i ),
						persister.ToColumns( alias, i ),
						persister,
						alias,
						associations,
						visitedPersisters,
						SubPath( path, persister.GetSubclassPropertyName( i ) ),
						currentDepth,
						factory
						);
				}
			}
		}

		/// <summary>
		/// For a component, add to a list of associations to be fetched by outerjoin
		/// </summary>
		/// <param name="componentType"></param>
		/// <param name="propertyNumber"></param>
		/// <param name="cols"></param>
		/// <param name="persister"></param>
		/// <param name="alias"></param>
		/// <param name="associations"></param>
		/// <param name="visitedPersisters"></param>
		/// <param name="aliasedCols"></param>
		/// <param name="path"></param>
		/// <param name="currentDepth"></param>
		/// <param name="factory"></param>
		private void WalkComponentTree(
			IAbstractComponentType componentType,
			int propertyNumber,
			string[ ] cols,
			string[ ] aliasedCols,
			IOuterJoinLoadable persister,
			string alias,
			IList associations,
			ISet visitedPersisters,
			string path,
			int currentDepth,
			ISessionFactoryImplementor factory )
		{
			IType[ ] types = componentType.Subtypes;
			string[ ] propertyNames = componentType.PropertyNames;
			int begin = 0;
			for( int i = 0; i < types.Length; i++ )
			{
				int length = types[ i ].GetColumnSpan( factory );
				string[ ] range = ArrayHelper.Slice( cols, begin, length );
				string[ ] aliasedRange = ArrayHelper.Slice( aliasedCols, begin, length );

				if( types[ i ].IsAssociationType )
				{
					IAssociationType associationType = ( IAssociationType ) types[ i ];

					if ( IsJoinedFetchAlwaysDisabled( persister, associationType, propertyNumber ) )
					{
						return;
					}

					string[] aliasedFkColumns = GetAliasedForeignKeyColumns( persister, alias, associationType, aliasedRange );
					string[] fkColumns = GetForeignKeyColumns( persister, associationType, range );
					string subpath = SubPath( path, propertyNames[ i ] );
					JoinType joinType = GetJoinType(
						associationType,
						componentType.EnableJoinedFetch( i ),
						subpath,
						persister.GetSubclassPropertyTableName( propertyNumber ),
						fkColumns,
						factory
						);

					if ( joinType != JoinType.None )
					{
						WalkAssociationTree(
							associationType,
							aliasedFkColumns,
							persister,
							alias,
							associations,
							visitedPersisters,
							subpath,
							currentDepth,
							joinType,
							factory
							);
					}
				}
				else if( types[ i ].IsComponentType )
				{
					string subpath = SubPath( path, propertyNames[ i ] );

					WalkComponentTree( 
						( IAbstractComponentType ) types[ i ], 
						propertyNumber, 
						range, 
						aliasedRange, 
						persister, 
						alias, 
						associations, 
						visitedPersisters, 
						subpath, 
						currentDepth,
						factory
						);
				}
				begin += length;
			}
		}

		/// <summary>
		/// For a composite element, add to a list of associations to be fetched by outerjoin
		/// </summary>
		/// <param name="compositeType"></param>
		/// <param name="cols"></param>
		/// <param name="persister"></param>
		/// <param name="alias"></param>
		/// <param name="associations"></param>
		/// <param name="visitedPersisters"></param>
		/// <param name="path"></param>
		/// <param name="currentDepth"></param>
		/// <param name="factory"></param>
		private void WalkCompositeElementTree(
			IAbstractComponentType compositeType,
			string[ ] cols,
			IQueryableCollection persister,
			string alias,
			IList associations,
			ISet visitedPersisters,
			string path,
			int currentDepth,
			ISessionFactoryImplementor factory )
		{
			IType[ ] types = compositeType.Subtypes;
			string[ ] propertyNames = compositeType.PropertyNames;
			int begin = 0;
			for( int i = 0; i < types.Length; i++ )
			{
				int length = types[ i ].GetColumnSpan( factory );
				string[ ] range = ArrayHelper.Slice( cols, begin, length );

				if( types[ i ].IsAssociationType )
				{
					IAssociationType associationType = types[ i ] as IAssociationType;

					// simple, because we can't have a one-to-one or collection in a composite element:
					string[] aliasedForeignKeyColumns = StringHelper.Qualify( alias, range );
					string subpath = SubPath( path, propertyNames[ i ] );
					JoinType joinType = GetJoinType(
						associationType,
						compositeType.EnableJoinedFetch( i ),
						subpath,
						persister.TableName,
						range,
						factory
						);

					if ( joinType != JoinType.None )
					{
						WalkAssociationTree( 
							associationType, 
							aliasedForeignKeyColumns, 
							persister, 
							alias, 
							associations, 
							visitedPersisters, 
							subpath, 
							currentDepth,
							joinType,
							factory
							);
					}
				}
				else if( types[ i ].IsComponentType )
				{
					string subpath = SubPath( path, propertyNames[ i ] );
					WalkCompositeElementTree(
						( IAbstractComponentType ) types[ i ],
						range,
						persister,
						alias,
						associations,
						visitedPersisters,
						subpath,
						currentDepth,
						factory
						);

				}
				begin += length;
			}
		}

		/// <summary>
		///  Add on association (one-to-one or many-to-one) to a list of associations be fetched by outerjoin (if necessary)
		/// </summary>
		/// <param name="type"></param>
		/// <param name="aliasedForeignKeyColumns"></param>
		/// <param name="persister"></param>
		/// <param name="alias"></param>
		/// <param name="associations"></param>
		/// <param name="visitedPersisters"></param>
		/// <param name="path"></param>
		/// <param name="currentDepth"></param>
		/// <param name="joinType"></param>
		/// <param name="factory"></param>
		private void WalkAssociationTree(
			IAssociationType type,
			string[ ] aliasedForeignKeyColumns,
			IJoinable persister,
			string alias,
			IList associations,
			ISet visitedPersisters,
			string path,
			int currentDepth,
			JoinType joinType,
			ISessionFactoryImplementor factory )
		{
			IJoinable joinable = type.GetJoinable( factory );

			int maxFetchDepth = factory.MaximumFetchDepth;

			bool enabled = ( joinType == JoinType.InnerJoin ) || (
				( maxFetchDepth <= 0 || currentDepth < maxFetchDepth ) &&
				!visitedPersisters.Contains( joinable ) &&
				( !joinable.IsCollection || !ContainsCollectionPersister( associations ) )
				);

			if ( enabled )
			{
				visitedPersisters.Add( persister );
				OuterJoinableAssociation assoc = new OuterJoinableAssociation();
				associations.Add( assoc );

				// After adding to collection!!
				string subalias = GenerateTableAlias( joinable.Name, associations.Count, path, joinable.IsManyToMany );

				assoc.Joinable = joinable;
				assoc.TableName = joinable.TableName;
				assoc.PrimaryKeyColumns = type.GetReferencedColumns( factory );
				assoc.ForeignKeyColumns = aliasedForeignKeyColumns;
				assoc.Subalias = subalias;
				assoc.Owner = GetPosition( alias, associations );
				assoc.IsOneToOne = type.IsEntityType && ( (EntityType) type).IsOneToOne && ( (EntityType) type).IsUniqueKeyReference;
				assoc.JoinType = joinType;

				if ( assoc.ForeignKeyColumns.Length != assoc.PrimaryKeyColumns.Length || assoc.ForeignKeyColumns.Length == 0 )
				{
					throw new MappingException( string.Format( "Invalid join columns for association: {0}", path ) );
				}

				int nextDepth = currentDepth + 1;
				if ( !joinable.IsCollection )
				{
					if ( joinable is IOuterJoinLoadable )
					{
						WalkClassTree( (IOuterJoinLoadable) joinable, subalias, associations, visitedPersisters, path, nextDepth, factory );
					}
				}
				else
				{
					if ( joinable is IQueryableCollection )
					{
						WalkCollectionTree( (IQueryableCollection) joinable, subalias, associations, visitedPersisters, path, nextDepth, factory ) ;
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		/// <param name="type"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		protected bool IsJoinedFetchEnabledByDefault( OuterJoinFetchStrategy config, IAssociationType type, ISessionFactoryImplementor factory )
		{
			if ( !type.IsEntityType && !type.IsPersistentCollectionType )
			{
				return false;
			}
			else
			{
				switch ( config )
				{
					case OuterJoinFetchStrategy.Eager:
						return true;

					case OuterJoinFetchStrategy.Lazy:
						return false;

					case OuterJoinFetchStrategy.Auto:
						if ( !factory.IsOuterJoinedFetchEnabled )
						{
							return false;
						}
						if ( type.IsEntityType )
						{
							EntityType entityType = type as EntityType;
							IClassPersister persister = factory.GetPersister( entityType.AssociatedClass );
							return !persister.HasProxy || ( entityType.IsOneToOne && ( (OneToOneType) entityType).IsNullable );
						}
						else
						{
							return false;
						}

					default:
						throw new ArgumentOutOfRangeException( "config", config, "Unknown OJ strategy " + config.ToString() );
				}
			}
		}

		/// <summary></summary>
		protected override int[] Owners
		{
			get { return owners; }
		}

		/// <summary></summary>
		protected void SetOwners( int[] values )
		{
			owners = values;
		}

		/// <summary></summary>
		protected LockMode[ ] LockModeArray
		{
			get { return lockModeArray; }
			set { lockModeArray = value; }
		}

		/// <summary></summary>
		protected internal override SqlString SqlString
		{
			get { return sqlString; }

			set 
			{
				sqlString = value; 
				if ( value != null )
				{
					System.Diagnostics.Debug.WriteLine( value.ToString() );
				}
			}
		}

		/// <summary></summary>
		protected override ILoadable[ ] Persisters
		{
			get { return classPersisters; }
			set { classPersisters = value; }
		}

		/// <summary>
		/// Generate a select list of columns containing all properties of the entity classes
		/// </summary>
		/// <param name="associations"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public string SelectString( IList associations, ISessionFactoryImplementor factory )
		{
			if ( associations.Count == 0 )
			{
				return string.Empty;
			}
			else
			{
				StringBuilder buf = new StringBuilder( associations.Count * 100 );
				buf.Append( StringHelper.CommaSpace );
				int aliasCount = 0;
				for( int i = 0; i < associations.Count; i++ )
				{
					OuterJoinableAssociation join = ( OuterJoinableAssociation ) associations[ i ];
					string selectFragment = join.Joinable.SelectFragment( 
						join.Subalias,
						Suffixes[ aliasCount ],
						join.JoinType == JoinType.LeftOuterJoin 
						).ToString() ;

					buf.Append( selectFragment );
					if ( join.Joinable.ConsumesAlias() )
					{
						aliasCount++;
					}
					if( i < associations.Count - 1 && !selectFragment.Trim().Equals( string.Empty ) )
					{
						buf.Append( StringHelper.CommaSpace );
					}
				}
				return buf.ToString();
			}
		}

		/// <summary></summary>
		protected override string[] Suffixes
		{
			get { return suffixes; }
			set { suffixes = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="className"></param>
		/// <param name="n"></param>
		/// <param name="path"></param>
		/// <param name="isLinkTable"></param>
		/// <returns></returns>
		protected virtual string GenerateTableAlias( string className, int n, string path, bool isLinkTable )
		{
			return GenerateAlias( className, n );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		protected virtual string GenerateRootAlias( string tableName )
		{
			return GenerateAlias( tableName, 0 );
		}

		/// <summary></summary>
		protected override ICollectionPersister CollectionPersister
		{
			get { return null; }
		}

		/// <summary>
		/// Generate a sequence of <tt>LEFT OUTER JOIN</tt> clauses for the given associations.
		/// </summary>
		/// <param name="associations"></param>
		/// <returns></returns>
		protected JoinFragment MergeOuterJoins( IList associations )
		{
			JoinFragment outerjoin = Dialect.CreateOuterJoinFragment();

			foreach( OuterJoinLoader.OuterJoinableAssociation oj in associations )
			{
				outerjoin.AddJoin( oj.TableName, oj.Subalias, oj.ForeignKeyColumns, oj.PrimaryKeyColumns, oj.JoinType );
				outerjoin.AddJoins( 
					oj.Joinable.FromJoinFragment( oj.Subalias, false, true ),
					oj.Joinable.WhereJoinFragment( oj.Subalias, false, true )
					);
			}

			return outerjoin;
		}

		/// <summary>
		/// Count the number of instances of Joinable which are actually
		/// also instances of Loadable, or are one-to-many associations
		/// </summary>
		/// <param name="associations"></param>
		/// <returns></returns>
		protected static int CountClassPersisters( IList associations )
		{
			int result = 0;
			foreach ( OuterJoinableAssociation oj in associations )
			{
				if ( oj.Joinable.ConsumesAlias() ) 
				{
					result++;
				}
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="associations"></param>
		/// <returns></returns>
		protected static bool ContainsCollectionPersister( IList associations )
		{
			foreach( OuterJoinLoader.OuterJoinableAssociation oj in associations )
			{
				if ( oj.Joinable.IsCollection )
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lockModes"></param>
		/// <returns></returns>
		protected override LockMode[ ] GetLockModes( IDictionary lockModes )
		{
			return lockModeArray;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		/// <param name="lockMode"></param>
		/// <returns></returns>
		protected LockMode[ ] CreateLockModeArray( int length, LockMode lockMode )
		{
			LockMode[ ] lmArray = new LockMode[length];
			for( int i = 0; i < length; i++ )
			{
				lmArray[ i ] = lockMode;
			}
			return lmArray;
		}

		private string SubPath( string path, string property )
		{
			if( path == null || path.Length == 0 )
			{
				return property;
			}
			else
			{
				return StringHelper.Qualify( path, property );
			}
		}

		/// <summary>
		/// Render the where condition for a (batch) load by identifier / collection key
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="alias"></param>
		/// <param name="columnNames"></param>
		/// <param name="type"></param>
		/// <param name="batchSize"></param>
		/// <returns></returns>
		protected static SqlString WhereString( ISessionFactoryImplementor factory, string alias, string[] columnNames, IType type, int batchSize )
		{
			WhereBuilder whereFrag = new WhereBuilder( factory );

			return whereFrag.WhereClause( alias, columnNames, type );
		}

		/// <summary>
		/// Get the position of the join with the given alias in the list of joins, or -1 if not found
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="associations"></param>
		/// <returns></returns>
		private static int GetPosition( string alias, IList associations )
		{
			int result = 0;
			for( int i = 0; i < associations.Count; i++ )
			{
				OuterJoinableAssociation oj = associations[ i ] as OuterJoinableAssociation;
				if ( oj.Joinable.ConsumesAlias() )
				{
					if ( oj.Subalias.Equals( alias ) )
					{
						return result;
					}
					result++;
				}
			}

			return -1;
		}

		private static string[] GetAliasedForeignKeyColumns( IOuterJoinLoadable persister, string alias, IAssociationType associationType, string[] aliasedPropertyColumns )
		{
			if ( associationType.UsePrimaryKeyAsForeignKey )
			{
				// a one-to-one association or collection
				return StringHelper.Qualify( alias, persister.IdentifierColumnNames );
			}
			else
			{
				// a many-to-one association
				return aliasedPropertyColumns;
			}
		}

		private static string[] GetForeignKeyColumns( IOuterJoinLoadable persister, IAssociationType associationType, string[] propertyColumns )
		{
			if ( associationType.UsePrimaryKeyAsForeignKey )
			{
				return persister.IdentifierColumnNames;
			}
			else
			{
				return propertyColumns;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="oj"></param>
		/// <param name="joins"></param>
		/// <param name="dontIgnore"></param>
		/// <returns></returns>
		protected int ToOwner( OuterJoinableAssociation oj, int joins, bool dontIgnore )
		{
			if ( dontIgnore )
			{
				return oj.Owner == -1 ? joins : oj.Owner;  //TODO: UGLY AS SIN!
			}
			else
			{
				return -1;
			}
		}
	}
}