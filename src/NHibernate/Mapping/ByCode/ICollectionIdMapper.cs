using System;
using NHibernate.Type;

namespace NHibernate.Mapping.ByCode
{
	public interface ICollectionIdMapper
	{
		void Generator(IGeneratorDef generator);
		void Generator(IGeneratorDef generator, Action<IGeneratorMapper> generatorMapping);

		void Type(IIdentifierType persistentType);
		void Column(string name);
		void Length(int length);
	}
}