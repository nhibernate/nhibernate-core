using NHibernate.Type;

namespace NHibernate.Mapping.ByCode
{
	// 6.0 TODO: inherit from IColumnsAndFormulasMapper
	public interface IElementMapper : IColumnsMapper
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
		// 6.0 TODO: remove once inherited from IColumnsAndFormulasMapper
		void Formula(string formula);
	}
}
