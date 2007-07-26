using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Microsoft.CSharp;

namespace NHibernate.Tool.HbmXsd
{
	public class XsdCodeGenerator
	{
		public void Execute(string fileName, string @namespace, XmlSchema schema)
		{
			using (StreamWriter writer = new StreamWriter(fileName, false))
			{
				CodeNamespace code = new CodeNamespace(@namespace);
				GenerateClasses(code, schema);
				CustomizeGeneratedCode(code, schema);
				WriteCSharpCodeFile(code, writer);
			}
		}

		private static void GenerateClasses(CodeNamespace code, XmlSchema schema)
		{
			XmlSchemas schemas = new XmlSchemas();
			schemas.Add(schema);
			XmlSchemaImporter importer = new XmlSchemaImporter(schemas);

			CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
			CodeGenerationOptions options = CodeGenerationOptions.None;
			XmlCodeExporter exporter = new XmlCodeExporter(code, codeCompileUnit, options);

			foreach (XmlSchemaObject item in schema.Items)
			{
				XmlSchemaElement element = item as XmlSchemaElement;

				if (element != null)
				{
					XmlQualifiedName name = new XmlQualifiedName(element.Name, schema.TargetNamespace);
					XmlTypeMapping map = importer.ImportTypeMapping(name);
					exporter.ExportTypeMapping(map);
				}
			}
		}

		protected virtual void CustomizeGeneratedCode(CodeNamespace code, XmlSchema schema)
		{
		}

		private static void WriteCSharpCodeFile(CodeNamespace code, TextWriter writer)
		{
			CSharpCodeProvider provider = new CSharpCodeProvider();
			CodeGeneratorOptions options = new CodeGeneratorOptions();

			provider.GenerateCodeFromNamespace(code, writer, options);
		}
	}
}