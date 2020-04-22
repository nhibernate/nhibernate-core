using System;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Criterion
{
	/// <summary> 
	/// This class is semi-deprecated. Use <see cref="Restrictions"/>. 
	/// </summary>
	/// <seealso cref="Restrictions"/>
	public sealed class Expression : Restrictions
	{
		private Expression()
		{
			// can not be instantiated
		}

		/// <summary>
		/// Apply a constraint expressed in SQL, with the given SQL parameters
		/// The string {alias} will be replaced by the alias of the root entity.
		/// Criteria aliases can also be used: "{a}.Value + {bc}.Value". Such aliases need to be registered via call to AddCriteriaAliases("a", "bc")
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public static SQLCriterion Sql(SqlString sql, object[] values, IType[] types)
		{
			return new SQLCriterion(sql, values, types);
		}

		/// <summary>
		/// Apply a constraint expressed in SQL, with the given SQL parameter
		/// The string {alias} will be replaced by the alias of the root entity.
		/// Criteria aliases can also be used: "{a}.Value + {bc}.Value". Such aliases need to be registered via call to AddCriteriaAliases("a", "bc")
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static SQLCriterion Sql(SqlString sql, object value, IType type)
		{
			return Sql(sql, new object[] { value }, new IType[] { type });
		}

		/// <summary>
		/// Apply a constraint expressed in SQL, with the given SQL parameter
		/// The string {alias} will be replaced by the alias of the root entity.
		/// Criteria aliases can also be used: "{a}.Value + {bc}.Value". Such aliases need to be registered via call to AddCriteriaAliases("a", "bc")
		/// </summary>
		public static SQLCriterion Sql(string sql, object value, IType type)
		{
			return Sql(sql, new object[] { value }, new IType[] { type });
		}

		/// <summary>
		/// Apply a constraint expressed in SQL, with the given SQL parameter
		/// The string {alias} will be replaced by the alias of the root entity.
		/// Criteria aliases can also be used: "{a}.Value + {bc}.Value". Such aliases need to be registered via call to AddCriteriaAliases("a", "bc")
		/// </summary>
		public static SQLCriterion Sql(string sql, object[] values, IType[] types)
		{
			return new SQLCriterion(SqlString.Parse(sql), values, types);
		}

		/// <summary>
		/// Apply a constraint expressed in SQL
		/// The string {alias} will be replaced by the alias of the root entity.
		/// Criteria aliases can also be used: "{a}.Value + {bc}.Value". Such aliases need to be registered via call to AddCriteriaAliases("a", "bc")
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static SQLCriterion Sql(SqlString sql)
		{
			return Sql(sql, Array.Empty<object>(), TypeHelper.EmptyTypeArray);
		}

		/// <summary>
		/// Apply a constraint expressed in SQL
		/// The string {alias} will be replaced by the alias of the root entity.
		/// Criteria aliases can also be used: "{a}.Value + {bc}.Value". Such aliases need to be registered via call to AddCriteriaAliases("a", "bc")
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static SQLCriterion Sql(string sql)
		{
			return Sql(sql, Array.Empty<object>(), TypeHelper.EmptyTypeArray);
		}
	}
}
