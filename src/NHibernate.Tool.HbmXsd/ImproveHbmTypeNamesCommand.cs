using System.CodeDom;

namespace NHibernate.Tool.HbmXsd
{
	/// <summary>
	/// Responsible for customizing type names in the generated code to match the desired output.
	/// </summary>
	public class ImproveHbmTypeNamesCommand : ImproveTypeNamesCommand
	{
		public ImproveHbmTypeNamesCommand(CodeNamespace code)
			: base(code)
		{
		}

		/// <summary>
		/// Overrides the <see cref="ImproveTypeNamesCommand.GetNewTypeName"/> to add the Hbm prefix and
		/// handle several specials cases.
		/// </summary>
		protected override string GetNewTypeName(string originalName, string rootElementName)
		{
			const string Prefix = "Hbm";

			switch (originalName)
			{
				case "hibernatemapping":
					return Prefix + "Mapping";

				case "resultset":
					return Prefix + "ResultSet";

				case "customSQL":
				case "cacheType":
					return Prefix + StringTools.CamelCase(originalName);

				default:
					return Prefix + base.GetNewTypeName(originalName, rootElementName);
			}
		}
	}
}