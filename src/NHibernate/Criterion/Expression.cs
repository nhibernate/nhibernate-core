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
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public static AbstractCriterion Sql(SqlString sql, object[] values, IType[] types)
		{
			return new SQLCriterion(sql, values, types);
		}

		/// <summary>
		/// Apply a constraint expressed in SQL, with the given SQL parameter
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static AbstractCriterion Sql(SqlString sql, object value, IType type)
		{
			return Sql(sql, new object[] { value }, new IType[] { type });
		}

		/// <summary>
		/// Apply a constraint expressed in SQL, with the given SQL parameter
		/// </summary>
		public static AbstractCriterion Sql(string sql, object value, IType type)
		{
			return Sql(sql, new object[] { value }, new IType[] { type });
		}

		public static AbstractCriterion Sql(string sql, object[] values, IType[] types)
		{
			return new SQLCriterion(SqlString.Parse(sql), values, types);
		}

		/// <summary>
		/// Apply a constraint expressed in SQL
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static AbstractCriterion Sql(SqlString sql)
		{
			return Sql(sql, ArrayHelper.EmptyObjectArray, ArrayHelper.EmptyTypeArray);
		}

		/// <summary>
		/// Apply a constraint expressed in SQL
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static AbstractCriterion Sql(string sql)
		{
			return Sql(sql, ArrayHelper.EmptyObjectArray, ArrayHelper.EmptyTypeArray);
		}
	}
}
