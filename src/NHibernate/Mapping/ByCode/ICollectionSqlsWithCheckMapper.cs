using NHibernate.Util;

namespace NHibernate.Mapping.ByCode
{
	//6.0 TODO: merge into ICollectionSqlsMapper
	public interface ICollectionSqlsWithCheckMapper
	{
		void SqlInsert(string sql, SqlCheck sqlCheck);
		void SqlUpdate(string sql, SqlCheck sqlCheck);
		void SqlDelete(string sql, SqlCheck sqlCheck);
		void SqlDeleteAll(string sql, SqlCheck sqlCheck);
	}

	public static class CollectionSqlsWithCheckMapperExtensions
	{
		public static void SqlInsert(this ICollectionSqlsMapper mapper, string sql, SqlCheck sqlCheck)
		{
			ReflectHelper
				.CastOrThrow<ICollectionSqlsWithCheckMapper>(mapper, "SqlInsert with sqlCheck")
				.SqlInsert(sql, sqlCheck);
		}

		public static void SqlUpdate(this ICollectionSqlsMapper mapper, string sql, SqlCheck sqlCheck)
		{
			ReflectHelper
				.CastOrThrow<ICollectionSqlsWithCheckMapper>(mapper, "SqlUpdate with sqlCheck")
				.SqlUpdate(sql, sqlCheck);
		}

		public static void SqlDelete(this ICollectionSqlsMapper mapper, string sql, SqlCheck sqlCheck)
		{
			ReflectHelper
				.CastOrThrow<ICollectionSqlsWithCheckMapper>(mapper, "SqlDelete with sqlCheck")
				.SqlDelete(sql, sqlCheck);
		}

		public static void SqlDeleteAll(this ICollectionSqlsMapper mapper, string sql, SqlCheck sqlCheck)
		{
			ReflectHelper
				.CastOrThrow<ICollectionSqlsWithCheckMapper>(mapper, "SqlDeleteAll with sqlCheck")
				.SqlDeleteAll(sql, sqlCheck);
		}
	}
}
