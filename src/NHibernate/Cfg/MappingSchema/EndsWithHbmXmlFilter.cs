using System;

namespace NHibernate.Cfg.MappingSchema
{
	/// <summary>
	/// Responsible for checking that a resource name matches the default pattern of "*.hbm.xml". This is the
	/// default filter for <see cref="MappingDocumentAggregator" />.
	/// </summary>
	public class EndsWithHbmXmlFilter : IAssemblyResourceFilter
	{
		public bool ShouldParse(string resourceName)
		{
			if (resourceName == null)
				throw new ArgumentNullException("resourceName");

			// TODO: ignore case?
			return resourceName.EndsWith(".hbm.xml");
		}
	}
}