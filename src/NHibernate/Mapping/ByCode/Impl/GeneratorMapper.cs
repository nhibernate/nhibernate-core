using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class GeneratorMapper : IGeneratorMapper
	{
		private readonly HbmGenerator generator;

		public GeneratorMapper(HbmGenerator generator)
		{
			this.generator = generator;
		}

		#region Implementation of IGeneratorMapper

		public void Params(object generatorParameters)
		{
			this.Params(generatorParameters.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(generatorParameters, null)));
		}

		public void Params(IDictionary<string, object> generatorParameters)
		{
			if (generatorParameters == null)
			{
				return;
			}
			generator.param = (from pi in generatorParameters
							   let pname = pi.Key
							   let pvalue = pi.Value
							   select
								new HbmParam { name = pname, Text = new[] { ReferenceEquals(pvalue, null) ? "null" : pvalue.ToString() } }).
				ToArray();
		}

		#endregion
	}
}