﻿using System;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Loader.Criteria;
using NHibernate.SqlCommand;
using NHibernate.Type;
using IQueryable = NHibernate.Persister.Entity.IQueryable;

namespace NHibernate.Criterion
{
	/// <summary>
	/// Entity projection
	/// </summary>
	[Serializable]
	public class EntityProjection : IProjection
	{
		private string _entityAlias;
		private System.Type _entityType;
		private IType[] _types;
		private string[] _identifierColumnAliases;

		/// <summary>
		/// Root entity projection
		/// </summary>
		public EntityProjection() : this(null, null)
		{
		}

		/// <summary>
		/// Entity projection for given type and alias
		/// </summary>
		/// <param name="entityType">Type of entity</param>
		/// <param name="entityAlias">Entity alias</param>
		public EntityProjection(System.Type entityType, string entityAlias)
		{
			_entityType = entityType;
			_entityAlias = entityAlias;
		}

		/// <summary>
		/// Fetch lazy properties
		/// </summary>
		public bool FetchLazyProperties { get; set; }

		/// <summary>
		/// Lazy load entity
		/// </summary>
		public bool Lazy { get; set; }

		internal IQueryable Persister { get; private set; }
		internal string ColumnAliasSuffix { get; private set; }
		internal string TableAlias { get; private set; }

		#region Configuration methods

		/// <summary>
		/// Lazy load entity
		/// </summary>
		public EntityProjection SetLazy(bool lazy = true)
		{
			Lazy = lazy;
			return this;
		}

		/// <summary>
		/// Fetch lazy properties
		/// </summary>
		public EntityProjection SetFetchLazyProperties(bool fetchLazyProperties = true)
		{
			FetchLazyProperties = fetchLazyProperties;
			return this;
		}

		#endregion Configuration methods

		#region IProjection implementation

		string[] IProjection.Aliases => new[] { _entityAlias };

		bool IProjection.IsAggregate => false;

		bool IProjection.IsGrouped => false;

		IType[] IProjection.GetTypes(string alias, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return null;
		}

		string[] IProjection.GetColumnAliases(string alias, int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return null;
		}

		IType[] IProjection.GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			SetFields(criteriaQuery);

			return _types;
		}

		string[] IProjection.GetColumnAliases(int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			SetFields(criteriaQuery);

			return _identifierColumnAliases;
		}

		SqlString IProjection.ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery)
		{
			SetFields(criteriaQuery);

			string identifierSelectFragment = Persister.IdentifierSelectFragment(TableAlias, ColumnAliasSuffix);
			return new SqlString(
				Lazy
					? identifierSelectFragment
					: string.Concat(
						identifierSelectFragment,
						Persister.PropertySelectFragment(TableAlias, ColumnAliasSuffix, FetchLazyProperties)));
		}

		SqlString IProjection.ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			throw new InvalidOperationException("not a grouping projection");
		}

		TypedValue[] IProjection.GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return Array.Empty<TypedValue>();
		}

		#endregion IProjection implementation

		private void SetFields(ICriteriaQuery criteriaQuery)
		{
			//Persister is required, so let's use it as "initialized marker"
			if (Persister != null)
				return;

			if (!(criteriaQuery is ISupportEntityProjectionCriteriaQuery entityProjectionQuery))
			{
				throw new HibernateException($"Projecting to entities requires a '{criteriaQuery.GetType().FullName}' type to implement {nameof(ISupportEntityProjectionCriteriaQuery)} interface.");
			}

			var criteria = entityProjectionQuery.RootCriteria;

			if (!Lazy)
			{
				entityProjectionQuery.RegisterEntityProjection(this);
			}

			if (_entityType == null)
			{
				_entityType = criteria.GetRootEntityTypeIfAvailable();
			}

			if (_entityAlias == null)
			{
				_entityAlias = criteria.Alias;
			}

			Persister = criteriaQuery.Factory.GetEntityPersister(_entityType.FullName) as IQueryable;
			if (Persister == null)
				throw new HibernateException($"Projecting to entities requires a '{typeof(IQueryable).FullName}' persister, '{_entityType.FullName}' does not have one.");

			ICriteria subcriteria = criteria.GetCriteriaByAlias(_entityAlias);
			if (subcriteria == null)
				throw new HibernateException($"Criteria\\QueryOver alias '{_entityAlias}' for entity projection is not found.");

			TableAlias = criteriaQuery.GetSQLAlias(
				subcriteria,
				Persister.IdentifierPropertyName ?? string.Empty);

			ColumnAliasSuffix = BasicLoader.GenerateSuffix(criteriaQuery.GetIndexForAlias());

			_identifierColumnAliases = Persister.GetIdentifierAliases(ColumnAliasSuffix);

			_types = new IType[] { TypeFactory.ManyToOne(Persister.EntityName, true) };
		}
	}
}
