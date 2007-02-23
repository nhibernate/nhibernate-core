using System;

namespace NHibernate.Tool.hbm2net
{
	/// <summary>
	/// Summary description for Bootstrap.
	/// </summary>
	public class Bootstrap
	{
		[STAThread]
		public static void Main(String[] args)
		{
			CodeGenerator.Main(args);
		}
	}
}