using NHibernate.Type;

namespace NHibernate.Mapping.ByCode
{
	public interface IPropertyMapper : IEntityPropertyMapper, IColumnsMapper
	{
		void Type(IType persistentType);
		void Type<TPersistentType>();
		void Type<TPersistentType>(object parameters);
		void Type(System.Type persistentType, object parameters);
		void Length(int length);
		void Precision(short precision);
		void Scale(short scale);
		void NotNullable(bool notnull);
		void Unique(bool unique);
		void UniqueKey(string uniquekeyName);
		void Index(string indexName);
		void Formula(string formula);
		void Update(bool consideredInUpdateQuery);
		void Insert(bool consideredInInsertQuery);
		void Lazy(bool isLazy);
		void Generated(PropertyGeneration generation);
	}
}