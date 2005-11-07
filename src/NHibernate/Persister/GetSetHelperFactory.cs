using System;
using System.Text;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Reflection;

using log4net;

using NHibernate.Property;

namespace NHibernate.Persister 
{

    /// <summary>
    /// Factory that generate object based on IGetSetHelper needed to replace the use
    /// of reflection.
    /// </summary>
    /// <remarks>
    /// Used in <see cref="NHibernate.Persister.AbstractEntityPersister"/> and
    /// <see cref="NHibernate.Type.ComponentType"/>
    /// </remarks>
    public class GetSetHelperFactory 
	{

        private static readonly ILog log = LogManager.GetLogger(typeof(GetSetHelperFactory));

        CompilerParameters cp = new CompilerParameters();
        System.Type mappedClass;
        ISetter[] setters;
        IGetter[] getters;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="mappedClass">The target class</param>
        /// <param name="setters">Array of setters</param>
        /// <param name="getters">Array of getters</param>
        private GetSetHelperFactory(System.Type mappedClass, ISetter[] setters, IGetter[] getters)
        {
            this.mappedClass = mappedClass;
            this.getters = getters;
            this.setters = setters;
        }

        /// <summary>
        /// Private ctor. Can't create an empty object
        /// </summary>
        private GetSetHelperFactory() {}


        /// <summary>
        /// Generate the IGetSetHelper object
        /// </summary>
        /// <param name="mappedClass">The target class</param>
        /// <param name="setters">Array of setters</param>
        /// <param name="getters">Array of getters</param>
        /// <returns>null if the generation fail</returns>
        static public IGetSetHelper Create(System.Type mappedClass, ISetter[] setters, IGetter[] getters)
        {
            // Uncomment this to work without CodeDom
            //return null;

            try
            {
                GetSetHelperFactory f = new GetSetHelperFactory(mappedClass, setters, getters);
                f.InitCompiler();
                string code = f.GenerateCode();
                return f.Build(code);
            }
            catch (Exception e)
            {
                log.Info( "Disabling reflection optimizer for class " + mappedClass.FullName );
				log.Debug( "CodeDOM compilation failed", e );
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
            #if !DEBUG
            cp.CompilerOptions = "/optimize";
            #endif

            AddAssembly(Assembly.GetExecutingAssembly().Location);
            AddAssembly(mappedClass.Module.FullyQualifiedName);
        }

        /// <summary>
        /// Add an assembly to the list of ReferencedAssemblies
        /// required to build the class
        /// </summary>
        /// <param name="name"></param>
        private void AddAssembly(string name)
        {
            if (name.StartsWith("System.")) return;

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
        private IGetSetHelper Build(string code)
        {
            CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
            #if NET_2_0
            CompilerResults res = provider.CompileAssemblyFromSource(cp, new string[] { code });
            #else
            ICodeCompiler compiler = provider.CreateCompiler();
            CompilerResults res = compiler.CompileAssemblyFromSource(cp, code);
            #endif

            if (res.Errors.HasErrors)
            {
                log.Error("Compiled with error:\n" + code);
                foreach (CompilerError e in res.Errors)
                {
                    log.Error(
                        String.Format("Line:{0}, Column:{1} Message:{2}",
                            e.Line, e.Column, e.ErrorText)
                    );
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
            IGetSetHelper getset = (IGetSetHelper)assembly.CreateInstance(types[0].FullName, false,
                BindingFlags.CreateInstance, null, new object[] { setters, getters },
                CultureInfo.InvariantCulture, null);

            return getset;
        }

        const string header =
            "using System;\n" +
            "using NHibernate.Property;\n" +
            "namespace NHibernate.Persister {\n";

        const string classDef =
            "public class GetSetHelper_{0} : IGetSetHelper {{\n" +
            "  ISetter[] setters;\n" +
            "  IGetter[] getters;\n" +
            "  public GetSetHelper_{0}(ISetter[] setters, IGetter[] getters) {{\n" +
            "    this.setters = setters;\n" +
            "    this.getters = getters;\n" +
            "  }}\n";

        const string startSetMethod =
            "public void SetPropertyValues(object obj, object[] values) {{\n" +
            "  {0} t = ({0})obj;\n";

        const string closeSetMethod =
            "}\n";

        const string startGetMethod =
            "public object[] GetPropertyValues(object obj) {{\n" +
            "  {0} t = ({0})obj;\n" +
            "  object[] ret = new object[{1}];\n";

        const string closeGetMethod =
            "  return ret;\n" +
            "}\n";

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
            StringBuilder sb = new StringBuilder();

            sb.Append(header);
            sb.AppendFormat(classDef, mappedClass.FullName.Replace('.', '_').Replace("+", "__"));

            sb.AppendFormat(startSetMethod, mappedClass.FullName.Replace('+', '.'));
            for (int i = 0; i < setters.Length; i++)
            {
                ISetter setter = setters[i];
                if (setter is BasicSetter && IsPublic(setter.PropertyName))
                {
                    if (setter.Property.PropertyType.IsValueType)
                    {
                        sb.AppendFormat(
                            "  t.{0} = values[{2}] == null ? new {1}() : ({1})values[{2}];\n",
                            setter.PropertyName,
                            setter.Property.PropertyType.FullName.Replace('+', '.'), 
                            i);
                    }
                    else
                    {
                        sb.AppendFormat("  t.{0} = ({1})values[{2}];\n",
                            setter.PropertyName,
                            setter.Property.PropertyType.FullName.Replace('+', '.'), i);
                    }

                    AddAssembly(setter.Property.PropertyType.Assembly.Location);
                }
                else
                {
                    sb.AppendFormat("  setters[{0}].Set(obj, values[{0}]);\n", i);
                }
            }
            sb.Append(closeSetMethod);  // Close Set

            sb.AppendFormat(startGetMethod, mappedClass.FullName.Replace('+', '.'), getters.Length);
            for (int i = 0; i < getters.Length; i++)
            {
                IGetter getter = getters[i];
                if (getter is BasicGetter && IsPublic(getter.PropertyName))
                {
                    sb.AppendFormat("  ret[{0}] = t.{1};\n", i, getter.PropertyName);
                }
                else
                {
                    sb.AppendFormat("  ret[{0}] = getters[{0}].Get(obj);\n", i);
                }
            }
            sb.Append(closeGetMethod);

            sb.Append("}\n");  // Close class
            sb.Append("}\n");  // Close namespace

            return sb.ToString();
        }


    }
}
