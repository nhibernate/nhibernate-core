using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;

using Microsoft.CSharp;
using NHibernate.Properties;

namespace NHibernate.Bytecode.CodeDom
{
	/// <summary>
	/// CodeDOM-based bytecode provider.
	/// </summary>
	public class BytecodeProviderImpl : AbstractBytecodeProvider
	{
		private static readonly ILogger log = LoggerProvider.LoggerFor(typeof (BytecodeProviderImpl));

		#region IBytecodeProvider Members

		public override IReflectionOptimizer GetReflectionOptimizer(System.Type clazz, IGetter[] getters, ISetter[] setters)
		{
			if (clazz.IsValueType)
			{
				// Cannot create optimizer for value types - the setter method will not work.
				log.Info("Disabling reflection optimizer for value type " + clazz.FullName);
				return null;
			}
			return new Generator(clazz, getters, setters).CreateReflectionOptimizer();
		}

		#endregion

		#region Nested type: Generator

		public class Generator
		{
			private const string classDef =
				@"public class GetSetHelper_{0} : IReflectionOptimizer, IAccessOptimizer {{
					ISetter[] setters;
					IGetter[] getters;
					
					public GetSetHelper_{0}(ISetter[] setters, IGetter[] getters) {{
						this.setters = setters;
						this.getters = getters;
					}}

					public IInstantiationOptimizer InstantiationOptimizer {{
						get {{ return null; }}
					}}

					public IAccessOptimizer AccessOptimizer {{
						get {{ return this; }}
					}}
					";

			private const string closeGetMethod = "  return ret;\n" + "}\n";
			private const string closeSetMethod = "}\n";

			private const string header =
				"using System;\n" + "using NHibernate.Property;\n" + "namespace NHibernate.Bytecode.CodeDom {\n";

			private const string startGetMethod =
				"public object[] GetPropertyValues(object obj) {{\n" + "  {0} t = ({0})obj;\n"
				+ "  object[] ret = new object[{1}];\n";

			private const string startSetMethod =
				"public void SetPropertyValues(object obj, object[] values) {{\n" + "  {0} t = ({0})obj;\n";

			private readonly CompilerParameters cp = new CompilerParameters();
			private readonly IGetter[] getters;
			private readonly System.Type mappedClass;
			private readonly ISetter[] setters;

			/// <summary>
			/// ctor
			/// </summary>
			/// <param name="mappedClass">The target class</param>
			/// <param name="setters">Array of setters</param>
			/// <param name="getters">Array of getters</param>
			public Generator(System.Type mappedClass, IGetter[] getters, ISetter[] setters)
			{
				this.mappedClass = mappedClass;
				this.getters = getters;
				this.setters = setters;
			}

			public IReflectionOptimizer CreateReflectionOptimizer()
			{
				try
				{
					InitCompiler();
					return Build(GenerateCode());
				}
				catch (Exception e)
				{
					log.Info("Disabling reflection optimizer for class " + mappedClass.FullName);
					log.Debug("CodeDOM compilation failed", e);
					return null;
				}
			}

			/// <summary>
			/// Set up the compiler options
			/// </summary>
			private void InitCompiler()
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("Init compiler for class " + mappedClass.FullName);
				}

				cp.GenerateInMemory = true;
				cp.TreatWarningsAsErrors = true;
#if ! DEBUG
				cp.CompilerOptions = "/optimize";
#endif

				AddAssembly(Assembly.GetExecutingAssembly().Location);

				Assembly classAssembly = mappedClass.Assembly;
				AddAssembly(classAssembly.Location);

				foreach (AssemblyName referencedName in classAssembly.GetReferencedAssemblies())
				{
					Assembly referencedAssembly = Assembly.Load(referencedName);
					AddAssembly(referencedAssembly.Location);
				}
			}

			/// <summary>
			/// Add an assembly to the list of ReferencedAssemblies
			/// required to build the class
			/// </summary>
			/// <param name="name"></param>
			private void AddAssembly(string name)
			{
				if (name.StartsWith("System."))
				{
					return;
				}

				if (!cp.ReferencedAssemblies.Contains(name))
				{
					if (log.IsDebugEnabled)
					{
						log.Debug("Adding referenced assembly " + name);
					}
					cp.ReferencedAssemblies.Add(name);
				}
			}

			/// <summary>
			/// Build the generated code
			/// </summary>
			/// <param name="code">Generated code</param>
			/// <returns>An instance of the generated class</returns>
			private IReflectionOptimizer Build(string code)
			{
				CodeDomProvider provider = new CSharpCodeProvider();
				CompilerResults res = provider.CompileAssemblyFromSource(cp, new[] {code});

				if (res.Errors.HasErrors)
				{
					log.Debug("Compiled with error:\n" + code);
					foreach (CompilerError e in res.Errors)
					{
						log.Debug(String.Format("Line:{0}, Column:{1} Message:{2}", e.Line, e.Column, e.ErrorText));
					}
					throw new InvalidOperationException(res.Errors[0].ErrorText);
				}
				else
				{
					if (log.IsDebugEnabled)
					{
						log.Debug("Compiled ok:\n" + code);
					}
				}

				Assembly assembly = res.CompiledAssembly;
				System.Type[] types = assembly.GetTypes();
				var optimizer =
					(IReflectionOptimizer)
					assembly.CreateInstance(types[0].FullName, false, BindingFlags.CreateInstance, null,
					                        new object[] {setters, getters}, null, null);

				return optimizer;
			}

			/// <summary>
			/// Check if the property is public
			/// </summary>
			/// <remarks>
			/// <para>If IsPublic==true I can directly set the property</para>
			/// <para>If IsPublic==false I need to use the setter/getter</para>
			/// </remarks>
			/// <param name="propertyName"></param>
			/// <returns></returns>
			private bool IsPublic(string propertyName)
			{
				return mappedClass.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public) != null;
			}

			/// <summary>
			/// Generate the required code
			/// </summary>
			/// <returns>C# code</returns>
			private string GenerateCode()
			{
				var sb = new StringBuilder();

				sb.Append(header);
				sb.AppendFormat(classDef, mappedClass.FullName.Replace('.', '_').Replace("+", "__"));

				sb.AppendFormat(startSetMethod, mappedClass.FullName.Replace('+', '.'));
				for (int i = 0; i < setters.Length; i++)
				{
					ISetter setter = setters[i];

					if (setter is BasicPropertyAccessor.BasicSetter && IsPublic(setter.PropertyName))
					{
						System.Type type = getters[i].ReturnType;

						if (type.IsValueType)
						{
							sb.AppendFormat("  t.{0} = values[{2}] == null ? new {1}() : ({1})values[{2}];\n", setter.PropertyName,
							                type.FullName.Replace('+', '.'), i);
						}
						else
						{
							sb.AppendFormat("  t.{0} = ({1})values[{2}];\n", setter.PropertyName, type.FullName.Replace('+', '.'), i);
						}
					}
					else
					{
						sb.AppendFormat("  setters[{0}].Set(obj, values[{0}]);\n", i);
					}
				}
				sb.Append(closeSetMethod); // Close Set

				sb.AppendFormat(startGetMethod, mappedClass.FullName.Replace('+', '.'), getters.Length);
				for (int i = 0; i < getters.Length; i++)
				{
					IGetter getter = getters[i];
					if (getter is BasicPropertyAccessor.BasicGetter && IsPublic(getter.PropertyName))
					{
						sb.AppendFormat("  ret[{0}] = t.{1};\n", i, getter.PropertyName);
					}
					else
					{
						sb.AppendFormat("  ret[{0}] = getters[{0}].Get(obj);\n", i);
					}
				}
				sb.Append(closeGetMethod);

				sb.Append("}\n"); // Close class
				sb.Append("}\n"); // Close namespace

				return sb.ToString();
			}
		}

		#endregion
	}
}