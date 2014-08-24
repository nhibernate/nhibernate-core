namespace NHibernate.Mapping.ByCode
{
	public interface ICollectionSqlsMapper
	{
		void Loader(string namedQueryReference);
		void SqlInsert(string sql);
		void SqlUpdate(string sql);
		void SqlDelete(string sql);
		void SqlDeleteAll(string sql);
		void Subselect(string sql);
	}
}