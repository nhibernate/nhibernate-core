using System.Globalization;
using System.Threading;

namespace NHibernate.Tool.HbmXsd
{
	public class Program
	{
		private static void Main(string[] args)
		{
			string outFile = args.Length == 0 ? @"..\..\..\NHibernate\Cfg\MappingSchema\Hbm.generated.cs" : args[0];
			var currentUiCulture = new CultureInfo("en-us");
			Thread.CurrentThread.CurrentCulture = currentUiCulture;
			Thread.CurrentThread.CurrentUICulture = currentUiCulture;
			new HbmCodeGenerator().Execute(outFile);
		}
	}
}