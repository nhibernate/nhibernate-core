#if NETCOREAPP2_0
namespace NHibernate.TestDatabaseSetup
{
	public class Program
	{
		public static int Main(string[] args)
		{
			return new NUnitLite.AutoRun(typeof(Program).Assembly).Execute(args);
		}
	}
}
#endif