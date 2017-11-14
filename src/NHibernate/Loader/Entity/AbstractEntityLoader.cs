using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Param;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;

namespace NHibernate.Loader.Entity
{
	/// <summary>
	/// Abstract superclass for entity loaders that use outer joins
	/// </summary>
	public abstract partial class AbstractEntityLoader : OuterJoinLoader, IUniqueEntityLoader
	{
		protected static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof (AbstractEntityLoader));
		protected readonly IOuterJoinLoadable persister;
		protected readonly string entityName;
		private IParameterSpecification[] parametersSpecifications;

		protected AbstractEntityLoader(IOuterJoinLoadable persister, IType uniqueKeyType, ISessionFactoryImplementor factory,
									IDictionary<string, IFilter> enabledFilters) : base(factory, enabledFilters)
		{
			UniqueKeyType = uniqueKeyType;
			entityName = persister.EntityName;
			this.persister = persister;
		}

		protected override bool IsSingleRowLoader
		{
			get { return true; }
		}

		public object Load(object id, object optionalObject, ISessionImplementor session)
		{
			return Load(session, id, optionalObject, id);
		}

		protected virtual object Load(ISessionImplementor session, object id, object optionalObject, object optionalId)
		{
			IList list = LoadEntity(session, id, UniqueKeyType, optionalObject, entityName, optionalId, persister);

			if (list.Count == 1)
			{
				return list[0];
			}
			else if (list.Count == 0)
			{
				return null;
			}
			else
			{
				if (CollectionOwners != null)
				{
					return list[0];
				}
				else
				{
					throw new HibernateException(
						string.Format("More than one row with the given identifier was found: {0}, for class: {1}", id,
									  persister.EntityName));
				}
			}
		}

		protected override object GetResultColumnOrRow(object[] row, IResultTransformer resultTransformer, DbDataReader rs,
													   ISessionImplementor session)
		{
			return row[row.Length - 1];
		}

		protected IType UniqueKeyType { get; private set; }

		private IEnumerable<IParameterSpecification> CreateParameterSpecificationsAndAssignBackTrack(IEnumerable<Parameter> sqlPatameters)
		{
			var specifications = new List<IParameterSpecification>();
			int position = 0;
			var parameters = sqlPatameters.ToArray();
			for (var sqlParameterPos = 0; sqlParameterPos < parameters.Length;)
			{
				var specification = new PositionalParameterSpecification(1, 0, position++) {ExpectedType = UniqueKeyType};
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
	}
}