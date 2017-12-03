﻿using System;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using IQueryable = NHibernate.Persister.Entity.IQueryable;

namespace NHibernate.Criterion
{
	using TThis = EntityProjection;

	[Serializable]
	public class EntityProjection : IProjection
	{
		private string _entityAlias;
		private string _columnAliasSuffix;
		private string _tableAlias;
		private IType[] _types;
		private string[] _columnAliases;

		/// <summary>
		/// Root entity projection
		/// </summary>
		public EntityProjection() : this(null, null)
		{
		}

		public EntityProjection(System.Type rootEntity, string entityAlias)
		{
			RootEntity = rootEntity;
			_entityAlias = entityAlias;
		}

		public bool FetchLazyProperties { get; set; }
		public bool IsReadOnly { get; set; }
		public bool Lazy { get; set; }

		internal string[] IdentifierColumnAliases { get; private set; }
		internal string[][] PropertyColumnAliases { get; private set; }
		internal IQueryable Persister { get; private set; }
		internal System.Type RootEntity { get; private set; }

		#region Configuration methods

		public TThis SetLazy(bool lazy = true)
		{
			Lazy = lazy;
			return this;
		}

		public TThis SetAllPropertyFetch(bool fetchLazyProperties = true)
		{
			FetchLazyProperties = fetchLazyProperties;
			return this;
		}

		public TThis SetReadonly(bool isReadOnly = true)
		{
			IsReadOnly = isReadOnly;
			return this;
		}

		#endregion Configuration methods

		#region IProjection implementation

		/// <summary>
		/// Entity alias
		/// </summary>
		public string[] Aliases
		{
			get { return new[] {_entityAlias}; }
		}

		bool IProjection.IsAggregate
		{
			get { return false; }
		}

		bool IProjection.IsGrouped
		{
			get { return false; }
		}

		IType[] IProjection.GetTypes(string alias, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new[] {criteriaQuery.GetType(criteria, alias)};
		}

		IType[] IProjection.GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			SetFields(criteria, criteriaQuery);

			return _types;
		}

		public string[] GetColumnAliases(int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			SetFields(criteria, criteriaQuery);

			return _columnAliases;
		}

		public string[] GetColumnAliases(string alias, int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			SetFields(criteria, criteriaQuery);

			return _columnAliases;
		}

		SqlString IProjection.ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery)
		{
			SetFields(criteria, criteriaQuery);

			string identifierSelectFragment = Persister.IdentifierSelectFragment(_tableAlias, _columnAliasSuffix);
			return new SqlString(
				Lazy
					? identifierSelectFragment
					: string.Concat(
						identifierSelectFragment,
						Persister.PropertySelectFragment(_tableAlias, _columnAliasSuffix, FetchLazyProperties)));
		}

		SqlString IProjection.ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			throw new NotImplementedException();
		}

		TypedValue[] IProjection.GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			throw new NotImplementedException();
		}

		#endregion IProjection implementation

		private void SetFields(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			//Persister is required, so let's use it as "initialized marker"
			if (Persister != null)
				return;

			if (RootEntity == null)
			{
				RootEntity = criteria.GetRootEntityTypeIfAvailable();
			}

			if (_entityAlias == null)
			{
				_entityAlias = criteria.Alias;
			}

			Persister = (IQueryable) criteriaQuery.Factory.GetEntityPersister(RootEntity.FullName);

			ICriteria subcriteria = criteria.GetCriteriaByAlias(_entityAlias);
			if (subcriteria == null)
				throw new HibernateException($"Criteria\\QueryOver alias '{_entityAlias}' for entity projection is not found.");

			_tableAlias = criteriaQuery.GetSQLAlias(
				subcriteria,
				Persister.IdentifierPropertyName ?? string.Empty);

			_columnAliasSuffix = criteriaQuery.GetIndexForAlias().ToString();

			IdentifierColumnAliases = Persister.GetIdentifierAliases(_columnAliasSuffix);

			PropertyColumnAliases = Lazy
				? new string[][] { }
				: Enumerable.Range(0, Persister.PropertyNames.Length).Select(i => Persister.GetPropertyAliases(_columnAliasSuffix, i)).ToArray();

			_columnAliases = IdentifierColumnAliases.Concat(PropertyColumnAliases.SelectMany(ca => ca)).ToArray();

			_types = new IType[] {new EntityProjectionType(this)};
		}
	}
}
