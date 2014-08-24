namespace NHibernate.Mapping.ByCode
{
	public interface IColumnMapper
	{
		void Name(string name);
		void Length(int length);
		void Precision(short precision);
		void Scale(short scale);
		void NotNullable(bool notnull);
		void Unique(bool unique);
		void UniqueKey(string uniquekeyName);
		void SqlType(string sqltype);
		void Index(string indexName);
		void Check(string checkConstraint);
		void Default(object defaultValue);
	}
}