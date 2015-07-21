using NHibernate.Type;
using NHibernate.UserTypes;

namespace NHibernate.Mapping.ByCode
{
	public interface IVersionMapper : IAccessorPropertyMapper, IColumnsMapper
	{
		void Type(IVersionType persistentType);
		void Type<TPersistentType>() where TPersistentType : IUserVersionType;
		void Type(System.Type persistentType);
		void UnsavedValue(object value);
		void Insert(bool useInInsert);
		void Generated(VersionGeneration generatedByDb);
	}
}