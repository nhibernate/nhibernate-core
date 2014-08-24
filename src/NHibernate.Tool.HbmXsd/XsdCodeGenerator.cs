using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Microsoft.CSharp;

namespace NHibernate.Tool.HbmXsd
{
	/// <summary>Responsible for code generation from an XML schema.</summary>
	public class XsdCodeGenerator
	{
		/// <summary>Generates C# classes.</summary>
		/// <param name="outputFileName">The file to which the generated classes are written.</param>
		/// <param name="namespace">The namespace to which the generated classes belong.</param>
		/// <param name="schema">The schema from which the classes are generated.</param>
		public void Execute(string outputFileName, string @namespace, XmlSchema schema)
		{
			if (schema == null)
				throw new ArgumentNullException("schema");

			CodeNamespace code = new CodeNamespace(@namespace);

			GenerateClasses(code, schema);
			CustomizeGeneratedCode(code, schema);
			WriteCSharpCodeFile(code, outputFileName);
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

		/// <summary>
		/// If overridden by inheritors, customizes the generated code DOM before writing it to file.
		/// </summary>
		/// <param name="code">The customizable code DOM.</param>
		/// <param name="sourceSchema">The source XML Schema.</param>
		protected virtual void CustomizeGeneratedCode(CodeNamespace code, XmlSchema sourceSchema)
		{
		}

		private static void WriteCSharpCodeFile(CodeNamespace code, string outputFileName)
		{
			CSharpCodeProvider provider = new CSharpCodeProvider();
			CodeGeneratorOptions options = new CodeGeneratorOptions();

			using (StreamWriter writer = new StreamWriter(outputFileName, false))
				provider.GenerateCodeFromNamespace(code, writer, options);
		}
	}
}