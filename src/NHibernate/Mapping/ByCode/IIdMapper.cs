using System;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode
{
	// 6.0 TODO: move into IIdMapper,
	// and probably add an ITypeMapper for mutualizing the four Type methods with IElementMapper and IPropertyMapper
	// (Note that there is no IKeyPropertyMapper to be concerned with, the KeyPropertyMapper use IPropertyMapper
	// directly instead.)
	public static class IdMapperExtensions
	{
		public static void Type<TPersistentType>(this IIdMapper idMapper) where TPersistentType : IIdentifierType
		{
			ReflectHelper
				.CastOrThrow<IdMapper>(idMapper, "generic Type method")
				.Type<TPersistentType>();
		}

		public static void Type<TPersistentType>(this IIdMapper idMapper, object parameters) where TPersistentType : IIdentifierType
		{
			ReflectHelper
				.CastOrThrow<IdMapper>(idMapper, "generic parameterized Type method")
				.Type<TPersistentType>(parameters);
		}

		public static void Type(this IIdMapper idMapper, System.Type persistentType, object parameters)
		{
			ReflectHelper
				.CastOrThrow<IdMapper>(idMapper, "parameterized Type method")
				.Type(persistentType, parameters);
		}
	}
	
	public interface IIdMapper : IAccessorPropertyMapper, IColumnsMapper
	{
		void Generator(IGeneratorDef generator);
		void Generator(IGeneratorDef generator, Action<IGeneratorMapper> generatorMapping);

		void Type(IIdentifierType persistentType);
		void UnsavedValue(object value);
		void Length(int length);
	}
}
