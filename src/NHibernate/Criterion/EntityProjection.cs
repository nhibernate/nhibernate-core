﻿using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Loader.Criteria;
using NHibernate.Persister.Entity;
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
		/// Fetch all lazy properties
		/// </summary>
		public bool FetchLazyProperties { get; set; }

		/// <summary>
		/// Fetch individual lazy properties or property groups
		/// Note: To fetch single property it must be mapped with unique fetch group (lazy-group)
		/// </summary>
		public ICollection<string> FetchLazyPropertyGroups { get; set; }

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
		/// Fetch all lazy properties
		/// </summary>
		public EntityProjection SetFetchLazyProperties(bool fetchLazyProperties = true)
		{
			FetchLazyProperties = fetchLazyProperties;
			return this;
		}

		/// <summary>
		/// Fetch individual lazy properties or property groups
		/// Note: To fetch single property it must be mapped with unique fetch group (lazy-group)
		/// </summary>
		public EntityProjection SetFetchLazyPropertyGroups(params string[] lazyPropertyGroups)
		{
			FetchLazyPropertyGroups = lazyPropertyGroups;
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
						GetPropertySelectFragment()));
		}

		private string GetPropertySelectFragment()
		{
			return FetchLazyProperties
				? Persister.PropertySelectFragment(TableAlias, ColumnAliasSuffix, FetchLazyProperties)
				: Persister.PropertySelectFragment(TableAlias, ColumnAliasSuffix, FetchLazyPropertyGroups);
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
				throw new ArgumentException(
					$"Projecting to entities requires a '{criteriaQuery.GetType().FullName}' type to implement " +
					$"{nameof(ISupportEntityProjectionCriteriaQuery)} interface.",
					nameof(criteriaQuery));
			}

			var criteria = entityProjectionQuery.RootCriteria;

			if (!Lazy)
			{
				entityProjectionQuery.RegisterEntityProjection(this);
			}

			if (_entityAlias == null)
			{
				_entityAlias = criteria.Alias;
			}

			ICriteria subcriteria =
				criteria.GetCriteriaByAlias(_entityAlias)
				?? throw new HibernateException($"Criteria\\QueryOver alias '{_entityAlias}' for entity projection is not found.");

			var entityName =
				criteriaQuery.GetEntityName(subcriteria)
				?? throw new HibernateException($"Criteria\\QueryOver alias '{_entityAlias}' is not associated with an entity.");

			Persister =
				criteriaQuery.Factory.GetEntityPersister(entityName) as IQueryable
				?? throw new HibernateException($"Projecting to entities requires a '{typeof(IQueryable).FullName}' persister, '{entityName}' does not have one.");

			if (_entityType == null)
			{
				_entityType = Persister.Type.ReturnedClass;
			}

			TableAlias = criteriaQuery.GetSQLAlias(
				subcriteria,
				Persister.IdentifierPropertyName ?? string.Empty);

			ColumnAliasSuffix = BasicLoader.GenerateSuffix(criteriaQuery.GetIndexForAlias());

			_identifierColumnAliases = Persister.GetIdentifierAliases(ColumnAliasSuffix);

			_types = new IType[] { TypeFactory.ManyToOne(Persister.EntityName, true) };
		}
	}
}
