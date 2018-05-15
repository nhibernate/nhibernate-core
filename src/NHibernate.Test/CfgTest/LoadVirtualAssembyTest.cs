using System;
using System.Collections.Generic;
using NUnit.Framework;
using NHibernate.Cfg;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.CfgTest
{
	[TestFixture]
	public class LoadVirtualAssembyTest
    {

		[Test]
		public void VirtualAssembyLoadTest()
		{
			int assemblyGenerateCount = 10;
			var config = new Configuration();
			var mapper = new ModelMapper();
			var module = Compile(assemblyGenerateCount);
			config.AddMemoryAssembly(module.Assembly);
			mapper.AddMappings(module.Assembly.GetExportedTypes());
			config.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

			if (assemblyGenerateCount == config.ClassMappings.Count)
			{
				Assert.Pass("All virtual assembly mapped.");
			}
			else
			{
				Assert.Fail("Not all virtual assembly mapped.");
			}
		}


		public Module Compile(int assemblyCount = 1)
		{
			CompilerParameters CompilerParams = new CompilerParameters();
			CompilerParams.GenerateInMemory = true;
			CompilerParams.TreatWarningsAsErrors = false;
			CompilerParams.GenerateExecutable = false;
			CompilerParams.CompilerOptions = "/optimize";
			CompilerParams.ReferencedAssemblies.AddRange(new String[] {
				"System.dll",
				"System.Core.dll",
				"System.Data.dll",
				new Uri(typeof(Cfg.Environment).Assembly.EscapedCodeBase).LocalPath
			});

			string rawCode = @"
using NHibernate.Mapping.ByCode.Conformist;

public class Entity$$INDEX$$
{
    public virtual int ID { get; set; }
    public virtual string Name { get; set; }
}

public class Entity$$INDEX$$Map : ClassMapping<Entity$$INDEX$$>
{
    public Entity$$INDEX$$Map () 
    {
        Table(" + "\"Entity$$INDEX$$\""+ @");

		Id(x => x.ID, m =>
		{
			m.Column(" + "\"ID\"" + @");
		});

		Property(x => x.Name, m =>
		{
			m.Column(" + "\"Name\"" + @");
		});
	}
}
";
			CSharpCodeProvider provider = new CSharpCodeProvider();
			List<String> code = new List<string>();
			for (int i = 0; i < assemblyCount; i++)
			{
				code.Add(rawCode.Replace("$$INDEX$$", String.Format("{0:000}", i)));
			}
			CompilerResults compile = provider.CompileAssemblyFromSource(CompilerParams, code.ToArray());
			if (compile.Errors.HasErrors)
			{
				string text = "Compile error: ";
				foreach (CompilerError ce in compile.Errors)
				{
					text += "rn" + ce.ToString() + System.Environment.NewLine;
				}
				Assert.Fail(text);
			}
			return compile.CompiledAssembly.GetModules()[0];
		}

	}
}
