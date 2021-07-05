using NHibernate.Util;

namespace NHibernate.Mapping.ByCode
{
	//6.0 TODO: merge into IEntitySqlsMapper
	public interface IEntitySqlsWithCheckMapper
	{
		void SqlInsert(string sql, SqlCheck sqlCheck);
		void SqlUpdate(string sql, SqlCheck sqlCheck);
		void SqlDelete(string sql, SqlCheck sqlCheck);
	}

	public static class EntitySqlsWithCheckMapperExtensions
	{
		public static void SqlInsert(this IEntitySqlsMapper mapper, string sql, SqlCheck sqlCheck)
		{
			ReflectHelper
				.CastOrThrow<IEntitySqlsWithCheckMapper>(mapper, "SqlInsert with sqlCheck")
				.SqlInsert(sql, sqlCheck);
		}

		public static void SqlUpdate(this IEntitySqlsMapper mapper, string sql, SqlCheck sqlCheck)
		{
			ReflectHelper
				.CastOrThrow<IEntitySqlsWithCheckMapper>(mapper, "SqlUpdate with sqlCheck")
				.SqlUpdate(sql, sqlCheck);
		}

		public static void SqlDelete(this IEntitySqlsMapper mapper, string sql, SqlCheck sqlCheck)
		{
			ReflectHelper
				.CastOrThrow<IEntitySqlsWithCheckMapper>(mapper, "SqlDelete with sqlCheck")
				.SqlDelete(sql, sqlCheck);
		}
	}
}
