/*
* Created on 12-10-2003
* 
* To change the template for this generated file go to Window - Preferences -
* Java - Code Generation - Code and Comments
*/
using System;
using VelocityContext = NVelocity.VelocityContext;
using VelocityEngine = NVelocity.App.VelocityEngine;
using RuntimeConstants = NVelocity.Runtime.RuntimeConstants_Fields;

namespace NHibernate.Tool.hbm2net
{
	
	/// <author>  MAX
	/// 
	/// To change the template for this generated type comment go to Window -
	/// Preferences - Java - Code Generation - Code and Comments
	/// </author>
	public class VelocityRenderer:AbstractRenderer
	{
        private VelocityEngine ve;
		private NVelocity.Template template;
		
		/*
		* (non-Javadoc)
		* 
		* @see NHibernate.Tool.hbm2net.Renderer#render(java.lang.String,
		*      java.lang.String, NHibernate.Tool.hbm2net.ClassMapping,
		*      java.util.Map, java.io.PrintWriter)
		*/
		public override void  render(System.String savedToPackage, System.String savedToClass, ClassMapping classMapping, System.Collections.IDictionary class2classmap, System.IO.StreamWriter writer)
		{
			VelocityContext context = new VelocityContext();
			
			context.Put("savedToPackage", savedToPackage);
			context.Put("savedToClass", savedToClass);
			context.Put("clazz", classMapping);
			
			context.Put("class2classmap", class2classmap);
			
			context.Put("javaTool", new JavaTool());

			context.Put("runtimeversion", Guid.Empty.GetType().Assembly.ImageRuntimeVersion);
			
			System.IO.StringWriter sw = new System.IO.StringWriter();
			
			context.Put("classimports", "$classimports");

			// First run - writes to in-memory string
			template.Merge(context, sw);
			
			context.Put("classimports", new JavaTool().genImports(classMapping));
			
			// Second run - writes to file (allows for placing imports correctly and optimized ;)
			ve.Evaluate(context, writer, "hbm2net", sw.ToString());
		}
	
		public override void configure(System.Collections.Specialized.NameValueCollection props)
		{
			//			Commons.Collections.ExtendedProperties p = new Commons.Collections.ExtendedProperties();
			//			p.SetProperty( "runtime.log.logsystem.log4net.category", "x");
			//			p["resource.loader"] = "class";
			//			p["class.resource.loader.class"] = typeof(ClasspathResourceLoader).getName();
			System.IO.File.Delete("nvelocity.log");
			base.configure (props);
			ve = new VelocityEngine();
			ve.Init();
			template = ve.GetTemplate((props["template"] == null)?"pojo.vm":props["template"]);
		}
	}
}