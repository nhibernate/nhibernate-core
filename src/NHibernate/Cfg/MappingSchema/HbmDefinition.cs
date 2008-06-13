using System.Collections.Generic;

namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmDefinition : HbmBase
	{
		public Dictionary<string, string> FindParameters()
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>(10);

			if (param != null)
			{
				foreach (HbmParam hbmParam in param)
				{
					string name = hbmParam.name;
					string text = JoinString(hbmParam.Text);

					parameters.Add(name, text);
				}
			}
			return parameters;
		}
	}
}
