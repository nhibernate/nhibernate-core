using System;

namespace NHibernate.Tool.HbmXsd
{
	public class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length == 1)
				new HbmCodeGenerator().Execute(args[0]);
			else
				Console.WriteLine("usage: HbmXsd <outputfile>");
		}
	}
}