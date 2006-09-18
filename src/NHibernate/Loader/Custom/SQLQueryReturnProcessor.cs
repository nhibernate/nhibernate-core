using System;
using System.Collections;

using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Loader.Custom
{
	public class SQLQueryReturnProcessor
	{
		private ISQLQueryReturn[] queryReturns;

		private readonly IList aliases = new ArrayList();
		private readonly IList persisters = new ArrayList();
		private readonly IList propertyResults = new ArrayList();
		private readonly IList lockModes = new ArrayList();
		private readonly IDictionary alias2Persister = new Hashtable();
		private readonly IDictionary alias2Return = new Hashtable();
		private readonly IDictionary alias2OwnerAlias = new Hashtable();

		private readonly IList scalarTypes = new ArrayList();
		private readonly IList scalarColumnAliases = new ArrayList();

		private readonly ISessionFactoryImplementor factory;

		private IList collectionOwnerAliases = new ArrayList();
		private IList collectionAliases = new ArrayList();
		private IList collectionPersisters = new ArrayList();
		private IList collectionResults = new ArrayList();

		private ISessionFactoryImplementor Factory
		{
			get { return factory; }
		}

		private ISqlLoadable GetSQLLoadable( string entityName )
		{
			IEntityPersister persister = Factory.GetEntityPersister( entityName );
			ISqlLoadable result = persister as ISqlLoadable;
			if( result == null )
			{
				throw new MappingException( "class persister is not SQLLoadable: " + entityName );
			}
			return result;
		}

		public SQLQueryReturnProcessor(
			ISQLQueryReturn[] queryReturns,
			ISessionFactoryImplementor factory )
		{
			this.queryReturns = queryReturns;
			this.factory = factory;
		}

		public void Process()
		{
			// first, break down the returns into maps keyed by alias
			// so that role returns can be more easily resolved to their owners
			for( int i = 0; i < queryReturns.Length; i++ )
			{
				if (queryReturns[i] is SQLQueryNonScalarReturn)
				{
					SQLQueryNonScalarReturn rtn = (SQLQueryNonScalarReturn) queryReturns[i];
					alias2Return[rtn.Alias] = rtn;
					if (rtn is SQLQueryJoinReturn)
					{
						SQLQueryJoinReturn roleReturn = (SQLQueryJoinReturn) queryReturns[i];
						alias2OwnerAlias[roleReturn.Alias] = roleReturn.OwnerAlias;
					}
				}
			}

			// Now, process the returns
			for( int i = 0; i < queryReturns.Length; i++ )
			{
				ProcessReturn( queryReturns[ i ] );
			}
		}

		private void ProcessReturn( ISQLQueryReturn rtn )
		{
			if (rtn is SQLQueryScalarReturn)
			{
				ProcessScalarReturn((SQLQueryScalarReturn) rtn);
			}
			else if( rtn is SQLQueryRootReturn )
			{
				ProcessRootReturn( ( SQLQueryRootReturn ) rtn );
			}
			else if (rtn is SQLQueryCollectionReturn)
			{
				ProcessCollectionReturn((SQLQueryCollectionReturn) rtn);
			}
			else
			{
				ProcessJoinReturn((SQLQueryJoinReturn) rtn);
			}
		}

		private void ProcessScalarReturn(SQLQueryScalarReturn typeReturn)
		{
			scalarColumnAliases.Add(typeReturn.ColumnAlias);
			scalarTypes.Add(typeReturn.Type);
		}

		private void ProcessRootReturn( SQLQueryRootReturn rootReturn )
		{
			if( alias2Persister.Contains( rootReturn.Alias ) )
			{
				// already been processed...
				return;
			}

			ISqlLoadable persister = GetSQLLoadable( rootReturn.ReturnEntityName );
			aliases.Add( rootReturn.Alias );
			AddPersister( rootReturn.PropertyResultsMap, persister );
			alias2Persister[ rootReturn.Alias ] = persister;
			lockModes.Add( rootReturn.LockMode );
		}

		private void AddPersister( IDictionary propertyResult, ISqlLoadable persister )
		{
			persisters.Add( persister );
			propertyResults.Add( propertyResult );
		}

		private void AddCollection( string role, string alias, IDictionary propertyResults, LockMode lockMode )
		{
			IQueryableCollection collectionPersister = ( IQueryableCollection ) Factory.GetCollectionPersister( role );
			collectionPersisters.Add( collectionPersister );
			collectionAliases.Add( alias );
			this.collectionResults.Add( propertyResults );

			if( collectionPersister.IsOneToMany )
			{
				ISqlLoadable persister = ( ISqlLoadable ) collectionPersister.ElementPersister;
				aliases.Add( alias );
				AddPersister( Filter( propertyResults ), persister );
				lockModes.Add( lockMode );
				alias2Persister[ alias ] = persister;
			}
		}

		private IDictionary Filter( IDictionary propertyResults )
		{
			IDictionary result = new Hashtable( propertyResults.Count );

			string keyPrefix = "element.";

			foreach( DictionaryEntry element in propertyResults )
			{
				string path = ( string ) element.Key;
				if( path.StartsWith( keyPrefix ) )
				{
					result[ path.Substring( keyPrefix.Length ) ] = element.Value;
				}
			}

			return result;
		}

		private void ProcessCollectionReturn( SQLQueryCollectionReturn collectionReturn )
		{
			// we are initializing an owned collection
			//collectionOwners.add( new Integer(-1) );
			collectionOwnerAliases.Add( null );
			String role = collectionReturn.OwnerEntityName + '.' + collectionReturn.OwnerProperty;
			AddCollection(
					role,
					collectionReturn.Alias,
					collectionReturn.PropertyResultsMap,
					collectionReturn.LockMode
				);
		}

		private void ProcessJoinReturn( SQLQueryJoinReturn roleReturn )
		{
			string alias = roleReturn.Alias;
			if( alias2Persister.Contains( alias ) || collectionAliases.Contains( alias ) )
			{
				// already been processed...
				return;
			}

			string ownerAlias = roleReturn.OwnerAlias;

			// Make sure the owner alias is known...
			if( !alias2Return.Contains( ownerAlias ) )
			{
				throw new HibernateException(
						"Owner alias [" + ownerAlias + "] is unknown for alias [" +
						alias + "]"
				);
			}

			// If this return's alias has not been processed yet, do so b4 further processing of this return
			if( !alias2Persister.Contains( ownerAlias ) )
			{
				SQLQueryNonScalarReturn ownerReturn = ( SQLQueryNonScalarReturn ) alias2Return[ ownerAlias ];
				ProcessReturn( ownerReturn );
			}

			ISqlLoadable ownerPersister = ( ISqlLoadable ) alias2Persister[ ownerAlias ];
			IType returnType = ownerPersister.GetPropertyType( roleReturn.OwnerProperty );

			if( returnType.IsCollectionType )
			{
				string role = ownerPersister.MappedClass.FullName + '.' + roleReturn.OwnerProperty;
				AddCollection( role, alias, roleReturn.PropertyResultsMap, roleReturn.LockMode );
				collectionOwnerAliases.Add( ownerAlias );
			}
			else if( returnType.IsEntityType )
			{
				EntityType eType = ( EntityType ) returnType;
				string returnEntityName = eType.AssociatedClass.AssemblyQualifiedName;
				ISqlLoadable persister = GetSQLLoadable( returnEntityName );
				aliases.Add( alias );
				AddPersister( roleReturn.PropertyResultsMap, persister );
				lockModes.Add( roleReturn.LockMode );
				alias2Persister[ alias ] = persister;
			}
		}

		public IList CollectionAliases
		{
			get { return collectionAliases; }
		}

		public IList CollectionOwnerAliases
		{
			get { return collectionOwnerAliases; }
		}

		public IList CollectionPersisters
		{
			get { return collectionPersisters; }
		}

		public IDictionary Alias2Persister
		{
			get { return alias2Persister; }
		}

		public IList Aliases
		{
			get { return aliases; }
		}

		public IList LockModes
		{
			get { return lockModes; }
		}

		public IList Persisters
		{
			get { return persisters; }
		}

		public IDictionary Alias2OwnerAlias
		{
			get { return alias2OwnerAlias; }
		}

		public IList ScalarTypes
		{
			get { return scalarTypes; }
		}

		public IList ScalarColumnAliases
		{
			get { return scalarColumnAliases; }
		}

		public IList PropertyResults
		{
			get { return propertyResults; }
		}

		public IList CollectionPropertyResults
		{
			get { return collectionResults; }
		}

		public IDictionary Alias2Return
		{
			get { return alias2Return; }
		}
	}
}
