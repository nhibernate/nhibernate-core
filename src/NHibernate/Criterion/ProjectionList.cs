using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	[Serializable]
	public class ProjectionList : IProjection
	{
		private IList<IProjection> elements = new List<IProjection>();

		private IType[] _typesCache;
		private readonly Dictionary<int, string[]> _columnPositionAliasesCache;
		private readonly Dictionary<string, string[]> _columnAliasesCache;
		private readonly Dictionary<string, IType[]> _typesAliasCache;
		private readonly bool _enableMappingCaching;

		protected internal ProjectionList() : this(false)
		{
		}

		protected internal ProjectionList(bool enableMappingCaching)
		{
			_enableMappingCaching = enableMappingCaching;

			if (!_enableMappingCaching)
				return;

			_columnPositionAliasesCache = new Dictionary<int, string[]>();
			_columnAliasesCache = new Dictionary<string, string[]>();
			_typesAliasCache = new Dictionary<string, IType[]>();
		}

		public ProjectionList Create()
		{
			return Create(false);
		}

		public ProjectionList Create(bool enableMappingCaching)
		{
			return new ProjectionList(enableMappingCaching);
		}

		public ProjectionList Add(IProjection proj)
		{
			elements.Add(proj);

			if (!_enableMappingCaching)
				return this;

			_typesCache = null;
			_columnPositionAliasesCache.Clear();
			_columnAliasesCache.Clear();
			_typesAliasCache.Clear();
			return this;
		}

		public ProjectionList Add(IProjection projection, String alias)
		{
			return Add(Projections.Alias(projection, alias));
		}

		public ProjectionList Add<T>(IProjection projection, Expression<Func<T>> alias)
		{
			return Add(projection, ExpressionProcessor.FindMemberExpression(alias.Body));
		}


		public IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			if (!_enableMappingCaching)
				return GetTypesInternal(criteria, criteriaQuery);
			return _typesCache ?? (_typesCache = GetTypesInternal(criteria, criteriaQuery));
		}

		private IType[] GetTypesInternal(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			IList<IType> types = new List<IType>(Length);

			for (int i = 0; i < Length; i++)
			{
				IType[] elemTypes = this[i].GetTypes(criteria, criteriaQuery);
				foreach (IType elemType in elemTypes)
					types.Add(elemType);
			}
			IType[] result = new IType[types.Count];
			types.CopyTo(result, 0);
			return result;
		}

		public SqlString ToSqlString(ICriteria criteria, int loc, ICriteriaQuery criteriaQuery)
		{
			SqlStringBuilder buf = new SqlStringBuilder();
			for (int i = 0; i < Length; i++)
			{
				IProjection proj = this[i];
				buf.Add(proj.ToSqlString(criteria, loc, criteriaQuery));
				loc += proj.GetColumnAliases(loc, criteria, criteriaQuery).Length;
				if (i < elements.Count - 1)
				{
					buf.Add(", ");
				}
			}
			return buf.ToSqlString();
		}

		public SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			SqlStringBuilder buf = new SqlStringBuilder();
			for (int i = 0; i < Length; i++)
			{
				IProjection proj = this[i];
				if (proj.IsGrouped)
				{
					buf.Add(proj.ToGroupSqlString(criteria, criteriaQuery))
					   .Add(", ");
				}
			}
			if (buf.Count >= 2)
			{
				buf.RemoveAt(buf.Count - 1);
			}
			return buf.ToSqlString();
		}

		public string[] GetColumnAliases(int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			if (!_enableMappingCaching)
				return GetColumnAliasesInternal(position, criteria, criteriaQuery);

			if (!_columnPositionAliasesCache.TryGetValue(position, out var value))
			{
				value = GetColumnAliasesInternal(position, criteria, criteriaQuery);
				_columnPositionAliasesCache.Add(position, value);
			}
			return value;
		}

		public string[] GetColumnAliasesInternal(int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var result = new List<string>(Length);
			for (var i = 0; i < Length; i++)
			{
				var colAliases = this[i].GetColumnAliases(position, criteria, criteriaQuery);
				result.AddRange(colAliases);
				position += colAliases.Length;
			}
			return result.ToArray();
		}

		public string[] GetColumnAliases(string alias, int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			if (!_enableMappingCaching)
				return GetColumnAliasesInternal(alias, position, criteria, criteriaQuery);

			if (!_columnAliasesCache.TryGetValue(alias, out var value))
			{
				value = GetColumnAliasesInternal(alias, position, criteria, criteriaQuery);
				_columnAliasesCache.Add(alias, value);
			}
			return value;
		}

		private string[] GetColumnAliasesInternal(
			string alias,
			int position,
			ICriteria criteria,
			ICriteriaQuery criteriaQuery)
		{
			for (int i = 0; i < Length; i++)
			{
				string[] result = this[i].GetColumnAliases(alias, position, criteria, criteriaQuery);
				if (result != null) return result;
				position += this[i].GetColumnAliases(position, criteria, criteriaQuery).Length;
			}
			return null;
		}


		public IType[] GetTypes(string alias, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			if (!_enableMappingCaching)
				return GetTypesInternal(alias, criteria, criteriaQuery);

			if (!_typesAliasCache.TryGetValue(alias, out var value))
			{
				value = GetTypesInternal(alias, criteria, criteriaQuery);
				_typesAliasCache.Add(alias, value);
			}
			return value;
		}

		public IType[] GetTypesInternal(string alias, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			for (int i = 0; i < Length; i++)
			{
				IType[] result = this[i].GetTypes(alias, criteria, criteriaQuery);
				if (result != null) return result;
			}
			return null;
		}

		public String[] Aliases
		{
			get
			{
				IList<string> aliasList = new List<string>(Length);

				for (int i = 0; i < Length; i++)
				{
					String[] aliases = this[i].Aliases;
					foreach (string alias in aliases)
						aliasList.Add(alias);
				}
				string[] result = new string[aliasList.Count];
				aliasList.CopyTo(result, 0);
				return result;
			}
		}

		public IProjection this[int index]
		{
			get { return elements[index]; }
		}

		public int Length
		{
			get { return elements.Count; }
		}

		public override String ToString()
		{
			return elements.ToString();
		}

		public bool IsGrouped
		{
			get
			{
				for (int i = 0; i < Length; i++)
				{
					if (this[i].IsGrouped)
						return true;
				}
				return false;
			}
		}

		public bool IsAggregate
		{
			get
			{
				for (int i = 0; i < Length; i++)
				{
					if (this[i].IsAggregate)
						return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Gets the typed values for parameters in this projection
		/// </summary>
		/// <param name="criteria">The criteria.</param>
		/// <param name="criteriaQuery">The criteria query.</param>
		/// <returns></returns>
		public TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			List<TypedValue> values = new List<TypedValue>();
			foreach (IProjection element in elements)
			{
				values.AddRange(element.GetTypedValues(criteria, criteriaQuery));
			}
			return values.ToArray();
		}
	}
}
