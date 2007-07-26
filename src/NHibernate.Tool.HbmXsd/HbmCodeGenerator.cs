using System.CodeDom;
using System.IO;
using System.Xml.Schema;

namespace NHibernate.Tool.HbmXsd
{
	public class HbmCodeGenerator : XsdCodeGenerator
	{
		private const string GeneratedCodeNamespace = "NHibernate.Cfg.MappingSchema";
		private const string MappingSchemaResourceName = "NHibernate.Tool.HbmXsd.nhibernate-mapping.xsd";

		public void Execute(string outputFileName)
		{
			using (Stream xsdStream = typeof (Program).Assembly.GetManifestResourceStream(MappingSchemaResourceName))
			{
				XmlSchema schema = XmlSchema.Read(xsdStream, null);
				Execute(outputFileName, GeneratedCodeNamespace, schema);
			}
		}

		protected override void CustomizeGeneratedCode(CodeNamespace code, XmlSchema schema)
		{
			new ImproveHbmTypeNamesCommand(code).Execute();
			// TODO fields?
		}
	}
}