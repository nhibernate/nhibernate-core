using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using NHibernate;
using NHibernate.Example.Web.Domain;

namespace NHibernate.Example.Web
{
	public class ExampleApplication : HttpApplication
	{
		public static readonly Cfg.Configuration Configuration;
		public static readonly ISessionFactory SessionFactory;

		static ExampleApplication()
		{
			log4net.Config.XmlConfigurator.Configure();
			Configuration = new NHibernate.Cfg.Configuration()
				.SetDefaultAssembly(typeof(Item).Assembly.FullName)
				.SetDefaultNamespace(typeof(Item).Namespace)
				.AddDirectory(new DirectoryInfo(HostingEnvironment.MapPath("~/App_Data/")));
			
			SessionFactory = Configuration.BuildSessionFactory();
		}
		
		public static ISession GetCurrentSession()
		{
			return SessionFactory.GetCurrentSession();
		}
	}
}