using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace NHibernate.Tool.HbmXsd
{
	public class Program
	{
		private static void Main(string[] args)
		{
			string outFile = Path.GetFullPath(args.Length == 0
				? @"..\..\..\..\NHibernate\Cfg\MappingSchema\Hbm.generated.cs"
				: args[0]);
			if (!Directory.Exists(Path.GetDirectoryName(outFile)))
			{
				Console.Error.WriteLine("Invalid target path: directory does not exist.");
				Console.Error.WriteLine(outFile);
				Environment.ExitCode = -1;
				return;
			}
			var currentUiCulture = new CultureInfo("en-us");
			Thread.CurrentThread.CurrentCulture = currentUiCulture;
			Thread.CurrentThread.CurrentUICulture = currentUiCulture;
			new HbmCodeGenerator().Execute(outFile);
			Console.WriteLine("Done");
		}
	}
}
