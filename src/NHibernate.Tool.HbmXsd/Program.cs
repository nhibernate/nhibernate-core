namespace NHibernate.Tool.HbmXsd
{
	public class Program
	{
		private static void Main(string[] args)
		{
			string outFile = args.Length == 0 ? @"..\..\..\NHibernate\Cfg\MappingSchema\Hbm.generated.cs" : args[0];
			new HbmCodeGenerator().Execute(outFile);
		}
	}
}