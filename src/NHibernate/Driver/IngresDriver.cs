using System;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Ingres DataProvider
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class IngresDriver : ReflectionBasedDriver
	{
		public IngresDriver() : base("Ingres.Client", "Ingres.Client.IngresConnection", "Ingres.Client.IngresCommand") {}

		public override bool UseNamedPrefixInSql
		{
			get { return false; }
		}

		public override bool UseNamedPrefixInParameter
		{
			get { return false; }
		}

		public override string NamedPrefix
		{
			get { return String.Empty; }
		}
	}
}