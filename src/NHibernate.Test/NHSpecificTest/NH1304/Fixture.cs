using System.Text;
using log4net.Core;
using NHibernate.Tuple.Entity;
using NUnit.Framework;
namespace NHibernate.Test.NHSpecificTest.NH1304
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void BuildSessionFactory()
		{
			using (LogSpy ls = new LogSpy(typeof (AbstractEntityTuplizer)))
			{
				base.BuildSessionFactory();
				StringBuilder wholeMessage = new StringBuilder();
				foreach (LoggingEvent loggingEvent in ls.Appender.GetEvents())
				{
					string singleMessage = loggingEvent.RenderedMessage;
					if (singleMessage.IndexOf("AbstractEntityTuplizer") > 0)
						Assert.Greater(singleMessage.IndexOf("No custom accessors found"), -1);
					wholeMessage.Append(singleMessage);
				}
				string logs = wholeMessage.ToString();
				Assert.AreEqual(-1, logs.IndexOf("Custom accessors found"));
			}
		}
	}
}