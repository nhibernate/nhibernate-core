using System;
using System.Collections;
using Iesi.Collections;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Persister.Collection;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Custom
{
	public class SQLCustomQuery : ICustomQuery
	{
		private readonly System.Type[] entityNames;
		private readonly string[] collectionRoles;
		private readonly int[] collectionOwners;
		private readonly int[] entityOwners;
		private readonly LockMode[] lockModes;
		private readonly SqlString sql;
		private readonly ISet querySpaces = new HashedSet();
		private readonly IDictionary namedParameters;
		private readonly IType[] scalarTypes;
		private readonly string[] scalarColumnAliases;
		private readonly IEntityAliases[] entityDescriptors;
		private readonly ICollectionAliases[] collectionDescriptors;
		private readonly string[] returnAliases;

		public SqlString SQL
		{
			get { return sql; }
		}

		public IDictionary NamedParameterBindPoints
		{
			get { return namedParameters; }
		}

		public string[] CollectionRoles
		{
			get { return collectionRoles; }
		}

		public System.Type[] EntityNames
		{
			get { return entityNames; }
		}

		public LockMode[] LockModes
		{
			get { return lockModes; }
		}

		public IEntityAliases[] EntityAliases
		{
			get { return entityDescriptors; }
		}

		public ICollectionAliases[] CollectionAliases
		{
			get { return collectionDescriptors; }
		}

		public ISet QuerySpaces
		{
			get { return querySpaces; }
		}

		public int[] CollectionOwner
		{
			get { return collectionOwners; }
		}

		public int[] EntityOwners
		{
			get { return entityOwners; }
		}

		public string[] ScalarColumnAliases
		{
			get { return scalarColumnAliases; }
		}

		public IType[] ScalarTypes
		{
			get { return scalarTypes; }
		}

		public SQLCustomQuery(
				ISQLQueryReturn[] queryReturns,
				string sqlQuery,
				ICollection additionalQuerySpaces,
				ISessionFactoryImplementor factory )
		{
			SQLQueryReturnProcessor processor = new SQLQueryReturnProcessor( queryReturns, factory );
			processor.Process();

			IDictionary[] propertyResultMaps = new IDictionary[ processor.PropertyResults.Count ];
			processor.PropertyResults.CopyTo( propertyResultMaps, 0 );
			IDictionary[] collectionResultMaps = new IDictionary[ processor.CollectionPropertyResults.Count ];
			processor.CollectionPropertyResults.CopyTo( collectionResultMaps, 0 );

			IList collectionSuffixes = new ArrayList();
			IList collectionOwnerAliases = processor.CollectionOwnerAliases;
			IList collectionPersisters = processor.CollectionPersisters;
			int size = collectionPersisters.Count;
			if( size != 0 )
			{
				collectionOwners = new int[ size ];
				collectionRoles = new string[ size ];
				//collectionDescriptors = new CollectionAliases[size];
				for( int i = 0; i < size; i++ )
				{
					ICollectionPersister collectionPersister = ( ICollectionPersister ) collectionPersisters[ i ];
					collectionRoles[ i ] = ( collectionPersister ).Role;
					collectionOwners[ i ] = processor.Aliases.IndexOf( collectionOwnerAliases[ i ] );
					string suffix = i + "__";
					collectionSuffixes.Add( suffix );
					//collectionDescriptors[i] = new GeneratedCollectionAliases( collectionResultMaps[i], collectionPersister, suffix );
				}
			}
			else
			{
				collectionRoles = null;
				//collectionDescriptors = null;
				collectionOwners = null;
			}

			string[] aliases = ArrayHelper.ToStringArray( processor.Aliases );
			string[] collAliases = ArrayHelper.ToStringArray( processor.CollectionAliases );
			string[] collSuffixes = ArrayHelper.ToStringArray( collectionSuffixes );

			ISqlLoadable[] entityPersisters = new ISqlLoadable[ processor.Persisters.Count ];
			processor.Persisters.CopyTo( entityPersisters, 0 );

			ISqlLoadableCollection[] collPersisters = new ISqlLoadableCollection[ collectionPersisters.Count ];
			collectionPersisters.CopyTo( collPersisters, 0 );

			lockModes = new LockMode[ processor.LockModes.Count ];
			processor.LockModes.CopyTo( lockModes, 0 );

			scalarColumnAliases = ArrayHelper.ToStringArray( processor.ScalarColumnAliases );
			scalarTypes = ArrayHelper.ToTypeArray( processor.ScalarTypes );

			// need to match the "sequence" of what we return. scalar first, entity last.
			returnAliases = ArrayHelper.Join( scalarColumnAliases, aliases );

			string[] suffixes = BasicLoader.GenerateSuffixes( entityPersisters.Length );

			SQLQueryParser parser = new SQLQueryParser(
					sqlQuery,
					processor.Alias2Persister,
					processor.Alias2Return,
					aliases,
					collAliases,
					collPersisters,
					suffixes,
					collSuffixes
			);

			sql = parser.Process();

			namedParameters = parser.NamedParameters;

			// Populate entityNames, entityDescrptors and querySpaces
			entityNames = new System.Type[ entityPersisters.Length ];
			entityDescriptors = new IEntityAliases[ entityPersisters.Length ];
			for( int i = 0; i < entityPersisters.Length; i++ )
			{
				ISqlLoadable persister = entityPersisters[ i ];
				//alias2Persister.put( aliases[i], persister );
				//TODO: Does not consider any other tables referenced in the query
				querySpaces.AddAll( persister.QuerySpaces );
				entityNames[ i ] = persister.MappedClass;
				if( parser.QueryHasAliases )
				{
					entityDescriptors[ i ] = new DefaultEntityAliases(
							propertyResultMaps[ i ],
							entityPersisters[ i ],
							suffixes[ i ]
						);
				}
				else
				{
					entityDescriptors[ i ] = new ColumnEntityAliases(
							propertyResultMaps[ i ],
							entityPersisters[ i ],
							suffixes[ i ]
						);
				}
			}
			if( additionalQuerySpaces != null )
			{
				querySpaces.AddAll( additionalQuerySpaces );
			}

			if( size != 0 )
			{
				collectionDescriptors = new ICollectionAliases[ size ];
				for( int i = 0; i < size; i++ )
				{
					ICollectionPersister collectionPersister = ( ICollectionPersister ) collectionPersisters[ i ];
					string suffix = i + "__";
					if( parser.QueryHasAliases )
					{
						collectionDescriptors[ i ] = new GeneratedCollectionAliases( collectionResultMaps[ i ], collectionPersister, suffix );
					}
					else
					{
						collectionDescriptors[ i ] = new ColumnCollectionAliases( collectionResultMaps[ i ], ( ISqlLoadableCollection ) collectionPersister );
					}
				}
			}
			else
			{
				collectionDescriptors = null;
			}


			// Resolve owners
			IDictionary alias2OwnerAlias = processor.Alias2OwnerAlias;
			int[] ownersArray = new int[ entityPersisters.Length ];
			for( int j = 0; j < aliases.Length; j++ )
			{
				string ownerAlias = ( string ) alias2OwnerAlias[ aliases[ j ] ];
				if( StringHelper.IsNotEmpty( ownerAlias ) )
				{
					ownersArray[ j ] = processor.Aliases.IndexOf( ownerAlias );
				}
				else
				{
					ownersArray[ j ] = -1;
				}
			}
			if( ArrayHelper.IsAllNegative( ownersArray ) )
			{
				ownersArray = null;
			}
			this.entityOwners = ownersArray;

		}

		public string[] ReturnAliases
		{
			get { return returnAliases; }
		}
	}
}
