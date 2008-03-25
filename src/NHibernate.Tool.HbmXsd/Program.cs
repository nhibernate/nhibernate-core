using System;

namespace NHibernate.Tool.HbmXsd
{
	public class Program
	{
		private static void Main(string[] args)
		{
			// For debugging: ..\..\..\NHibernate\Cfg\MappingSchema\Hbm.generated.cs

			if (args.Length == 1)
				new HbmCodeGenerator().Execute(args[0]);
			else
				Console.WriteLine("usage: HbmXsd <outputfile>");
		}
	}
}