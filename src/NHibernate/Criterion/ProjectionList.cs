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
				loc += proj.GetColumnAliases(loc, criteria, criteriaQuery).Length;
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

		public string[] GetColumnAliases(int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
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
