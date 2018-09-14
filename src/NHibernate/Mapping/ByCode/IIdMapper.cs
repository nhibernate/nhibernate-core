using System;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode
{
	public interface IIdMapper : IAccessorPropertyMapper, IColumnsMapper
	{
		void Generator(IGeneratorDef generator);
		void Generator(IGeneratorDef generator, Action<IGeneratorMapper> generatorMapping);

		void Type(IIdentifierType persistentType);
		void UnsavedValue(object value);
		void Length(int length);
	}

	public static class IdMapperExtensions
	{
		public static void Type<TPersistentType>(this IIdMapper idMapper)
		{
			Type<TPersistentType>(idMapper, null);
		}

		public static void Type<TPersistentType>(this IIdMapper idMapper, object parameters)
		{
			Type(idMapper, typeof (TPersistentType), parameters);
		}

		// 6.0 TODO: move into IIdMapper,
		// and probably add an ITypeMapper for mutualizing it with IElementMapper and IPropertyMapper
		// (Note that there is no IKeyPropertyMapper to be concerned with, the KeyPropertyMapper use IPropertyMapper
		// directly instead.)
		public static void Type(this IIdMapper idMapper, System.Type persistentType, object parameters)
		{
			ReflectHelper
				.CastOrThrow<IdMapper>(idMapper, "Type method with a type argument")
				.Type(persistentType, parameters);
		}
	}
}
