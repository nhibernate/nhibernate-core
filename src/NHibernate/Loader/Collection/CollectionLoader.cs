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
								IDictionary<string, IFilter> enabledFilters)
			: base(factory, enabledFilters)
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

		protected override IEnumerable<IParameterSpecification> GetParameterSpecifications()
		{
			return parametersSpecifications ?? (parametersSpecifications = CreateParameterSpecificationsAndAssignBackTrack(SqlString.GetParameters()).ToArray());
		}

		protected SqlString GetSubSelectWithLimits(SqlString subquery, ICollection<IParameterSpecification> parameterSpecs, RowSelection processedRowSelection, IDictionary<string, TypedValue> parameters)
		{
			ISessionFactoryImplementor sessionFactory = Factory;
			Dialect.Dialect dialect = sessionFactory.Dialect;

			RowSelection selection = processedRowSelection;
			bool useLimit = UseLimit(selection, dialect);
			if (useLimit)
			{
				bool hasFirstRow = GetFirstRow(selection) > 0;
				bool useOffset = hasFirstRow && dialect.SupportsLimitOffset;
				int max = GetMaxOrLimit(dialect, selection);
				int? skip = useOffset ? (int?)dialect.GetOffsetValue(GetFirstRow(selection)) : null;
				int? take = max != int.MaxValue ? (int?)max : null;

				Parameter skipSqlParameter = null;
				Parameter takeSqlParameter = null;
				if (skip.HasValue)
				{
					string skipParameterName = "nhsubselectskip";
					var skipParameter = new NamedParameterSpecification(1, 0, skipParameterName) { ExpectedType = NHibernateUtil.Int32 };
					skipSqlParameter = Parameter.Placeholder;
					skipSqlParameter.BackTrack = skipParameter.GetIdsForBackTrack(sessionFactory).First();
					parameters.Add(skipParameterName, new TypedValue(skipParameter.ExpectedType, skip.Value, EntityMode.Poco));
					parameterSpecs.Add(skipParameter);
				}
				if (take.HasValue)
				{
					string takeParameterName = "nhsubselecttake";
					var takeParameter = new NamedParameterSpecification(1, 0, takeParameterName) { ExpectedType = NHibernateUtil.Int32 };
					takeSqlParameter = Parameter.Placeholder;
					takeSqlParameter.BackTrack = takeParameter.GetIdsForBackTrack(sessionFactory).First();
					parameters.Add(takeParameterName, new TypedValue(takeParameter.ExpectedType, take.Value, EntityMode.Poco));
					parameterSpecs.Add(takeParameter);
				}

				// The dialect can move the given parameters where he need, what it can't do is generates new parameters loosing the BackTrack.
				SqlString result;
				if (TryGetLimitString(dialect, subquery, skip, take, skipSqlParameter, takeSqlParameter, out result)) return result;
			}
			return subquery;
		}
	}
}