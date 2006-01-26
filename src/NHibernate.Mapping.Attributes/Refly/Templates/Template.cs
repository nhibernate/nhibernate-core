using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Collections.Specialized;

namespace Refly.Templates
{
	using Refly.CodeDom;

	public abstract class Template : ITemplate
	{
		#region Fields
		private string templateName;
		private string nameFormat;
		private string outputPath = "";
		private string _namespace = null;
		private NamespaceDeclaration ns=null;
		private CodeGenerator compiler = new CodeGenerator();
		private ImportCollection imports = new ImportCollection();
		private CodeLanguage outputLanguage=CodeLanguage.Cs;
		#endregion

		public Template(string templateName, string nameFormat)
		{
			this.templateName=templateName;
			this.nameFormat=nameFormat;
			Import system=new Import();
			system.Name="System";
			this.imports.Add(system);
		}

		#region Properties
		[Category("Data")]
		[Description("Format string to name the class")]
		public string NameFormat
		{
			get
			{
				return this.nameFormat;
			}
			set
			{
				this.nameFormat=value;
			}
		}

		[Category("Data")]
		[Description("Tab used in generation of classes")]
		public string Tab
		{
			get
			{
				return this.compiler.Tab;
			}
			set
			{
				this.compiler.Tab=value;
			}
		}

		[Category("Data")]
		[Description("Base directory of generated files")]
		[EditorAttribute(typeof(FolderNameEditor),typeof(UITypeEditor))]
		public string OutputPath
		{
			get
			{
				return this.outputPath;
			}
			set
			{
				this.outputPath=value;
			}
		}


		[Category("Data")]
		[Description("Base namespace for generated classes")]
		public string Namespace
		{
			get
			{
				return this._namespace;
			}
			set
			{
				this._namespace=value;
			}
		}

		[Category("Data")]
		[Description("Namespace imports")]
		[EditorAttribute(typeof(System.ComponentModel.Design.CollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public ImportCollection Imports
		{
			get
			{
				return this.imports;
			}
			set
			{
				this.imports=value;
			}
		}

		[Category("Data")]
		[Description("Output language")]
		public CodeLanguage OutputLanguage
		{
			get
			{
				return this.outputLanguage;
			}
			set
			{
				this.outputLanguage=value;
			}
		}
		#endregion

		#region Protected Properties
		public NamespaceDeclaration NamespaceDeclaration
		{
			get
			{
				return this.ns;
			}
			set
			{
				this.NamespaceDeclaration=value;
			}
		}

		protected CodeGenerator Compiler
		{
			get
			{
				return this.compiler;
			}
		}
		#endregion

		#region ITemplate Members

		public virtual void Prepare()
		{
			// check paramters
			if(this.Namespace==null || this.Namespace.Length==0)
				throw new ArgumentException("Namespace is empty");

			this.ns=new NamespaceDeclaration(this.Namespace);
			foreach(Import import in this.imports)
				this.ns.Imports.Add(import.ToString());
		}

		public abstract void Generate();

		protected virtual void Compile()
		{

			switch(this.outputLanguage)
			{
				case CodeLanguage.Cs:
					this.compiler.Provider = CodeGenerator.CsProvider;
					break;
				case CodeLanguage.Vb:
					this.compiler.Provider = CodeGenerator.VbProvider;
					break;
			}
			this.compiler.GenerateCode(this.outputPath, this.ns);
		}

		#endregion


		[Browsable(false)]
		public virtual string TemplateName
		{
			get
			{
				return this.templateName;
			}
			set
			{
				this.templateName=value;
			}
		}

		public override string ToString()
		{
			return this.TemplateName;
		}

	}
}
