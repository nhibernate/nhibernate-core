using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Persister.Entity
{
	internal class SubqueryPropertyMapping : IPropertyMapping
	{
		private readonly Dictionary<string, string[]> _propertyColumns = new Dictionary<string, string[]>();
		private readonly Dictionary<string, IType> _propertyTypes = new Dictionary<string, IType>();
		private readonly Dictionary<string, IPropertyMapping> _propertyAliasMappings = new Dictionary<string, IPropertyMapping>();
		private readonly Dictionary<string, string> _propertyMappingSuffixes = new Dictionary<string, string>();
		private readonly Dictionary<IPropertyMapping, string> _propertyMappings = new Dictionary<IPropertyMapping, string>();
		private readonly Dictionary<string, ISelectExpression> _aliasSelectExpressions = new Dictionary<string, ISelectExpression>();
		private readonly HashSet<FromElement> _nonScalarFromElements = new HashSet<FromElement>();
		private readonly SelectClause _selectClause;

		public SubqueryPropertyMapping(IType type, SelectClause selectClause)
		{
			_selectClause = selectClause;
			// We need unflattened expressions to correctly process nested aliases
			var selectExpressions = selectClause.OriginalSelectExpressions;
			var nonScalarExpressions = selectClause.NonScalarExpressions;
			Type = type;
			if (selectClause.IsScalarSelect)
			{
				for (var i = 0; i < selectExpressions.Count; i++)
				{
					var scalarExpression = selectExpressions[i];
					var key = scalarExpression.Alias;
					if (string.IsNullOrEmpty(key) && scalarExpression is DotNode dotNode)
					{
						key = dotNode.PropertyPath.Split(StringHelper.Dot).Last();
					}

					if (key == null)
					{
						continue;
					}

					if (_aliasSelectExpressions.ContainsKey(key))
					{
						throw new QueryException($"Subquery contains duplicated property '{key}', use an alias with a different name.");
					}

					_aliasSelectExpressions.Add(key, scalarExpression);
					_propertyColumns.Add(key, selectClause.GetScalarColumns(scalarExpression));
					_propertyTypes.Add(key, scalarExpression.DataType);
				}
			}

			for (var i = 0; i < nonScalarExpressions.Count; i++)
			{
				var expression = nonScalarExpressions[i];
				var fromElement = expression.FromElement;
				if (fromElement == null)
				{
					continue;
				}

				if (!fromElement.IsFetch)
				{
					_nonScalarFromElements.Add(fromElement);
				}

				var mapping = fromElement.GetPropertyMapping("");
				if (mapping == null)
				{
					continue;
				}

				var alias = expression.Alias;
				if (!string.IsNullOrEmpty(alias))
				{
					_propertyAliasMappings.Add(alias, mapping);
					_propertyMappingSuffixes.Add(alias, fromElement.EntitySuffix ?? fromElement.CollectionSuffix);
					if (!_aliasSelectExpressions.ContainsKey(alias))
					{
						_aliasSelectExpressions.Add(alias, expression);
					}
				}
				else
				{
					if (_propertyMappings.ContainsKey(mapping))
					{
						throw new QueryException($"Subquery selects the same type '{mapping.Type}' multiple times. Use unique alias for each type selection.");
					}

					_propertyMappings.Add(mapping, fromElement.EntitySuffix ?? fromElement.CollectionSuffix);
				}
			}
		}

		public IType Type { get; }

		public List<ISelectExpression> GetRelatedSelectExpressions(string path, out SelectClause selectClause)
		{
			var paths = path.Split(StringHelper.Dot);
			var alias = paths[0];
			if (!_aliasSelectExpressions.TryGetValue(alias, out var expression))
			{
				selectClause = null;
				return null;
			}

			if (paths.Length == 1)
			{
				selectClause = _selectClause;
				var replacements = _selectClause.GetReplacedExpressions(expression);
				if (replacements != null)
				{
					return replacements;
				}

				return new List<ISelectExpression> {expression};
			}

			if (_propertyAliasMappings.TryGetValue(alias, out var mapping) &&
				mapping is SubqueryPropertyMapping joinSubQueryMapping)
			{
				return joinSubQueryMapping.GetRelatedSelectExpressions(JoinPaths(paths.Skip(1)), out selectClause);
			}

			selectClause = null;
			return null;
		}

		public bool ContainsEntityAlias(string alias, IType type)
		{
			var aliases = alias.Split(StringHelper.Dot);
			var rootAlias = aliases[0];
			if (!_propertyAliasMappings.TryGetValue(rootAlias, out var mapping))
			{
				return false;
			}

			if (aliases.Length == 1)
			{
				return mapping.Type.Equals(type);
			}

			if (mapping is SubqueryPropertyMapping joinSubQueryMapping)
			{
				return joinSubQueryMapping.ContainsEntityAlias(JoinPaths(aliases.Skip(1)), type);
			}

			return false;
		}

		public IType ToType(string propertyName)
		{
			if (!TryToType(propertyName, out var type))
			{
				throw new QueryException(string.Format("could not resolve property: {0} of: {1}", propertyName, "SubQuery"));
			}

			return type;
		}

		public bool TryToType(string propertyName, out IType type)
		{
			var paths = propertyName.Split(StringHelper.Dot);
			var propertyAlias = paths[0];
			if (paths.Length == 1 && _propertyTypes.TryGetValue(propertyAlias, out type))
			{
				return true;
			}

			if (_propertyAliasMappings.TryGetValue(propertyAlias, out var mapping))
			{
				if (paths.Length > 1)
				{
					return mapping.TryToType(JoinPaths(paths.Skip(1)), out type);
				}

				type = mapping.Type;
				return type != null;
			}

			foreach (var propertyMapping in _propertyMappings.Keys)
			{
				if (propertyMapping.TryToType(propertyName, out type))
				{
					return true;
				}
			}

			type = null;
			return false;
		}

		public string[] ToColumns(string alias, string propertyName)
		{
			if (TryGetColumns(alias, propertyName, out var columns))
			{
				return columns;
			}

			throw new QueryException(string.Format("could not resolve property: {0} of: {1}", propertyName, "SubQuery"));
		}

		public List<string> GetPropertiesColumns(string alias)
		{
			var columns = new List<string>();
			foreach (var fromElement in _nonScalarFromElements)
			{
				var fragment = fromElement.GetPropertiesSelectFragment(fromElement.EntitySuffix, alias);
				if (fromElement is JoinSubqueryFromElement)
				{
					columns.AddRange(fragment.GetColumnAliases());
				}
				else
				{
					columns.AddRange(fragment.GetColumnAliases().Select(o => StringHelper.Qualify(alias, o)));
				}
			}

			if (_selectClause.ColumnNames != null)
			{
				foreach (var scalarColumns in _selectClause.ColumnNames)
				{
					columns.AddRange(QualifyColumns(scalarColumns, alias));
				}
			}

			return columns;
		}

		public List<string> GetIdentifiersColumns(string alias)
		{
			var columns = new List<string>();
			foreach (var fromElement in _nonScalarFromElements)
			{
				if (fromElement.FromClause.IsScalarSubQuery)
				{
					continue;
				}

				var fragment = fromElement.GetIdentifierSelectFragment(fromElement.EntitySuffix, alias);
				if (fragment == null)
				{
					continue; // Subquery with scalar select
				}

				if (fromElement is JoinSubqueryFromElement)
				{
					columns.AddRange(fragment.GetColumnAliases());
				}
				else
				{
					columns.AddRange(fragment.GetColumnAliases().Select(o => StringHelper.Qualify(alias, o)));
				}
			}

			return columns;
		}

		private bool TryGetColumns(string alias, string propertyName, out string[] columns)
		{
			var paths = propertyName.Split(StringHelper.Dot);
			var propertyAlias = paths[0];
			if (paths.Length == 1 && _propertyColumns.TryGetValue(propertyAlias, out columns))
			{
				columns = QualifyColumns(columns, alias);
				return true;
			}

			if (_propertyAliasMappings.TryGetValue(propertyAlias, out var mapping))
			{
				var suffix = _propertyMappingSuffixes[propertyAlias];
				if (
					(paths.Length > 1 && TryGetQualifiedColumns(mapping, JoinPaths(paths.Skip(1)), alias, suffix, out columns)) ||
					(paths.Length == 1 && TryGetQualifiedColumns(mapping, EntityPersister.EntityID, alias, suffix, out columns))
				)
				{
					return true;
				}
			}

			if (propertyName == EntityPersister.EntityID)
			{
				columns = GetIdentifiersColumns(alias).ToArray();
				return columns.Length > 0;
			}

			foreach (var pair in _propertyMappings)
			{
				if (pair.Key.TryToType(propertyName, out _) && TryGetQualifiedColumns(pair.Key, propertyName, alias, pair.Value, out columns))
				{
					return true;
				}
			}

			columns = null;
			return false; 
		}

		private bool TryGetQualifiedColumns(IPropertyMapping propertyMapping, string propertyName, string alias, string suffix, out string[] columns)
		{
			columns = null;
			if (propertyMapping is ISqlLoadable loadable)
			{
				var aliasedColumns = loadable.GetSubclassPropertyColumnAliases(propertyName, suffix);
				if (aliasedColumns != null)
				{
					columns = QualifyColumns(aliasedColumns, alias);
				}
			}
			else if (propertyMapping is SubqueryPropertyMapping subQueryMapping && subQueryMapping.TryGetColumns(alias, propertyName, out columns))
			{
				return true;
			}

			return columns != null;
		}

		private static string[] QualifyColumns(string[] columns, string alias)
		{
			var result = new string[columns.Length];
			for (int i = 0; i < columns.Length; i++)
			{
				result[i] = StringHelper.Qualify(alias, columns[i]);
			}

			return result;
		}

		private static string JoinPaths(IEnumerable<string> paths)
		{
#if NETCOREAPP
			return string.Join(StringHelper.Dot, paths);
#else
			return string.Join(".", paths);
#endif
		}

		public string[] ToColumns(string propertyName)
		{
			throw new NotSupportedException();
		}
	}
}
