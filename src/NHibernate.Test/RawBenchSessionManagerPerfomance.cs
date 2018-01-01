//#define UPSTREAM
using System;
using System.IO;
using NHibernate.Util;
using NUnit.Framework;


namespace NHibernate.Test
{
	[TestFixture]
	public class RawBenchSessionManagerPerfomance
	{
#if UPSTREAM
		[Test]
		public void InternLevel_Upstream()
		{
			//Just to force initialize static ctor and do not calculate memory consumed by Environoment
			Cfg.Environment.VerifyProperties(CollectionHelper.EmptyDictionary<string, string>());

			RunTest();
		}

#else

		[Test]
		public void InternLevel_Minimal()
		{
			Cfg.Environment.InternLevel = InternLevel.Minimal;
			RunTest();
		}
		
		[Test]
		public void InternLevel_Default()
		{
			Cfg.Environment.InternLevel = InternLevel.Default;
			RunTest();
		}

		[Test]
		public void InternLevel_SessionFactories()
		{
			Cfg.Environment.InternLevel = InternLevel.SessionFactories;
			RunTest();
		}

		[Test]
		public void InternLevel_AppDomains()
		{
			Cfg.Environment.InternLevel = InternLevel.AppDomains;
			RunTest();
		}


#endif

		private static void RunTest()
		{
			var factory = NH.Bencher.SessionManager.SessionFactory;

			var setup = new AppDomainSetup();
			var si = AppDomain.CurrentDomain.SetupInformation;
			setup.ApplicationBase = si.ApplicationBase;
			setup.ConfigurationFile = si.ConfigurationFile;

			AppDomain newDomain = AppDomain.CreateDomain("New Domain", null, si);

#if !UPSTREAM
			newDomain.SetData("internLevel", Cfg.Environment.InternLevel);
#endif
			try
			{
				newDomain.DoCallBack(
					() =>
					{
						StringWriter s = new StringWriter();
						Console.SetOut(s);
						Console.WriteLine();
						Console.WriteLine();
						Console.WriteLine("From new App Domain...");
#if !UPSTREAM
						Cfg.Environment.InternLevel = (InternLevel) AppDomain.CurrentDomain.GetData("internLevel");
#endif
						try
						{
							
							var factory2 = NH.Bencher.SessionManager.SessionFactory;
						}
						finally
						{
							AppDomain.CurrentDomain.SetData("log", s.ToString());
							AppDomain.CurrentDomain.SetData("memory", NH.Bencher.SessionManager.LastTotalMemoryUsage);
						}
					});
			}
			finally
			{
				Console.WriteLine(newDomain.GetData("log"));
				Console.WriteLine("Total Memory Usage in 2 App Domains: " + ToKbSize((long) newDomain.GetData("memory") + NH.Bencher.SessionManager.LastTotalMemoryUsage));

				AppDomain.Unload(newDomain);
			}
		}

		private static string ToKbSize(long bytes)
		{
			return (bytes / 1024.0).ToString("0,0.00") + " Kb";
		}
	}

}
