using System.CodeDom;
using System.IO;
using System.Xml.Schema;

namespace NHibernate.Tool.HbmXsd
{
	/// <summary>
	/// Responsible for generating the NHibernate.Cfg.MappingSchema.Hbm* classes from nhibernate-mapping.xsd.
	/// </summary>
	public class HbmCodeGenerator : XsdCodeGenerator
	{
		private const string GeneratedCodeNamespace = "NHibernate.Cfg.MappingSchema";
		private const string MappingSchemaResourceName = "NHibernate.Tool.HbmXsd.nhibernate-mapping.xsd";

		/// <summary>Generates C# classes.</summary>
		/// <param name="outputFileName">The file to which the generated code is written.</param>
		public void Execute(string outputFileName)
		{
			using (Stream stream = GetType().Assembly.GetManifestResourceStream(MappingSchemaResourceName))
			{
				XmlSchema schema = XmlSchema.Read(stream, null);
				Execute(outputFileName, GeneratedCodeNamespace, schema);
			}
		}

		/// <summary>
		/// Customizes the generated code to better conform to the NHibernate's coding conventions.
		/// </summary>
		/// <param name="code">The customizable code DOM.</param>
		/// <param name="sourceSchema">The source XML Schema.</param>
		protected override void CustomizeGeneratedCode(CodeNamespace code, XmlSchema sourceSchema)
		{
			new ImproveHbmTypeNamesCommand(code).Execute();
			new ImproveEnumFieldsCommand(code).Execute();

			// TODO: Rename class fields?
		}
	}
}