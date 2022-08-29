using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class GeneratorMapper : IGeneratorMapper
	{
		private readonly HbmGenerator _generator;

		public GeneratorMapper(HbmGenerator generator)
		{
			_generator = generator;
		}

		#region Implementation of IGeneratorMapper

		public void Params(object generatorParameters)
		{
			var dictionary = generatorParameters.GetType()
												.GetProperties()
												.ToDictionary(x => x.Name, x => x.GetValue(generatorParameters, null));
			Params(dictionary);
		}

		public void Params(IDictionary<string, object> generatorParameters)
		{
			if (generatorParameters == null)
			{
				return;
			}

			_generator.param = generatorParameters.ToArray(
				pi =>
				{
					object pvalue = pi.Value;
					return new HbmParam { name = pi.Key, Text = new[] { ReferenceEquals(pvalue, null) ? "null" : pvalue.ToString() } };
				});
		}

		#endregion
	}
}
