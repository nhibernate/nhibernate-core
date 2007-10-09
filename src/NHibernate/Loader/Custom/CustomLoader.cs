using System;
using System.Collections;
using System.Data;
using Iesi.Collections;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Custom
{
	public class CustomLoader : Loader
	{
		// Currently *not* cachable if autodiscover types is in effect (e.g. "select * ...")

		private readonly SqlString sql;
		private readonly ISet querySpaces = new HashedSet();
		private readonly IDictionary namedParameterBindPoints;

		private readonly IQueryable[] entityPersisters;
		private readonly int[] entityOwners;
		private readonly IEntityAliases[] entityAliases;

		private readonly IQueryableCollection[] collectionPersisters;
		private readonly int[] collectionOwners;
		private readonly ICollectionAliases[] collectionAliases;

		private LockMode[] lockModes;
		private readonly ResultRowProcessor rowProcessor;

		private readonly IType[] resultTypes;
		private readonly string[] transformerAliases;

		public CustomLoader(
			ICustomQuery customQuery,
			ISessionFactoryImplementor factory)
			: base(factory)
		{
			this.sql = customQuery.SQL;
			this.querySpaces.AddAll(customQuery.QuerySpaces);
			this.namedParameterBindPoints = customQuery.NamedParameterBindPoints;

			IList entityPersisters = new ArrayList();
			IList entityOwners = new ArrayList();
			IList entityAliases = new ArrayList();

			IList collectionPersisters = new ArrayList();
			IList collectionOwners = new ArrayList();
			IList collectionAliases = new ArrayList();

			IList lockModes = new ArrayList();
			IList resultColumnProcessors = new ArrayList();
			IList nonScalarReturnList = new ArrayList();
			IList resultTypes = new ArrayList();
			IList specifiedAliases = new ArrayList();

			int returnableCounter = 0;
			bool hasScalars = false;

			foreach (IReturn rtn in customQuery.CustomQueryReturns)
			{
				if (rtn is ScalarReturn)
				{
					ScalarReturn scalarRtn = (ScalarReturn) rtn;
					resultTypes.Add(scalarRtn.Type);
					specifiedAliases.Add(scalarRtn.ColumnAlias);
					resultColumnProcessors.Add(
						new ScalarResultColumnProcessor(
							scalarRtn.ColumnAlias,
							scalarRtn.Type
							)
						);
					hasScalars = true;
				}
				else if (rtn is RootReturn)
				{
					RootReturn rootRtn = (RootReturn) rtn;
					IQueryable persister = (IQueryable) factory.GetEntityPersister(rootRtn.EntityName);
					entityPersisters.Add(persister);
					lockModes.Add(rootRtn.LockMode);
					resultColumnProcessors.Add(new NonScalarResultColumnProcessor(returnableCounter++));
					nonScalarReturnList.Add(rtn);
					entityOwners.Add(-1);
					resultTypes.Add(persister.Type);
					specifiedAliases.Add(rootRtn.Alias);
					entityAliases.Add(rootRtn.EntityAliases);
					querySpaces.AddAll(persister.QuerySpaces);
				}
				else if (rtn is CollectionReturn)
				{
					CollectionReturn collRtn = (CollectionReturn) rtn;
					String role = collRtn.OwnerEntityName + "." + collRtn.OwnerProperty;
					IQueryableCollection persister = (IQueryableCollection) factory.GetCollectionPersister(role);
					collectionPersisters.Add(persister);
					lockModes.Add(collRtn.LockMode);
					resultColumnProcessors.Add(new NonScalarResultColumnProcessor(returnableCounter++));
					nonScalarReturnList.Add(rtn);
					collectionOwners.Add(-1);
					resultTypes.Add(persister.Type);
					specifiedAliases.Add(collRtn.Alias);
					collectionAliases.Add(collRtn.CollectionAliases);
					// determine if the collection elements are entities...
					IType elementType = persister.ElementType;
					if (elementType.IsEntityType)
					{
						IQueryable elementPersister = (IQueryable) ((EntityType) elementType).GetAssociatedJoinable(factory);
						entityPersisters.Add(elementPersister);
						entityOwners.Add(-1);
						entityAliases.Add(collRtn.ElementEntityAliases);
						querySpaces.AddAll(elementPersister.QuerySpaces);
					}
				}
				else if (rtn is EntityFetchReturn)
				{
					EntityFetchReturn fetchRtn = (EntityFetchReturn) rtn;
					NonScalarReturn ownerDescriptor = fetchRtn.Owner;
					int ownerIndex = nonScalarReturnList.IndexOf(ownerDescriptor);
					entityOwners.Add(ownerIndex);
					lockModes.Add(fetchRtn.LockMode);
					IQueryable ownerPersister = DetermineAppropriateOwnerPersister(ownerDescriptor);
					EntityType fetchedType = (EntityType) ownerPersister.GetPropertyType(fetchRtn.OwnerProperty);
					System.Type entityName = fetchedType.GetAssociatedClass(Factory);
					IQueryable persister = (IQueryable) factory.GetEntityPersister(entityName);
					entityPersisters.Add(persister);
					nonScalarReturnList.Add(rtn);
					specifiedAliases.Add(fetchRtn.Alias);
					entityAliases.Add(fetchRtn.EntityAliases);
					querySpaces.AddAll(persister.QuerySpaces);
				}
				else if (rtn is CollectionFetchReturn)
				{
					CollectionFetchReturn fetchRtn = (CollectionFetchReturn) rtn;
					NonScalarReturn ownerDescriptor = fetchRtn.Owner;
					int ownerIndex = nonScalarReturnList.IndexOf(ownerDescriptor);
					collectionOwners.Add(ownerIndex);
					lockModes.Add(fetchRtn.LockMode);
					IQueryable ownerPersister = DetermineAppropriateOwnerPersister(ownerDescriptor);
					String role = ownerPersister.ClassName + '.' + fetchRtn.OwnerProperty;
					IQueryableCollection persister = (IQueryableCollection) factory.GetCollectionPersister(role);
					collectionPersisters.Add(persister);
					nonScalarReturnList.Add(rtn);
					specifiedAliases.Add(fetchRtn.Alias);
					collectionAliases.Add(fetchRtn.CollectionAliases);
					// determine if the collection elements are entities...
					IType elementType = persister.ElementType;
					if (elementType.IsEntityType)
					{
						IQueryable elementPersister = (IQueryable) ((EntityType) elementType).GetAssociatedJoinable(factory);
						entityPersisters.Add(elementPersister);
						entityOwners.Add(ownerIndex);
						entityAliases.Add(fetchRtn.ElementEntityAliases);
						querySpaces.AddAll(elementPersister.QuerySpaces);
					}
				}
				else
				{
					throw new HibernateException("unexpected custom query return type : " + rtn.GetType().FullName);
				}
			}

			this.entityPersisters = new IQueryable[entityPersisters.Count];
			for (int i = 0; i < entityPersisters.Count; i++)
			{
				this.entityPersisters[i] = (IQueryable) entityPersisters[i];
			}
			this.entityOwners = ArrayHelper.ToIntArray(entityOwners);
			this.entityAliases = new IEntityAliases[entityAliases.Count];
			for (int i = 0; i < entityAliases.Count; i++)
			{
				this.entityAliases[i] = (IEntityAliases) entityAliases[i];
			}

			this.collectionPersisters = new IQueryableCollection[collectionPersisters.Count];
			for (int i = 0; i < collectionPersisters.Count; i++)
			{
				this.collectionPersisters[i] = (IQueryableCollection) collectionPersisters[i];
			}
			this.collectionOwners = ArrayHelper.ToIntArray(collectionOwners);
			this.collectionAliases = new ICollectionAliases[collectionAliases.Count];
			for (int i = 0; i < collectionAliases.Count; i++)
			{
				this.collectionAliases[i] = (ICollectionAliases) collectionAliases[i];
			}

			this.lockModes = new LockMode[lockModes.Count];
			for (int i = 0; i < lockModes.Count; i++)
			{
				this.lockModes[i] = (LockMode) lockModes[i];
			}

			this.resultTypes = ArrayHelper.ToTypeArray(resultTypes);
			this.transformerAliases = ArrayHelper.ToStringArray(specifiedAliases);

			this.rowProcessor = new ResultRowProcessor(
				hasScalars,
				(ResultColumnProcessor[]) ArrayHelper.ToArray(resultColumnProcessors, typeof(ResultColumnProcessor))
				);
		}

		private IQueryable DetermineAppropriateOwnerPersister(NonScalarReturn ownerDescriptor)
		{
			string entityName = null;
			if (ownerDescriptor is RootReturn)
			{
				entityName = ((RootReturn) ownerDescriptor).EntityName;
			}
			else if (ownerDescriptor is CollectionReturn)
			{
				CollectionReturn collRtn = (CollectionReturn) ownerDescriptor;
				string role = collRtn.OwnerEntityName + "." + collRtn.OwnerProperty;
				ICollectionPersister persister = Factory.GetCollectionPersister(role);
				EntityType ownerType = (EntityType) persister.ElementType;
				entityName = ownerType.GetAssociatedClass(Factory).AssemblyQualifiedName;
			}
			else if (ownerDescriptor is FetchReturn)
			{
				FetchReturn fetchRtn = (FetchReturn) ownerDescriptor;
				IQueryable persister = DetermineAppropriateOwnerPersister(fetchRtn.Owner);
				IType ownerType = persister.GetPropertyType(fetchRtn.OwnerProperty);
				if (ownerType.IsEntityType)
				{
					entityName = ((EntityType) ownerType).GetAssociatedClass(Factory).AssemblyQualifiedName;
				}
				else if (ownerType.IsCollectionType)
				{
					IType ownerCollectionElementType = ((CollectionType) ownerType).GetElementType(Factory);
					if (ownerCollectionElementType.IsEntityType)
					{
						entityName = ((EntityType) ownerCollectionElementType).GetAssociatedClass(Factory).AssemblyQualifiedName;
					}
				}
			}

			if (entityName == null)
			{
				throw new HibernateException("Could not determine fetch owner : " + ownerDescriptor);
			}

			return (IQueryable) Factory.GetEntityPersister(entityName);
		}


		protected internal override ILoadable[] EntityPersisters
		{
			get { return entityPersisters; }
			set { throw new NotSupportedException("CustomLoader.set_EntityPersisters"); }
		}

		protected internal override LockMode[] GetLockModes(IDictionary lockModesMap)
		{
			return lockModes;
		}

		protected override ICollectionPersister[] CollectionPersisters
		{
			get { return collectionPersisters; }
		}

		protected override int[] Owners
		{
			get { return entityOwners; }
		}

		protected override int[] CollectionOwners
		{
			get { return collectionOwners; }
		}

		public ISet QuerySpaces
		{
			get { return querySpaces; }
		}

		// TODO
		//protected string QueryIdentifier
		//{
		//	get { return customQuery.SQL; }
		//}

		public IList List(
			ISessionImplementor session,
			QueryParameters queryParameters)
		{
			return List(session, queryParameters, querySpaces, resultTypes);
		}

		// Not ported: scroll

		protected override object GetResultColumnOrRow(object[] row, IResultTransformer resultTransformer, IDataReader rs,
		                                               ISessionImplementor session)
		{
			return rowProcessor.BuildResultRow(row, rs, resultTransformer != null, session);
		}

		protected override IList GetResultList(IList results, IResultTransformer resultTransformer)
		{
			// meant to handle dynamic instantiation queries...(Copy from QueryLoader)
			HolderInstantiator holderInstantiator = HolderInstantiator.GetHolderInstantiator(
				null,
				resultTransformer,
				ReturnAliasesForTransformer
				);
			if (holderInstantiator.IsRequired)
			{
				for (int i = 0; i < results.Count; i++)
				{
					object[] row = (object[]) results[i];
					object result = holderInstantiator.Instantiate(row);
					results[i] = result;
				}

				return resultTransformer.TransformList(results);
			}
			else
			{
				return results;
			}
		}

		// Not ported: getReturnAliasesForTransformer()

		protected override IEntityAliases[] EntityAliases
		{
			get { return entityAliases; }
		}

		protected override ICollectionAliases[] CollectionAliases
		{
			get { return collectionAliases; }
		}

		public override int[] GetNamedParameterLocs(string name)
		{
			object loc = namedParameterBindPoints[name];
			if (loc == null)
			{
				throw new QueryException(
					"Named parameter does not appear in Query: " +
					name,
					sql.ToString());
			}

			if (loc is int)
			{
				return new int[] {(int) loc};
			}
			else
			{
				return ArrayHelper.ToIntArray((IList) loc);
			}
		}

		private string[] ReturnAliasesForTransformer
		{
			get { return transformerAliases; }
		}

		protected internal override SqlString SqlString
		{
			get { return sql; }
			set { throw new NotSupportedException("CustomLoader.set_SqlString"); }
		}

		public class ResultRowProcessor
		{
			private readonly bool hasScalars;
			private ResultColumnProcessor[] columnProcessors;

			public ResultRowProcessor(bool hasScalars, ResultColumnProcessor[] columnProcessors)
			{
				this.hasScalars = hasScalars || (columnProcessors == null || columnProcessors.Length == 0);
				this.columnProcessors = columnProcessors;
			}

			public object BuildResultRow(
				object[] data,
				IDataReader resultSet,
				bool hasTransformer,
				ISessionImplementor session)
			{
				object[] resultRow;
				if (!hasScalars)
				{
					resultRow = data;
				}
				else
				{
					// build an array with indices equal to the total number
					// of actual returns in the result Hibernate will return
					// for this query (scalars + non-scalars)
					resultRow = new object[columnProcessors.Length];
					for (int i = 0; i < columnProcessors.Length; i++)
					{
						resultRow[i] = columnProcessors[i].Extract(data, resultSet, session);
					}
				}

				return (hasTransformer)
				       	? resultRow
				       	: (resultRow.Length == 1)
				       	  	? resultRow[0]
				       	  	: resultRow;
			}
		}

		public interface ResultColumnProcessor
		{
			object Extract(object[] data, IDataReader resultSet, ISessionImplementor session);
		}

		public class NonScalarResultColumnProcessor : ResultColumnProcessor
		{
			private readonly int position;

			public NonScalarResultColumnProcessor(int position)
			{
				this.position = position;
			}

			public object Extract(
				Object[] data,
				IDataReader resultSet,
				ISessionImplementor session)
			{
				return data[position];
			}
		}

		public class ScalarResultColumnProcessor : ResultColumnProcessor
		{
			private int position = -1;
			private string alias;
			private IType type;

			public ScalarResultColumnProcessor(int position)
			{
				this.position = position;
			}

			public ScalarResultColumnProcessor(String alias, IType type)
			{
				this.alias = alias;
				this.type = type;
			}

			public object Extract(
				object[] data,
				IDataReader resultSet,
				ISessionImplementor session)
			{
				return type.NullSafeGet(resultSet, alias, session, null);
			}
		}

		public override string QueryIdentifier
		{
			get { return sql.ToString(); }
		}
	}
}