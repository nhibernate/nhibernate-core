using log4net.Plugin;
using log4net.Repository;

namespace NHibernate.Test.NHSpecificTest.NH995
{
	public class LogPlugin : IPlugin
	{
		public void Attach(ILoggerRepository repository)
		{
			
		}

		public void Shutdown()
		{
			// Nothing to do
		}

		public string Name
		{
			get { return "NH995"; }
		}
	}
}
