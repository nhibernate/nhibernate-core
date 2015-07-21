using System.Collections.Generic;
namespace NHibernate.Mapping.ByCode
{
	public interface IGeneratorMapper
	{
		void Params(object generatorParameters);

		void Params(IDictionary<string, object> generatorParameters);
	}
}