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
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		
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
			System.IO.File.Delete("nvelocity.log");
			base.configure (props);
			Commons.Collections.ExtendedProperties p = new Commons.Collections.ExtendedProperties();
			string templatename = props["template"];
			if (templatename == null)
			{
				log.Info("No template file was specified, using default");				
				p.SetProperty("resource.loader", "class");
				p.SetProperty("class.resource.loader.class", "NHibernate.Tool.hbm2net.StringResourceLoader;hbm2net");
				templatename = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("NHibernate.Tool.hbm2net.convert.vm")).ReadToEnd();
			}
			ve = new VelocityEngine();
			ve.Init(p);
			template = ve.GetTemplate(templatename);
		}
	}
}