using System;
using System.Collections.Generic;
using NUnit.Framework;
using NHibernate.Cfg;
using System.CodeDom.Compiler;
using System.Linq;
using Microsoft.CSharp;
using System.Reflection;
using NHibernate.Mapping.ByCode;

namespace NHibernate.Test.CfgTest
{
	[TestFixture]
	public class LoadMemoryOnlyAssembyTest
	{
		[Test]
		public void CompileMappingForDynamicInMemoryAssembly()
		{
			const int assemblyGenerateCount = 10;
			var code = GenerateCode(assemblyGenerateCount);
			var assembly = CompileAssembly(code);
			
			var config = new Configuration();
			var mapper = new ModelMapper();
			mapper.AddMappings(assembly.GetExportedTypes());
			config.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

			Assert.That(config.ClassMappings.Count, Is.EqualTo(assemblyGenerateCount), "Not all virtual assembly mapped");
		}

		private static string[] GenerateCode(int assemblyCount)
		{
			const string codeTemplate = @"
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
        Table(""Entity$$INDEX$$"");

		Id(x => x.ID, m =>
		{
			m.Column(""ID"");
		});

		Property(x => x.Name, m =>
		{
			m.Column(""Name"");
		});
	}
}";
			var code = new string[assemblyCount];
			for (var i = 0; i < code.Length; i++)
				code[i] = codeTemplate.Replace("$$INDEX$$", i.ToString("000"));
			return code;
		}

		private static Assembly CompileAssembly(string[] code)
		{
			var parameters = new CompilerParameters
			{
				GenerateInMemory = true,
				TreatWarningsAsErrors = false,
				GenerateExecutable = false,
				CompilerOptions = "/optimize"
			};
			parameters.ReferencedAssemblies.AddRange(
				new[]
				{
					"System.dll",
					"System.Core.dll",
					"System.Data.dll",
					new Uri(typeof(Cfg.Environment).Assembly.EscapedCodeBase).LocalPath
				});

			try
			{
				using (var provider = new CSharpCodeProvider())
				{
					var compile = provider.CompileAssemblyFromSource(parameters, code);

					if (compile.Errors.HasErrors)
					{
						var text = "Compile error: " +
						           string.Join(System.Environment.NewLine, compile.Errors.Cast<CompilerError>());

						Assert.Fail(text);
					}

					return compile.CompiledAssembly;
				}
			}
			catch (PlatformNotSupportedException e)
			{
				Assert.Inconclusive(e.Message);
				return null;
			}
		}
	}
}
