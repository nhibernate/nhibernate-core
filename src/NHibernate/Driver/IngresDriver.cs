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
		/// <summary></summary>
        public IngresDriver() : base(
            "Ca.Ingres.Client",
            "Ca.Ingres.Client.IngresConnection",
            "Ca.Ingres.Client.IngresCommand")
		{           
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInSql
		{
			get { return false; }
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInParameter
		{
			get { return false; }
		}

		/// <summary></summary>
		public override string NamedPrefix
		{
			get { return String.Empty; }
		}
	}
}