using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Param;
using NHibernate.Persister.Collection;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Loader.Collection
{
	/// <summary>
	/// Superclass for loaders that initialize collections
	/// <seealso cref="OneToManyLoader" />
	/// <seealso cref="BasicCollectionLoader" />
	/// </summary>
	public class CollectionLoader : OuterJoinLoader, ICollectionInitializer
	{
		private readonly IQueryableCollection collectionPersister;
		private IParameterSpecification[] parametersSpecifications;

		public CollectionLoader(IQueryableCollection persister, ISessionFactoryImplementor factory,
		                        IDictionary<string, IFilter> enabledFilters) : base(factory, enabledFilters)
		{
			collectionPersister = persister;
		}

		public override bool IsSubselectLoadingEnabled
		{
			get { return HasSubselectLoadableCollections(); }
		}

		protected IType KeyType
		{
			get { return collectionPersister.KeyType; }
		}

		public virtual void Initialize(object id, ISessionImplementor session)
		{
			LoadCollection(session, id, KeyType);
		}

		public override string ToString()
		{
			return GetType().FullName + '(' + collectionPersister.Role + ')';
		}

		protected virtual IEnumerable<IParameterSpecification> CreateParameterSpecificationsAndAssignBackTrack(IEnumerable<Parameter> sqlPatameters)
		{
			// This implementation can manage even the case of batch-loading
			var specifications = new List<IParameterSpecification>();
			int position = 0;
			var parameters = sqlPatameters.ToArray();
			for (var sqlParameterPos = 0; sqlParameterPos < parameters.Length; )
			{
				var specification = new PositionalParameterSpecification(1, 0, position++) { ExpectedType = KeyType };
				var paramTrackers = specification.GetIdsForBackTrack(Factory);
				foreach (var paramTracker in paramTrackers)
				{
					parameters[sqlParameterPos++].BackTrack = paramTracker;
				}
				specifications.Add(specification);
			}
			return specifications;
		}

		protected override IEnumerable<IParameterSpecification> GetParameterSpecifications(QueryParameters queryParameters, ISessionFactoryImplementor sessionFactory)
		{
			return parametersSpecifications ?? (parametersSpecifications = CreateParameterSpecificationsAndAssignBackTrack(SqlString.GetParameters()).ToArray());
		}
	}
}