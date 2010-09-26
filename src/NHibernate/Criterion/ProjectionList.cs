using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Criterion
{
	[Serializable]
	public class ProjectionList : IEnhancedProjection
	{
		private IList<IProjection> elements = new List<IProjection>();

		protected internal ProjectionList()
		{
		}

		public ProjectionList Create()
		{
			return new ProjectionList();
		}

		public ProjectionList Add(IProjection proj)
		{
			elements.Add(proj);
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

		public SqlString ToSqlString(ICriteria criteria, int loc, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			SqlStringBuilder buf = new SqlStringBuilder();
			for (int i = 0; i < Length; i++)
			{
				IProjection proj = this[i];
				buf.Add(proj.ToSqlString(criteria, loc, criteriaQuery, enabledFilters));
				loc += GetColumnAliases(loc, criteria, criteriaQuery, proj).Length;
				if (i < elements.Count - 1)
				{
					buf.Add(", ");
				}
			}
			return buf.ToSqlString();
		}

		public SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			SqlStringBuilder buf = new SqlStringBuilder();
			for (int i = 0; i < Length; i++)
			{
				IProjection proj = this[i];
				if (proj.IsGrouped)
				{
					buf.Add(proj.ToGroupSqlString(criteria, criteriaQuery,enabledFilters))
						.Add(", ");
				}
			}
			if (buf.Count >= 2)
			{
				buf.RemoveAt(buf.Count - 1);
			}
			return buf.ToSqlString();
		}

		public string[] GetColumnAliases(int loc)
		{
			IList<string> aliases = new List<string>(Length);

			for (int i = 0; i < Length; i++)
			{
				String[] colAliases = this[i].GetColumnAliases(loc);
				foreach (string alias in colAliases)
					aliases.Add(alias);
				loc += colAliases.Length;
			}
			string[] result = new string[aliases.Count];
			aliases.CopyTo(result, 0);
			return result;
		}

		public string[] GetColumnAliases(string alias, int loc)
		{
			for (int i = 0; i < Length; i++)
			{
				String[] result = this[i].GetColumnAliases(alias, loc);
				if (result != null) return result;
				loc += this[i].GetColumnAliases(loc).Length;
			}
			return null;
		}
		
		public string[] GetColumnAliases(int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			IList result = new ArrayList(this.Length);
			for (int i = 0; i < this.Length; i++)
			{
				string[] colAliases = ProjectionList.GetColumnAliases(position, criteria, criteriaQuery, this[i]);
				ArrayHelper.AddAll(result, colAliases);
				position += colAliases.Length;
			}
			return ArrayHelper.ToStringArray(result);
		}
		
		public string[] GetColumnAliases(string alias, int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			for (int i = 0; i < this.Length; i++)
			{
				string[] result = GetColumnAliases(alias, position, criteria, criteriaQuery, this[i]);
				if (result != null) return result;
				position += GetColumnAliases(position, criteria, criteriaQuery, this[i]).Length;
			}
			return null;
		}
		
		private static string[] GetColumnAliases(int position, ICriteria criteria, ICriteriaQuery criteriaQuery, IProjection projection)
		{
			return projection is IEnhancedProjection
				? ((IEnhancedProjection)projection).GetColumnAliases(position, criteria, criteriaQuery)
				: projection.GetColumnAliases(position);
		}

		private static string[] GetColumnAliases(string alias, int position, ICriteria criteria, ICriteriaQuery criteriaQuery, IProjection projection)
		{
			return projection is IEnhancedProjection
				? ((IEnhancedProjection)projection).GetColumnAliases(alias, position, criteria, criteriaQuery)
				: projection.GetColumnAliases(alias, position);
		}

		public IType[] GetTypes(string alias, ICriteria criteria, ICriteriaQuery criteriaQuery)
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
			get
			{
				return elements[index];
			}
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
				for(int i = 0; i < Length; i++)
				{
					if(this[i].IsAggregate)
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
