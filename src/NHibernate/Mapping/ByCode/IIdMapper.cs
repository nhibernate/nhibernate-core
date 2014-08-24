using System;
using NHibernate.Type;

namespace NHibernate.Mapping.ByCode
{
	public interface IIdMapper : IAccessorPropertyMapper
	{
		void Generator(IGeneratorDef generator);
		void Generator(IGeneratorDef generator, Action<IGeneratorMapper> generatorMapping);

		void Type(IIdentifierType persistentType);
		//void Type<TPersistentType>() where TPersistentType : IIdentifierType;
		//void Type<TPersistentType>(object parameters) where TPersistentType : IIdentifierType;
		//void Type(System.System.Type persistentType, object parameters);
		//void Column(Action<IColumnMapper> columnMapper);
		//void Columns(params Action<IColumnMapper>[] columnMapper);
		void UnsavedValue(object value);
		void Column(string name);
		void Length(int length);
	}
}