using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader
{
	public sealed class OuterJoinableAssociation
	{
		private readonly IAssociationType joinableType;
		private readonly IJoinable joinable;
		private readonly string lhsAlias; // belong to other persister
		private readonly string[] lhsColumns; // belong to other persister
		private readonly string rhsAlias;
		private readonly string[] rhsColumns;
		private readonly JoinType joinType;
		private readonly SqlString on;
		private readonly IDictionary<string, IFilter> enabledFilters;
		private readonly SelectMode _selectMode;

		public OuterJoinableAssociation(
			IAssociationType joinableType,
			String lhsAlias,
			String[] lhsColumns,
			String rhsAlias,
			JoinType joinType,
			SqlString withClause,
			ISessionFactoryImplementor factory,
			IDictionary<string, IFilter> enabledFilters,
			SelectMode selectMode) : this(joinableType, lhsAlias, lhsColumns, rhsAlias, joinType, withClause, factory, enabledFilters)
		{
			_selectMode = selectMode;
		}

		public OuterJoinableAssociation(IAssociationType joinableType, String lhsAlias, String[] lhsColumns, String rhsAlias,
										JoinType joinType, SqlString withClause, ISessionFactoryImplementor factory,
										IDictionary<string, IFilter> enabledFilters)
		{
			this.joinableType = joinableType;
			this.lhsAlias = lhsAlias;
			this.lhsColumns = lhsColumns;
			this.rhsAlias = rhsAlias;
			this.joinType = joinType;
			joinable = joinableType.GetAssociatedJoinable(factory);
			rhsColumns = JoinHelper.GetRHSColumnNames(joinableType, factory);
			on = new SqlString(joinableType.GetOnCondition(rhsAlias, factory, enabledFilters));
			if (SqlStringHelper.IsNotEmpty(withClause))
				on = on.Append(" and ( ").Append(withClause).Append(" )");
			this.enabledFilters = enabledFilters; // needed later for many-to-many/filter application
		}

		public JoinType JoinType
		{
			get { return joinType; }
		}

		public string RHSAlias
		{
			get { return rhsAlias; }
		}

		public SqlString On
		{
			get { return on; }
		}

		private bool IsEntityType
		{
			get { return joinableType.IsEntityType; }
		}

		public IAssociationType JoinableType
		{
			get { return joinableType; }
		}

		public string RHSUniqueKeyName
		{
			get { return joinableType.RHSUniqueKeyPropertyName; }
		}

		public bool IsCollection
		{
			get { return joinableType.IsCollectionType; }
		}

		public IJoinable Joinable
		{
			get { return joinable; }
		}

		public SelectMode SelectMode
		{
			get { return _selectMode; }
		}

		public int GetOwner(IList<OuterJoinableAssociation> associations)
		{
			if (IsEntityType || IsCollection)
			{
				return GetPosition(lhsAlias, associations);
			}
			else
			{
				return -1;
			}
		}

		/// <summary>
		/// Get the position of the join with the given alias in the
		/// list of joins
		/// </summary>
		private static int GetPosition(string lhsAlias, IEnumerable<OuterJoinableAssociation> associations)
		{
			int result = 0;
			foreach (OuterJoinableAssociation oj in associations)
			{
				if (oj.Joinable.ConsumesEntityAlias())
				{
					if (oj.rhsAlias.Equals(lhsAlias))
					{
						return result;
					}

					result++;
				}
			}
			return -1;
		}

		public void AddJoins(JoinFragment outerjoin)
		{
			outerjoin.AddJoin(joinable.TableName, rhsAlias, lhsColumns, rhsColumns, joinType, on);
			outerjoin.AddJoins(joinable.FromJoinFragment(rhsAlias, false, true),
							   joinable.WhereJoinFragment(rhsAlias, false, true));
		}

		public void ValidateJoin(string path)
		{
			if (rhsColumns == null || lhsColumns == null || lhsColumns.Length != rhsColumns.Length || lhsColumns.Length == 0)
			{
				throw new MappingException("invalid join columns for association: " + path);
			}
		}

		public bool IsManyToManyWith(OuterJoinableAssociation other)
		{
			if (joinable.IsCollection)
			{
				IQueryableCollection persister = (IQueryableCollection) joinable;
				if (persister.IsManyToMany)
				{
					return persister.ElementType == other.JoinableType;
				}
			}
			return false;
		}

		public void AddManyToManyJoin(JoinFragment outerjoin, IQueryableCollection collection)
		{
			string manyToManyFilter = collection.GetManyToManyFilterFragment(rhsAlias, enabledFilters);
			SqlString condition = string.Empty.Equals(manyToManyFilter)
								? on
								: SqlStringHelper.IsEmpty(on) ? new SqlString(manyToManyFilter) : 
									on.Append(" and ").Append(manyToManyFilter);

			outerjoin.AddJoin(joinable.TableName, rhsAlias, lhsColumns, rhsColumns, joinType, condition);
			outerjoin.AddJoins(joinable.FromJoinFragment(rhsAlias, false, true),
							   joinable.WhereJoinFragment(rhsAlias, false, true));
		}

		internal bool ShouldFetchCollectionPersister()
		{
			if (!Joinable.IsCollection)
				return false;

			switch (SelectMode)
			{
				case SelectMode.Undefined:
					return JoinType == JoinType.LeftOuterJoin;

				case SelectMode.Fetch:
				case SelectMode.FetchLazyProperties:
					return true;

				case SelectMode.ChildFetch:
				case SelectMode.JoinOnly:
					return false;
			}

			throw new ArgumentOutOfRangeException(nameof(SelectMode), SelectMode.ToString());
		}
	}
}
