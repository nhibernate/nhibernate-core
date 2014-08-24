using System.Collections.Generic;

namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmDefinition : HbmBase
	{
		public IDictionary<string, string> FindParameterValues()
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();

			if (param != null)
			{
				foreach (HbmParam parameter in param)
				{
					parameters.Add(parameter.name, parameter.GetText());
				}
			}

			return parameters;
		}
	}
}