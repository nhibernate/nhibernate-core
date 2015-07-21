using System.IO;
using System.Web;
using System.Web.Hosting;
using NHibernate.Cfg;
using NHibernate.Example.Web.Domain;

namespace NHibernate.Example.Web
{
	public class ExampleApplication : HttpApplication
	{
		public static readonly Configuration Configuration;
		public static readonly ISessionFactory SessionFactory;

		static ExampleApplication()
		{
			log4net.Config.XmlConfigurator.Configure();
			Configuration = new Configuration();
			string nhConfigPath = HostingEnvironment.MapPath("~/App_Data/hibernate.cfg.xml");
			if (File.Exists(nhConfigPath))
			{
				Configuration.Configure(nhConfigPath);
			}
			Configuration.SetDefaultAssembly(typeof(Item).Assembly.FullName)
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