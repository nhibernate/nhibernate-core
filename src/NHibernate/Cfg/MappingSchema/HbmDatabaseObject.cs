using System.Collections.Generic;

namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmDatabaseObject : HbmBase
	{
		public string FindCreateText()
		{
			HbmCreate createSchema = Find<HbmCreate>(Items);
			return JoinString(createSchema.Text);
		}

		public HbmDefinition FindDefinition()
		{
			return Find<HbmDefinition>(Items);
		}

		public IList<string> FindDialectScopeNames()
		{
			IList<string> dialectScopeNames = new List<string>();

			if (dialectscope != null)
				foreach (HbmDialectScope dialectScopeSchema in dialectscope)
					dialectScopeNames.Add(dialectScopeSchema.name);

			return dialectScopeNames;
		}

		public string FindDropText()
		{
			HbmDrop dropSchema = Find<HbmDrop>(Items);
			return JoinString(dropSchema.Text);
		}

		public bool HasDefinition()
		{
			return FindDefinition() != null;
		}
	}
}