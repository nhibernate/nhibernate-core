using System.Reflection;

namespace NHibernate.Cfg.MappingSchema
{
	/// <summary>
	/// Responsible for determining whether an embedded resource should be parsed for HBM XML data while
	/// iterating through an <see cref="Assembly" />.
	/// </summary>
	public interface IAssemblyResourceFilter
	{
		bool ShouldParse(string resourceName);
	}
}