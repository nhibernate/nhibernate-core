/*
* Created on 12-10-2003
* 
* To change the template for this generated file go to Window - Preferences -
* Java - Code Generation - Code and Comments
*/
using System;
using System.IO;
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
	public class VelocityRenderer : AbstractRenderer
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
		public override void render(string savedToPackage, string savedToClass, ClassMapping classMapping, System.Collections.IDictionary class2classmap, System.IO.StreamWriter writer)
		{
			VelocityContext context = new VelocityContext();
			
			context.Put("savedToPackage", savedToPackage);
			context.Put("savedToClass", savedToClass);
			context.Put("clazz", classMapping);
			
			context.Put("class2classmap", class2classmap);
			
			context.Put("languageTool", new LanguageTool());

			context.Put("runtimeversion", Guid.Empty.GetType().Assembly.ImageRuntimeVersion);
			
			System.IO.StringWriter sw = new System.IO.StringWriter();
			
			context.Put("classimports", "$classimports");

			// First run - writes to in-memory string
			template.Merge(context, sw);
			
			context.Put("classimports", new LanguageTool().genImports(classMapping));
			
			// Second run - writes to file (allows for placing imports correctly and optimized ;)
			ve.Evaluate(context, writer, "hbm2net", sw.ToString());
		}
	
		public override void configure(DirectoryInfo workingDirectory, System.Collections.Specialized.NameValueCollection props)
		{
			try
			{
				File.Delete("nvelocity.log");
			}
			catch (IOException)
			{
				// TODO: This is evil! need to investigate further. Cannot get
				// exclusive lock on the log file with this assembly now a 
				// library (as opposed to an exe). However not convinced that
				// the line isn't a hangover from java conversion. Need to 
				// investigate further.
				;
			}
			base.configure (workingDirectory, props);
			Commons.Collections.ExtendedProperties p = new Commons.Collections.ExtendedProperties();
			string templateName = props["template"];
			string templateSrc;
			if (templateName == null)
			{
				log.Info("No template file was specified, using default");	
				p.SetProperty("resource.loader", "class");
				p.SetProperty("class.resource.loader.class", "NHibernate.Tool.hbm2net.StringResourceLoader;NHibernate.Tool.hbm2net");
				templateSrc = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("NHibernate.Tool.hbm2net.convert.vm")).ReadToEnd();
			}
			else
			{
				// NH-242 raised issue of where NVelocity looks when supplied with a unpathed file name. Hence
				// will take responsiblity of explicitly instructing NVelocity where to look.
				if (!Path.IsPathRooted(templateName))
				{
					templateName = Path.Combine(this.WorkingDirectory.FullName, templateName);
				}
				if(!File.Exists(templateName))
				{
					string msg = string.Format("Cannot find template file using absolute path or relative to '{0}'.", this.WorkingDirectory.FullName);
					throw new IOException(msg);
				}

				p.SetProperty("resource.loader", "class");
				p.SetProperty("class.resource.loader.class", "NHibernate.Tool.hbm2net.StringResourceLoader;NHibernate.Tool.hbm2net");
				using(System.IO.StreamReader sr = new StreamReader(System.IO.File.OpenRead(templateName)))
				{
					templateSrc = sr.ReadToEnd();
					sr.Close();
				}
			}
			ve = new VelocityEngine();
			ve.Init(p);
			template = ve.GetTemplate(templateSrc);
		}
	}
}