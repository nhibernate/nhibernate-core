using NHibernate.Type;

namespace NHibernate.Mapping.ByCode
{
	// 6.0 TODO: inherit from IColumnsAndFormulasMapper
	public interface IMapKeyMapper : IColumnsMapper
	{
		void Type(IType persistentType);
		void Type<TPersistentType>();
		void Type(System.Type persistentType);
		void Length(int length);
		// 6.0 TODO: remove once inherited from IColumnsAndFormulasMapper
		void Formula(string formula);
	}
}
