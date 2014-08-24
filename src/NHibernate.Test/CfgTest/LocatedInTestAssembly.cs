using System;

namespace NHibernate.Test.CfgTest
{
	/// <summary>
	/// This class should be located in NHibernate.Test assembly
	/// (used from ConfigurationFixture)
	/// </summary>
	public class LocatedInTestAssembly
	{
		private int id;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}