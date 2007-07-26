using System.CodeDom;

namespace NHibernate.Tool.HbmXsd
{
	public class ImproveHbmTypeNamesCommand : ImproveTypeNamesCommand
	{
		public ImproveHbmTypeNamesCommand(CodeNamespace code) : base(code)
		{
		}

		protected override string CustomizeBetterTypeName(string betterTypeName)
		{
			return "Hbm" + betterTypeName;
		}

		protected override string HandleSpecialCaseTypeNames(string originalName)
		{
			switch (originalName)
			{
				case "hibernatemapping":
					return "Mapping";

				case "customSQL":
				case "cacheType":
					return CamelCase(originalName);

				default:
					return null;
			}
		}
	}
}