using System;

namespace NHibernate.Driver
{
	/// <summary>
	/// This provides a driver for Sybase ASE 15 using the ADO.NET driver.
	/// </summary>
	/// <remarks>
	/// You will need the following libraries available to your application:
	/// <ul>
	/// <li>Sybase.AdoNet2.AseClient.dll</li>
	/// <li>sybdrvado20.dll</li>
	/// </ul>
	/// </remarks>
	public class SybaseAseClientDriver : ReflectionBasedDriver
	{
		public SybaseAseClientDriver() : base("Sybase.AdoNet2.AseClient", "Sybase.Data.AseClient.AseConnection", "Sybase.Data.AseClient.AseCommand")
		{
		}
		
		public override string NamedPrefix
		{
			get { return "@"; }
		}
		
		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}
		
		public override bool UseNamedPrefixInSql
		{
			get { return true; }
		}
	}
}
