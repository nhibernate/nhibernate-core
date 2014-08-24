using NHibernate.Type;

namespace NHibernate.Mapping.ByCode
{
	public interface IMapKeyMapper : IColumnsMapper
	{
		void Type(IType persistentType);
		void Type<TPersistentType>();
		void Type(System.Type persistentType);
		void Length(int length);
		void Formula(string formula);
	}
}